import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { OrderParams } from '../../shared/models/orderParams';
import { Pagination } from '../../shared/models/pagination';
import { Order } from '../../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  // Pobieranie listy zamówień z backendu z obsługą paginacji i filtra
  getOrders(orderParams: OrderParams){
    let params = new HttpParams();
    // Jeśli ustawiono filtr (i nie jest "All") – dodaj jako query param
    if(orderParams.filter && orderParams.filter !== 'All'){
      params = params.append('status', orderParams.filter);
    }
    // Dodaj parametry paginacji
    params = params.append('pageIndex', orderParams.pageNumber);
    params = params.append('pageSize', orderParams.pageSize);
    // Wysyłamy żądanie GET z parametrami do endpointu admin/orders
    return this.http.get<Pagination<Order>>(this.baseUrl + 'admin/orders', {params} );
  }

  // Pobranie jednego zamówienia po ID
  getOrder(id: number){
    return this.http.get<Order>(this.baseUrl + 'admin/orders/' + id);
  }

  // Wywołanie refundu zamówienia (POST)
  refundOrder(id: number){
    return this.http.post<Order>(this.baseUrl + 'admin/orders/refund/' + id, {});
  }
}
