import { Component, inject } from '@angular/core';
import { MatBadge } from '@angular/material/badge';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { BusyService } from '../../core/services/busy.service';
import {MatProgressBar} from '@angular/material/progress-bar';

@Component({
  selector: 'app-header', //Nazwa znacznika HTML, którego użyjesz w innych komponentach: <app-header>
  imports: [
    MatIcon,
    MatButton,
    MatBadge,
    RouterLink,
    RouterLinkActive,
    MatProgressBar
  ],
  templateUrl: './header.component.html', //Ścieżka do pliku HTML (header.component.html
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  busyService = inject(BusyService)
}
