import { Component } from '@angular/core';
import { MatBadge } from '@angular/material/badge';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-header', //Nazwa znacznika HTML, którego użyjesz w innych komponentach: <app-header>
  imports: [
    MatIcon,
    MatButton,
    MatBadge
  ],
  templateUrl: './header.component.html', //Ścieżka do pliku HTML (header.component.html
  styleUrl: './header.component.scss'
})
export class HeaderComponent {

}
