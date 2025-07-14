import { Component, inject, input, output } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { BusyService } from '../../../core/services/busy.service';

@Component({
  selector: 'app-empty-state',
  imports: [
    MatIcon,
    MatButton
  ],
  templateUrl: './empty-state.component.html',
  styleUrl: './empty-state.component.scss'
})
export class EmptyStateComponent {
  busyService = inject(BusyService)
  message = input.required<string>()
  icon = input.required<string>()
  actionText = input.required<string>()
  //deklaracja eventu, który komponent będzie wysyłał na zewnątrz
  //Dzięki temu rodzic może reagować na coś, co się wydarzyło w środku tego komponentu.
  action = output<void>()

  //To metoda, która jest wywoływana np. po kliknięciu przycisku w szablonie
  onAction(){
    //Wyślij sygnał/zdarzenie o nazwie action
    this.action.emit()
  }
}
