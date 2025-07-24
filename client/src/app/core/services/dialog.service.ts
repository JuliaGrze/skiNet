import { inject, Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmattionDialogComponent } from '../../shared/components/confirmattion-dialog/confirmattion-dialog.component';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DialogService {
  // Wstrzykuje serwis dialogowy Angular Material do otwierania okien dialogowych
  private dialog = inject(MatDialog);

  // Metoda confirm służy do wyświetlenia okna potwierdzenia z podanym tytułem i wiadomością
  confirm(title: string, message: string){
    // Otwiera ConfirmattionDialogComponent jako dialog, przekazując dane i szerokość
    const dialogRef = this.dialog.open(ConfirmattionDialogComponent, {
      width: '400px',
      data: {title, message}
    });

    // Zwraca Promise z wartością z dialogu (true, jeśli potwierdzone, undefined jeśli anulowane)
    // firstValueFrom zamienia Observable z afterClosed() na Promise (łatwość użycia z async/await)
    return firstValueFrom(dialogRef.afterClosed());
  }
}
