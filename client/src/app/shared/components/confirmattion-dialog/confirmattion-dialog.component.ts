import { Component, inject } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-confirmattion-dialog',
  imports: [
    MatButton
  ],
  templateUrl: './confirmattion-dialog.component.html',
  styleUrl: './confirmattion-dialog.component.scss'
})
export class ConfirmattionDialogComponent {
  // Referencja do tego okna dialogowego (pozwala je zamknąć i przekazać wartość do komponentu otwierającego)
  dialogRef = inject(MatDialogRef<ConfirmattionDialogComponent>)

  // Dane przekazane do dialogu (np. tekst pytania, opis) – dostępne przez MAT_DIALOG_DATA
  data = inject(MAT_DIALOG_DATA)

  // Wywołane po kliknięciu "Accept" – zamyka okno i przekazuje true (akcja potwierdzona)
  onConfirm(){
    this.dialogRef.close(true)
  }

  // Wywołane po kliknięciu "Cancel" – zamyka okno i nie przekazuje żadnej wartości (undefined)
  onCancel(){
    this.dialogRef.close()
  }
}
