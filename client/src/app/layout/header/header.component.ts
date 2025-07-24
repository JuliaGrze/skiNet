import { Component, inject } from '@angular/core';
import { MatBadge } from '@angular/material/badge';
import { MatButton, MatButtonModule } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { BusyService } from '../../core/services/busy.service';
import {MatProgressBar} from '@angular/material/progress-bar';
import { CartService } from '../../core/services/cart.service';
import { AccountService } from '../../core/services/account.service';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { IsAdminDirective } from '../../shared/directives/is-admin.directive';

@Component({
  selector: 'app-header', //Nazwa znacznika HTML, którego użyjesz w innych komponentach: <app-header>
  imports: [
    MatIcon,
    MatButton,
    MatBadge,
    RouterLink,
    RouterLinkActive,
    MatProgressBar,
    MatMenuModule,
    MatButtonModule,
    MatDividerModule,
    IsAdminDirective
  ],
  templateUrl: './header.component.html', //Ścieżka do pliku HTML (header.component.html
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  busyService = inject(BusyService)
  cartService = inject(CartService)
  accountService = inject(AccountService)
  private router =  inject(Router)

  logOut(){
    this.accountService.logout().subscribe({
      next: () =>{
        this.accountService.currentUser.set(null)
        this.router.navigateByUrl('/')
      }
    })
  }
}
