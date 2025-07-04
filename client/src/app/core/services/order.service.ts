import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Order, OrderToCreate } from '../../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  baseUrl = environment.apiUrl
  private http = inject(HttpClient)
  orderComplete = false

  //Create order
  createOrder(orderToCreate: OrderToCreate){
    return this.http.post<Order>(this.baseUrl + 'orders', orderToCreate)
  }

  //get all user orders
  getOrdersForUser(){
    return this.http.get<Order[]>(this.baseUrl + 'orders')
  }

  //get user order by order id
  getOrderDetailed(id: number){
    return this.http.get<Order>(this.baseUrl + 'orders/' + id)
  }
}
