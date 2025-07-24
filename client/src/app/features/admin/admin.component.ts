import { AfterViewInit, Component, inject, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule} from '@angular/material/table';
import { Order } from '../../shared/models/order';
import { AdminService } from '../../core/services/admin.service';
import { OrderParams } from '../../shared/models/orderParams';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MatLabel } from '@angular/material/form-field';
import {MatSelectChange, MatSelectModule} from '@angular/material/select';
import {MatTooltipModule} from '@angular/material/tooltip';
import {MatTabsModule} from '@angular/material/tabs'
import { RouterLink } from '@angular/router';
import { DialogService } from '../../core/services/dialog.service';

@Component({
  selector: 'app-admin',
  imports: [
    MatTableModule, 
    MatPaginatorModule,
    MatButton,
    MatIcon,
    MatSelectModule,
    DatePipe,
    CurrencyPipe,
    MatLabel,
    MatTooltipModule,
    MatTabsModule,
    RouterLink
  ],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss'
})
export class AdminComponent implements OnInit {
  // Kolumny wyświetlane w tabeli (muszą odpowiadać polom w Order + "action" np. do refundu)
  displayedColumns: string[] = ['id', 'buyerEmail', 'orderDate',  'status', 'total', 'action'];
  // Źródło danych dla Angular Material Table
  dataSource = new MatTableDataSource<Order>([]);
  // Wstrzykujemy serwis admina (do pobierania zamówień)
  private adminService = inject(AdminService);
  private dialogService = inject(DialogService)
  // Obiekt z parametrami wyszukiwania/paginacji
  orderParams = new OrderParams()
  // Liczba wszystkich zamówień (do wyświetlania paginacji)
  totalItems = 0;
  // Lista statusów do filtrowania (dopasuj do swojego backendu!)
  statusOption = ['All', 'PaymentReceive', 'PaymentMismatch', 'Refunded', 'Pending'];

  // Po załadowaniu komponentu – ładujemy pierwszą stronę zamówień
  ngOnInit(): void {
    this.loadOrders();
  }

  // Pobieranie zamówień z backendu według aktualnych parametrów
  loadOrders() {
    this.adminService.getOrders(this.orderParams).subscribe({
      next: response => {
        if (response.data) {
          this.dataSource.data = response.data; // Wstaw zamówienia do tabeli
          this.totalItems = response.count;     // Ustaw liczbę wszystkich zamówień do paginacji
        }
      }
    });
  }

  // Obsługa zmiany strony w paginacji
  onPageChange(event: PageEvent) {
    this.orderParams.pageNumber = event.pageIndex + 1; // Angular Material paginacja zaczyna od 0!
    this.orderParams.pageSize = event.pageSize;
    this.loadOrders();
  }

  // Obsługa zmiany filtra statusu zamówień
  onFilterSelect(event: MatSelectChange) {
    this.orderParams.filter = event.value;
    this.orderParams.pageNumber = 1; // Wracamy na pierwszą stronę przy zmianie filtra
    this.loadOrders();
  }

  // Otwiera okno potwierdzenia refundu i wykonuje refundację jeśli user potwierdzi
  async openConfirmDialog(id: number){
    // Otwórz dialog potwierdzenia i poczekaj na decyzję użytkownika (true/undefined)
    const confirmed = await this.dialogService.confirm(
      'Confirm refund',
      'Are you sure you want to issue this refund? This cannot be undone'
    )
    // Jeśli user potwierdził (kliknął "Confirm"), wykonaj refundację
    if(confirmed) this.refundOrder(id)
  }

  //Zwrocenie zamowienia
  refundOrder(id: number){
    this.adminService.refundOrder(id).subscribe({
      next: order => {
        //zamieniamy zamówienie o tym id w tabeli na nowe zamówienie (z nowym statusem/refundem).
        //Mapujemy tablicę zamówień: jeśli id się zgadza, podmieniamy ten obiekt na zaktualizowane zamówienie z backendu
        this.dataSource.data = this.dataSource.data.map(o => o.id === id ? order : o)
      }
    })
  }
}
