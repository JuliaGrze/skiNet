import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../core/services/shop.service';
import { Product } from '../../shared/models/product';
import {MatCard} from '@angular/material/card';
import { ProductItemComponent } from "./product-item/product-item.component";
import { MatDialog } from '@angular/material/dialog';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatListOption, MatSelectionList, MatSelectionListChange } from '@angular/material/list';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { ShopParams } from '../../shared/models/shopParams';
import {MatPaginator} from '@angular/material/paginator';
import { Pagination } from '../../shared/models/pagination';
import { PageEvent } from '@angular/material/paginator';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-shop',
  imports: [
    ProductItemComponent,
    MatButton,
    MatIcon,
    MatMenu,
    MatSelectionList,
    MatListOption,
    MatMenuTrigger,
    MatPaginator,
    FormsModule
],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit{
    private shopService = inject(ShopService)
    private dialogService = inject(MatDialog)
    products?: Pagination<Product>
    sortOptions = [
      {name: 'Alphabetical', value: 'name'},
      {name: 'Price: Low-High', value: 'priceAsc'},
      {name: 'Price: High-Low', value: 'priceDesc'}
    ]
    shopParams = new ShopParams()

  //request to backend to get product list
    ngOnInit(): void {
        this.initializeShop()
    }

    initializeShop() {
      this.shopService.getBrands()
      this.shopService.getTypes()
      this.getProducts()
    }

    //Sorting
    onSortChange(event: MatSelectionListChange){
      const selectedOption = event.options[0]
      if(selectedOption){
        this.shopParams.sort = selectedOption.value
        this.shopParams.pageNumber
        this.getProducts(); 
      }
    }


    getProducts(){
      this.shopService.getProducts(this.shopParams).subscribe({ //subscribe - uruchamia zapytanie HTTP i nasłuchuje na odpowiedź
            next: response => this.products = response, 
            error: error => console.log(error), 
        })
    }

    //Filtering
    //otwiera okno dialogowe z komponentem filtrów
    openFiltersDialog(){
      const dialogRef = this.dialogService.open(FiltersDialogComponent, {
        minWidth: '500px',
        data: {
          selectedBrands: this.shopParams.brands,
          selectedTypes: this.shopParams.types
        }
      })
      dialogRef.afterClosed().subscribe({
        next: result => {
          if(result){ //Jeśli dialog zwróci dane
            this.shopParams.brands = result.selectedBrands
            this.shopParams.types = result.selectedTypes
            this.shopParams.pageNumber = 1
            //apply filters
            this.getProducts()
          }
        }
      })
    }

    //Pagination
    handlePageEvent(event: PageEvent) {
      this.shopParams.pageNumber = event.pageIndex + 1;
      this.shopParams.pageSize = event.pageSize;
      this.getProducts();
    }

    //Searching
    onSearchChange(){
      this.shopParams.pageNumber = 1
      this.getProducts()
    }

}
