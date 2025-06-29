  import { inject, Injectable } from '@angular/core';
  import { ConfirmationToken, loadStripe, Stripe, StripeAddressElement, StripeAddressElementOptions, StripeElements, StripePaymentElement } from '@stripe/stripe-js'
  import { environment } from '../../../environments/environment';
  import { HttpClient } from '@angular/common/http';
  import { CartService } from './cart.service';
  import { Cart } from '../../shared/models/cart';
  import { firstValueFrom, map } from 'rxjs';
  import { AccountService } from './account.service';

  @Injectable({
    providedIn: 'root'
  })
  export class StripeService {
    baseUrl = environment.apiUrl
    private http = inject(HttpClient)
    private cartService = inject(CartService)
    private accountService = inject(AccountService)
    //manages all communication with Stripe from the frontend
    private stripePromise?: Promise<Stripe | null>
    //Securely collects data and transfers data directly to Stripe
    private elements?: StripeElements 
    private addressElement?: StripeAddressElement
    private paymentElement?: StripePaymentElement

    constructor(){
      this.stripePromise = loadStripe(environment.stripePublicKey)
    }

    // Returns a Stripe instance (or null if something went wrong)
    getStripeInstance(){
      return this.stripePromise
    }

    //przygotowuje wszystko, zanim pokażesz użytkownikowi formularz płatności kartą i pozwolisz mu zapłacić przez Stripe
    async initializeElements(){
      if(!this.elements){
        const stripe = await this.getStripeInstance()
        if(stripe){
          //firstValueFrom(...) konwertuje Observable (RxJS) do klasycznego Promise, żeby można było użyć await
          const cart = await firstValueFrom(this.createOrUpdatePaymentIntent())
          this.elements = stripe.elements(
            {clientSecret: 
              cart.clientSecret, 
              appearance: {labels: 'floating'}, //opcjonalna konfiguracja wyglądu (np. labelki unosi się nad inputem)
              locale: 'en'  // 🔁 wymuszenie języka angielskiego
            })
        }else{
          throw new Error('Stripe has not been loaded')
        }
      }
      return this.elements
    }

    //Tworzy komponent Stripe do wprowadzania adresu dostawy — AddressElement
    async createAddressElement(){
      if(!this.addressElement){
        //inicjalizujesz Stripe Elements (czyli pobierasz clientSecret i konfigurujesz Stripe)
        const elements = await this.initializeElements()
        if(elements){
          const user = this.accountService.currentUser()
          //domyslne wartosci w formularzu - ma dokładnie taki sam typ, jak pole defaultValues z obiektu StripeAddressElementOptions
          let defaultValues: StripeAddressElementOptions['defaultValues'] = {}

          if(user){
            defaultValues.name = user.firstName + ' ' + user.lastName
          }
          // jeśli użytkownik ma dane zapisane na koncie, formularz Stripe automatycznie je uzupełni.
          if(user?.address){
            defaultValues.address = {
              line1: user.address.line1,
              line2: user.address.line2,
              city: user.address.city,
              state: user.address.state,
              country: user.address.country,
              postal_code: user.address.postalCode
            }
          }

          const options: StripeAddressElementOptions = {
            //mode: 'shipping' oznacza, że chodzi o adres dostawy
            mode: 'shipping',
            defaultValues //dane domysle
          }
          //Tworzysz komponent adresu (gotowy do osadzenia w HTML)
          this.addressElement = elements.create('address', options)
        }else{
          throw new Error('Elements instance has not been loaded')
        }
      }
      return this.addressElement
    }

    // Tworzy komponent Stripe do obsługi płatności (PaymentElement)
    async createPaymentElement(){
      // Jeśli element płatności jeszcze nie istnieje – utwórz go
      if(!this.paymentElement){
        // Zainicjalizuj Stripe Elements (pobierze clientSecret i inne dane potrzebne do renderowania formularza)
        const elements = await this.initializeElements()
        if(elements){
          // Utwórz nowy element płatności (czyli gotowy komponent formularza płatności Stripe)
          this.paymentElement = elements.create('payment')
        }else{
          throw new Error('Elements instance has not been initialized')
        }
      }
      return this.paymentElement
    }

    // Tworzy "confirmation token" w Stripe 
    // potrzebny np. do późniejszej autoryzacji lub przetwarzania płatności na backendzie
    async createConfirmationToken(){
      // 1. Pobiera instancję Stripe (wcześniej załadowaną przez loadStripe)
      const stripe = await this.getStripeInstance()
      // 2. Inicjalizuje Stripe Elements – pobiera clientSecret i konfiguruje formularz
      const elements = await this.initializeElements()
      // 3. Próbuje wysłać dane formularza Stripe do Stripe (np. dane karty, adres itp.)
      const result = await elements.submit()

      // 4. Jeśli pojawi się błąd walidacji (np. brak danych karty) — rzuć wyjątek
      if(result.error) throw new Error(result.error.message)

      // 5. Jeśli instancja Stripe istnieje — tworzy tzw. "confirmation token"
      if(stripe){
        return await stripe.createConfirmationToken({elements})
      } else 
        throw new Error('Stripe not avaliable')
    }

    // potwierdza płatność przez Stripe
    async confirmPayemnt(confirmationToken: ConfirmationToken){
      // Pobiera Stripe (czyli bibliotekę do obsługi płatności)
      const stripe = await this.getStripeInstance()
      // Tworzy formularze Elements (jeśli jeszcze nie ma)
      const elements = await this.initializeElements()
      // Wysyła dane z formularzy do Stripe
      const result = await elements.submit()

      // Jeśli pojawi się błąd walidacji (np. brak danych karty) — rzuć wyjątek
      if(result.error) throw new Error(result.error.message)

      // Pobiera clientSecret z koszyka
      const clientSecret = this.cartService.cart()?.clientSecret

      // Jeśli wszystko jest OK — wysyła dane do Stripe
      if(stripe && clientSecret){
        // Potwierdza płatność i przekazuje wszystkie dane potrzebne do realizacji transakcji.
        return await stripe.confirmPayment({
          //To unikalny tajny identyfikator wygenerowany przez Stripe dla twojej płatności (koszyka)
          clientSecret: clientSecret,
          confirmParams: {
            confirmation_token: confirmationToken.id
          },
          redirect: 'if_required'
        })
      } else{
        throw new Error('Unable to load stripe')
      }
    }

    // Creates or updates PaymentIntent in backend based on cart
    createOrUpdatePaymentIntent(){
      const cart = this.cartService.cart()

      if(!cart) throw new Error('Problem with cart')

      return this.http.post<Cart>(this.baseUrl + 'payments/' + cart.id, {}).pipe(
        map(cart => {
          // We save the basket in frontend and backend
          this.cartService.setCart(cart)
          return cart
        })
      )
    }
    //zresetowanie stanu Stripe Elements
    disposeElements(){
      this.elements = undefined
      this.addressElement = undefined
      this.paymentElement = undefined
    }
  }
