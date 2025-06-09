import { Component } from '@angular/core';
import { MatCard } from '@angular/material/card';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  imports: [
    MatCard
  ],
  templateUrl: './server-error.component.html',
  styleUrl: './server-error.component.scss'
})
export class ServerErrorComponent {
  error?: any

  constructor(private router: Router) {
    //Ten kod w konstruktorze pobiera przekazane dane (np. error) z poprzedniej strony 
    // i przypisuje do właściwości, by móc je wyświetlić na stronie docelowej
    const navigation = this.router.getCurrentNavigation()
    this.error = navigation?.extras.state?.['error']
  }

}
