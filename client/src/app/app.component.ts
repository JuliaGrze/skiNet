import { Component} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./layout/header/header.component";
import { ShopComponent } from "./features/shop/shop.component";


@Component({
    selector: 'app-root', //Nazwa znacznika HTML – <app-root> (użyty właśnie w index.html)
    imports: [RouterOutlet, HeaderComponent], //Tu podajesz inne komponenty/moduły, z których ten komponent korzysta
    templateUrl: './app.component.html', 
    styleUrl: './app.component.scss'
})
export class AppComponent{
    title = 'SkiNet';
}
