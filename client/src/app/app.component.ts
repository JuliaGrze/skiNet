import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root', //Nazwa znacznika HTML – <app-root> (użyty właśnie w index.html)
  imports: [RouterOutlet], //Tu podajesz inne komponenty/moduły, z których ten komponent korzysta
  templateUrl: './app.component.html', //Ścieżka do pliku HTML zawierającego szablon tego komponentu
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'SkiNet';
}
