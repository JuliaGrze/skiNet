import { inject, Injectable } from '@angular/core';
import {MatSnackBar} from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
// własny serwis do wyświetlania snackbarów, moozna go uzywac wszedzie w aplikacji – 
// - nie musisz za każdym razem powtarzać kodu z MatSnackBar
export class SnackbarService {
  private snackbar = inject(MatSnackBar)

  error(message: string){
    this.snackbar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['snack-error']
    })
  }

  success(message: string){
    this.snackbar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['snack-success']
    })
  }
}
