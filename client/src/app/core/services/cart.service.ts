import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Cart, CartItem } from '../../shared/models/cart';
import { Product } from '../../shared/models/product';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  baseUrl = environment.apiUrl
  private http = inject(HttpClient)
  cart = signal<Cart | null>(null)

  //Total sum quantity in the cart
  itemCount = computed(() => { 
    return this.cart()?.items.reduce((sum, item) => sum + item.quantity, 0)
  })

  // Calculates order totals from the cart
  totals = computed(() => {
    const cart = this.cart()

    if(!cart) return null

    const subtotal = cart.items.reduce((sum, item) => sum + (item.price * item.quantity), 0)
    const shipping = 0
    const discount = 0

    return {
      subtotal,
      shipping,
      discount,
      total: subtotal + shipping - discount
    }
  })
  
  //Get shopping cart by cart id
  getCart(id: string){
    return this.http.get<Cart>(this.baseUrl + "cart?id=" + id).pipe(
      map(cart => {
        this.cart.set(cart)
        return cart
      })
    )
  }

  //Set schopping cart
  setCart(cart: Cart){
    return this.http.post<Cart>(this.baseUrl + 'cart', cart).subscribe({
      next: cart => this.cart.set(cart)
    })
  }

  //Add/Update item to cart
  addItemToCart(item: CartItem | Product, quantity = 1){
    const cart = this.cart() ?? this.createCart()

    //check if item is Product type
    if(this.isProduct(item)){
      item = this.mapProductToCartItem(item)
    }

    //Add/Update CartItem in Shopping Cart
    cart.items = this.addOrUpdateItem(cart.items, item, quantity)
    this.setCart(cart)
  }

  //Delete item from cart or change quantity
  removeItemFromCart(productId: number, quantity = 1){
    const cart = this.cart()
    if(!cart) return

    //find index searching product
    const index = cart.items.findIndex(x => x.productId == productId)

    //if product exist in cart
    if(index !== -1){
      if(cart.items[index].quantity > quantity){
        cart.items[index].quantity -= quantity
      }else{
        cart.items.splice(index, 1) //delete
      }

      if(cart.items.length === 0){
        //delete cart
        this.deleteCart()
      }else{
        //update cart
        this.setCart(cart)
      }
    }
  }

  //Delete cart
  deleteCart() {
    this.http.delete(this.baseUrl + 'cart?id=' + this.cart()?.id).subscribe({
      next: () => {
        localStorage.removeItem('cart_id')
        this.cart.set(null)
      }
    })
  }

  //Add new CartItem (Product) to cart or just update
  private addOrUpdateItem(items: CartItem[] = [], item: CartItem, quantity: number): CartItem[] {
    const index = items.findIndex(x => x.productId === item.productId);
    
    if(index === -1){
      item.quantity = quantity
      items.push(item)
    } else{
      items[index].quantity += quantity
    }
    return items
  }


  //Transfer Product item to CartItem type
  private mapProductToCartItem(item: Product): CartItem {
    return{
      productId: item.id,
      productName: item.name,
      price: item.price,
      quantity: 0,
      pictureUrl: item.pictureUrl,
      brand: item.brand,
      type: item.type
    }
  }

  //Create new shopping cart with uqniue id and add this to localstorage
  private createCart(): Cart{
    const cart = new Cart()
    localStorage.setItem('cart_id', cart.id)
    return cart
  }

  //check if item is Product type
  private isProduct(item: CartItem | Product) : item is Product { //Pozwala TypeScriptowi w kolejnych linijkach kodu wiedzieć, że możesz traktować ten obiekt jako Product
    //Sprawdza, czy pole id istnieje na obiekcie
    //Zakładamy, że tylko Product ma pole id (a CartItem nie ma lub ma inną nazwę)
    //Jeśli pole id istnieje (czyli jest różne od undefined), funkcja zwraca true — to znaczy, że item jest produktem
    return (item as Product).id !== undefined
  }

}
