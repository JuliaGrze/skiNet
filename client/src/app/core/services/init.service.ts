import { inject, Injectable } from '@angular/core';
import { CartService } from './cart.service';
import { forkJoin, of, tap } from 'rxjs';
import { AccountService } from './account.service';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
//Persist
export class InitService {
  private cartService = inject(CartService)
  private accountService = inject(AccountService)
  private signalrService = inject(SignalrService)

  init(){
    const cartId = localStorage.getItem('cart_id')
    // Jeśli mamy id koszyka, pobieramy go z backendu, 
    // w przeciwnym razie zwracamy Observable z wartością null
    const cart$ = cartId ? this.cartService.getCart(cartId) : of(null)

    // forkJoin łączy dwie obserwable: koszyk i dane użytkownika
    return forkJoin({
      cart: cart$,  // pobieranie koszyka (lub null)
      user: this.accountService.getUserInfo().pipe(
        tap(user => {
          // Jeśli jest zalogowany użytkownik (user != null), 
          // to tworzymy połączenie SignalR
          if(user)
            this.signalrService.createHubConnection()
        })
      )
    })
  }
}
