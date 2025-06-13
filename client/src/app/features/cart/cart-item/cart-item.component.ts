import { Component, inject, input } from '@angular/core';
import { CartItem } from '../../../shared/models/cart';
import { RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CurrencyPipe } from '@angular/common';
import { CartService } from '../../../core/services/cart.service';

@Component({
  selector: 'app-cart-item',
  imports: [
    RouterLink,
    MatIconModule,
    MatButtonModule,
    CurrencyPipe
  ],
  templateUrl: './cart-item.component.html',
  styleUrl: './cart-item.component.scss'
})
export class CartItemComponent {
  //Oznacza, że każdy, kto używa <app-cart-item>, MUSI przekazać atrybut [item], czyli CartItem
  item = input.required<CartItem>()
  cartService = inject(CartService)

  // quantity += 1
  incrementQuantity(){
    this.cartService.addItemToCart(this.item())
  }

  // quantity -= 1
  decrementQuantity(){
    this.cartService.removeItemFromCart(this.item().productId)
  }

  //remove item from cart
  removeItemFromCart(){
    this.cartService.removeItemFromCart(this.item().productId, this.item().quantity)
  }
}
