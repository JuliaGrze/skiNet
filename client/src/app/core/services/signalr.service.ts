import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr'
import { Order } from '../../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  hubUrl = environment.hubUrl
   // Obiekt reprezentujący połączenie SignalR
  hubConnection?: HubConnection
  // Reactive signal przechowujący ostatnie powiadomienie o zamówieniu (może być null)
  orderSignal = signal<Order | null>(null)

  // Metoda inicjuje połączenie z SignalR Hubem 
  createHubConnection(){
    // Tworzy nową instancję buildera połączenia  
    this.hubConnection = new HubConnectionBuilder() 
    .withUrl(this.hubUrl, {
      //Wysyłaj ciasteczka i nagłówki uwierzytelniania (np. tokeny JWT)  
      withCredentials: true
    })
     // Automatycznie ponawia połączenie po rozłączeniu  
    .withAutomaticReconnect()
    // Buduje gotowe połączenie  
    .build()

    // Uruchamia połączenie z hubem
    this.hubConnection.start()
    .catch(error => console.log(error))

    // Rejestruje odbiornik eventu 'OrderCompleteNotification' wysyłanego przez backend  
    this.hubConnection.on('OrderCompleteNotification', (order: Order) => {
      // Ustawia nową wartość w sygnale (aktualizuje subskrybentów)  
      this.orderSignal.set(order)
    })
  }

  // Metoda zamyka połączenie z hubem, jeśli jest aktywne  
  stopHubConnection(){
    if(this.hubConnection?.state === HubConnectionState.Connected){
      this.hubConnection.stop()
      .catch(error => console.log(error))
    }
  }
}
