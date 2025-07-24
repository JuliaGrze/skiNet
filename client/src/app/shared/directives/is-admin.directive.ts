import { Directive, effect, inject, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../../core/services/account.service';

// Chcemy, żeby element był widoczny tylko dla admina.
// Jeśli user nie jest adminem – element znika z DOM.
@Directive({
  selector: '[appIsAdmin]' // Pozwala użyć *appIsAdmin w HTML
})
export class IsAdminDirective{
  private accountService = inject(AccountService)
  // ViewContainerRef to "miejsce" w DOM, w którym możemy wyświetlić lub ukryć szablon
  private viewContainerRef = inject(ViewContainerRef)
  // TemplateRef to referencja do kawałka HTML (np. <a>Admin</a>), który możemy wyświetlić
  private templateRef = inject(TemplateRef)

  constructor() {
    //reaguje na zmiany wszystkich sygnałów użytych wewnątrz funkcji przekazanej do effect
    // w isAdmin korzystamy z sygnału currentUser i reguje na to
    effect(() => {
      // Jeśli user jest adminem – pokazujemy szablon (czyli renderujemy element w DOM)
      if(this.accountService.isAdmin()){
        this.viewContainerRef.createEmbeddedView(this.templateRef)
      } else{
        // Jeśli NIE jest adminem – usuwamy ten element z DOM
        this.viewContainerRef.clear()
      }
    })
   }

}
