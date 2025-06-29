import { Component, inject, OnInit } from '@angular/core';
import { OrderSummaryComponent } from "../../shared/components/order-summary/order-summary.component";
import { MatButtonModule } from '@angular/material/button';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatStepper, MatStepperModule} from '@angular/material/stepper';
import { Router, RouterLink } from '@angular/router';
import { StripeService } from '../../core/services/stripe.service';
import { ConfirmationToken, StripeAddressElement, StripeAddressElementChangeEvent, StripePaymentElement, StripePaymentElementChangeEvent } from '@stripe/stripe-js';
import { SnackbarService } from '../../core/services/snackbar.service';
import {MatCheckboxChange, MatCheckboxModule} from '@angular/material/checkbox';
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { Address } from '../../shared/models/user';
import { firstValueFrom } from 'rxjs';
import { AccountService } from '../../core/services/account.service';
import { CheckoutDeliveryComponent } from "./checkout-delivery/checkout-delivery.component";
import { CheckoutReviewComponent } from "./checkout-review/checkout-review.component";
import { CartService } from '../../core/services/cart.service';
import { CurrencyPipe, JsonPipe } from '@angular/common';
import { signal } from '@angular/core';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';

@Component({
  selector: 'app-checkout',
  imports: [
    OrderSummaryComponent,
    MatButtonModule,
    MatStepperModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    RouterLink,
    MatCheckboxModule,
    CheckoutDeliveryComponent,
    CheckoutReviewComponent,
    CurrencyPipe,
    MatProgressSpinnerModule
],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss'
})
export class CheckoutComponent implements OnInit{
  private stripeService = inject(StripeService)
  private snackbar = inject(SnackbarService)
  private accountService = inject(AccountService)
  private router = inject(Router)
  cartService = inject(CartService)
  addressElement?: StripeAddressElement
  paymentElement?: StripePaymentElement
  saveAddress = false
  completionStatus = signal<{address: boolean, cart: boolean, delivery: boolean}>(
    {address: false, cart: false, delivery: false}
  )
  confirmationtoken?: ConfirmationToken
  loading = false

  async ngOnInit() {
    try{
      //set default values in address
      this.addressElement = await this.stripeService.createAddressElement()
      this.addressElement.mount('#address-element')
      // Rejestruje funkcję handleAddressChange jako nasłuchiwacza zdarzeń typu change
      // Gdy użytkownik coś zmieni w formularzu adresu, ta metoda zostanie automatycznie wywołana
      this.addressElement.on('change', this.handleAddressChange)

      this.paymentElement = await this.stripeService.createPaymentElement()
      this.paymentElement.mount('#payment-element')
      this.paymentElement.on('change', this.handlePaymentChange)
    }catch(error: any) {
      this.snackbar.error(error.message)
    }
  }

  //  Ta metoda odbiera dane o zmianach w formularzu dotyczncym adresu
  handleAddressChange = (event: StripeAddressElementChangeEvent) => { 
    this.completionStatus.update(state => {
      state.address = event.complete
      return state
    })
  }
  //  Ta metoda odbiera dane o zmianach w formularzu dotyczncym metody platnosci
  handlePaymentChange = (event: StripePaymentElementChangeEvent) => {
    this.completionStatus.update(state => {
      state.cart = event.complete
      return state
    })
  }

  //  Ta metoda odbiera dane o zmianach w formularzu dotyczncym rodzaju dostawy
  handleDeliveryChange(event: boolean){
    this.completionStatus.update(state => {
      state.delivery = event
      return state
    })
  }

  // Updates the saveAddress flag depending on the checkbox state
  onSaveAddressCheckboxChange(event: MatCheckboxChange){
    this.saveAddress = event.checked
  }

  // zbiera wszystkie dane z formularzy płatności i adresu, a potem pyta Stripe o specjalny token, 
  // który pozwoli backendowi potwierdzić płatność w bezpieczny sposób.
  async getConfirmationToken(){
    try{
      //sprawdza czy wszystkie 3 kroki checkoutu sa zakoncozne 
      if(Object.values(this.completionStatus()).every(status => status == true)){
        // proba utworzenia confirmation token za pomocą Stripe
        const result = await this.stripeService.createConfirmationToken()

        //Jeśli Stripe zwróci błąd (np. niepełne dane w formularzu), zgłasza wyjątek
        if(result.error) throw new Error(result.error.message)
          // Jeśli wszystko poszło OK — zapisuje token do zmiennej i wypisuje go w konsoli
          this.confirmationtoken = result.confirmationToken
          console.log(this.confirmationtoken)
      }
    } catch (error: any){
      this.snackbar.error(error.message)
    }
  }

  // wykonuje określone akcje przy przechodzeniu między krokami w mat-stepper podczas procesu checkoutu
  async onStepChange(event:StepperSelectionEvent ){
    // Zapisuje adres do backendu, jeśli użytkownik przechodzi do kolejnego kroku
    // i np w 1 zaznaczył checkbox "Save as default address"  
    if(event.selectedIndex === 1){
      if(this.saveAddress){
        const address = await this.getAddressFromStripeAddress()
        address && await firstValueFrom(this.accountService.updateAddress(address))
      }
    }
    if(event.selectedIndex === 2){
      await firstValueFrom(this.stripeService.createOrUpdatePaymentIntent())
    }
    if(event.selectedIndex === 3){
      // Stripe zbiera wszystkie dane z formularzy (adres + karta)
      // I generuje tzw. confirmation token, który zostanie użyty do finalizacji płatności przez backend
      await this.getConfirmationToken()
    }
  }

  // potwierdza płatność przez Stripe
  async confirmPayment(stepper: MatStepper){
    this.loading = true
    try{
      if(this.confirmationtoken){
        // Wywołuje metodę confirmPayemnt() z serwisu Stripe
        // Stripe wysyła dane na swoje serwery i wykonuje faktyczną płatność kartą na podstawie tokena
        const result = await this.stripeService.confirmPayemnt(this.confirmationtoken)

        if(result.error){
          throw new Error(result.error.message)
        }else{
          this.cartService.deleteCart()
          this.cartService.selectedDelivery.set(null)
          this.router.navigateByUrl('/checkout/success')
        }
      }
    } catch(error: any){
      this.snackbar.error(error.message || 'Spmething went wrong')
      stepper.previous()
    } finally{
      this.loading = false
    }
  }

  //Gets the address data that the user entered in the Stripe AddressElement form
  private async getAddressFromStripeAddress() :Promise<Address | null>{
    const result = await this.addressElement?.getValue()
    const address = result?.value.address

    if(address){
      return{
        line1: address.line1,
        line2: address.line2 || undefined,
        city: address.city,
        country: address.country,
        state: address.state,
        postalCode: address.postal_code
      }
    }else{
      return null
    }
  }

  ngOnDestroy() : void{
    this.stripeService.disposeElements()
  }

}
