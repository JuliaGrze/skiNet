import { Component, inject, OnInit } from '@angular/core';
import { OrderService } from '../../../core/services/order.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Order } from '../../../shared/models/order';
import { MatCardModule } from '@angular/material/card';
import { MatButton } from '@angular/material/button';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { AddressPipe } from '../../../shared/pipes/address.pipe';
import { PaymentCardPipe } from '../../../shared/pipes/payment-card.pipe';
import { AccountService } from '../../../core/services/account.service';
import { AdminService } from '../../../core/services/admin.service';

@Component({
  selector: 'app-order-detailed',
  imports: [
    MatCardModule,
    MatButton,
    DatePipe,
    CurrencyPipe,
    AddressPipe,
    PaymentCardPipe,
    RouterLink
  ],
  templateUrl: './order-detailed.component.html',
  styleUrl: './order-detailed.component.scss'
})
export class OrderDetailedComponent implements OnInit {
  private orderService = inject(OrderService)
  //o serwis Angulara, który reprezentuje aktualną ścieżkę (routę) i pozwala Ci odczytać informacje o niej 
  private activatedRoute = inject(ActivatedRoute)
  private accountService = inject(AccountService)
  private adminService = inject(AdminService)
  private router = inject(Router)
  order?: Order
  // Tekst przycisku powrotu zależny od roli
  buttonText = this.accountService.isAdmin() ? 'Return to admin' : 'Retturn to orders'

  ngOnInit(): void {
    this.loadOrder()
  }

  // Obsługa kliknięcia przycisku powrotu:
  // - admin wraca do panelu admina
  // - zwykły user do historii zamówień
  onReturnClick(){
    this.accountService.isAdmin()
      ? this.router.navigateByUrl('/admin')
      : this.router.navigateByUrl('/orders')
  }

  // Ładuje szczegóły zamówienia z backendu:
  // - admin: może pobrać każde zamówienie po id (przez AdminService)
  // - zwykły user: tylko własne zamówienie (przez OrderService)
  loadOrder(){
    //Pobierz paramtetr id z URL
    const id = this.activatedRoute.snapshot.paramMap.get('id')
    if(!id) return

    // Zdecyduj, który serwis użyć (zależnie od roli)
    const loadOrderData = this.accountService.isAdmin()
      ? this.adminService.getOrder(+id)
      : this.orderService.getOrderDetailed(+id)

    //Pobierz szczegóły order
    loadOrderData.subscribe({
      next: order => this.order = order
    })
  }
}
