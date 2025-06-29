import { Component, inject, OnInit, output } from '@angular/core';
import { CheckoutService } from '../../../core/services/checkout.service';
import {MatRadioModule} from '@angular/material/radio';
import { CurrencyPipe } from '@angular/common';
import { CartService } from '../../../core/services/cart.service';
import { DeliveryMethod } from '../../../shared/models/deliveryMethod';

@Component({
  selector: 'app-checkout-delivery',
  imports: [
    MatRadioModule,
    CurrencyPipe
  ],
  templateUrl: './checkout-delivery.component.html',
  styleUrl: './checkout-delivery.component.scss'
})
export class CheckoutDeliveryComponent implements OnInit{
  checkoutService = inject(CheckoutService)
  cartService = inject(CartService)
  // oznacza, że CheckoutDeliveryComponent wysyła zdarzenie do rodzica
  deliveryComplete = output<boolean>()

  ngOnInit(): void {
    // Downloading shipping methods from backend
    // and setting the selected one if it was previously selected in the shopping cart
    this.checkoutService.getDeliveryMethods().subscribe({
      next: methods =>  {
        //check if the deliveryMethodId is already saved in the cart
        if(this.cartService.cart()?.deliveryMethodId){
          //searching in downloaded methods for the one that matches the saved deliveryMethodId from the cart
          const method = methods.find(x => x.id === this.cartService.cart()?.deliveryMethodId)
          if(method){
            this.cartService.selectedDelivery.set(method)
            this.deliveryComplete.emit(true)
          }
        }
      }
    })
  }

  //update delivery method in cart and save it in db
  updateDeliveryMethod(method: DeliveryMethod){
    this.cartService.selectedDelivery.set(method)
    const cart = this.cartService.cart()
    if(cart){
      cart.deliveryMethodId = method.id
      this.cartService.setCart(cart)
      this.deliveryComplete.emit(true)
    }
  }
}
