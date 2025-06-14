import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';

@Component({
  selector: 'app-text-input',
  imports: [
    ReactiveFormsModule,
    MatFormField,
    MatInput,
    MatError,
    MatLabel
  ],
  templateUrl: './text-input.component.html',
  styleUrl: './text-input.component.scss'
})
// interfejs Angulara pozwalający, by Twój własny komponent mógł działać jak zwykły <input> w ReactiveForms
export class TextInputComponent implements ControlValueAccessor{
  @Input() label = ''
  @Input() type = 'text'
  
  // daje dostęp do FormControl, do walidacji, błędów itd.
  //informuje Angular Dependency Injection, żeby szukać NgControl tylko w tym komponencie
  constructor(@Self() public controlDir: NgControl){
    //To jest najważniejsza linijka, żeby Twój komponent naprawdę "współpracował" z Angular Forms
    controlDir.valueAccessor = this
  }

  //Ułatwia dostęp do FormControl (czyli tego, co jest powiązane przez formControlName) wewnątrz customowego inputa
  get control(){
    return this.controlDir.control as FormControl
  }

  //Metody ktore musze zaimplementowac z ControlValueAccessor
  writeValue(obj: any): void {}
  registerOnChange(fn: any): void {}
  registerOnTouched(fn: any): void {}
  setDisabledState?(isDisabled: boolean): void {}
}
