import { Component, inject } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';
import { MatDivider } from '@angular/material/divider'
import { MatListOption, MatSelectionList } from '@angular/material/list'
import { MatButton } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';

@Component({
	selector: 'app-filters-dialog',
	imports: [
		MatDivider,
		MatSelectionList,
		MatListOption,
		MatButton,
		FormsModule
	],
	templateUrl: './filters-dialog.component.html',
	styleUrl: './filters-dialog.component.scss'
})
export class FiltersDialogComponent {
	shopService = inject(ShopService) 

	//To referencja (uchwyt) do otwartego okna dialogowego
	private dialogRef = inject(MatDialogRef<FiltersDialogComponent>)
	// umożliwia odebranie danych przekazanych do dialogu przez opcję data w metodzie open(...)
	data = inject(MAT_DIALOG_DATA)

	selectedBrands: string[] = this.data.selectedBrands
	selectedTypes: string[] = this.data.selectedTypes

	//Gdy użytkownik kliknie „Zastosuj” w oknie dialogowym, 
	//zamyka okno i przekazuje wybrane marki i typy z powrotem do rodzica (afterClosed().subscribe(...) w rodzicu odbierze ten obiekt
	applyFilters(){
		//Zamykasz okno dialogowe i przekazujesz dane do rodzica.
		this.dialogRef.close({
			selectedBrands : this.selectedBrands,
			selectedTypes: this.selectedTypes
		})
	}
}
