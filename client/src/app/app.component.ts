import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./layout/header/header.component";
import { HttpClient } from '@angular/common/http';
import { Product } from './shared/models/product';
import { Pagination } from './shared/models/pagination';

@Component({
    selector: 'app-root', //Nazwa znacznika HTML – <app-root> (użyty właśnie w index.html)
    imports: [RouterOutlet, HeaderComponent], //Tu podajesz inne komponenty/moduły, z których ten komponent korzysta
    templateUrl: './app.component.html', 
    styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit{
    baseUrl = 'https://localhost:5001/api/'
    private http = inject(HttpClient)
    //lub
    // constructor(private http: HttpClient){}
    title = 'SkiNet';
    products: Product[] = []

  //request to backend to get product list
    ngOnInit(): void {
        this.http.get<Pagination<Product>>(this.baseUrl + 'products').subscribe({ //subscribe - uruchamia zapytanie HTTP i nasłuchuje na odpowiedź
            next: response => this.products = response.data, 
            error: error => console.log(error), 
            complete: () => console.log('complete') //Wywoływane, gdy cały proces zapytania się zakończy.
        })
    }
}
