import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  loading = false
  busyRequestCount = 0

  // wywołujesz na początku każdego requestu
  busy(){
    this.busyRequestCount++
    this.loading = true
  }

  //wywołujesz po zakończeniu requestu (sukces lub błąd)
  idle(){
    this.busyRequestCount--
    if(this.busyRequestCount <= 0){
      this.busyRequestCount = 0
      this.loading = false
    }
  }
}
