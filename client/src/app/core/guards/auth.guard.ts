import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { map, of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService)
  const router = inject(Router)

  // Jeśli currentUser nie jest null (czyli user zalogowany)
  if(accountService.currentUser()) {
    return of(true)
  }
  else{ // Jeśli user nie jest zalogowany, przekieruj na login
    return accountService.getAuthState().pipe(
      map(auth => {
        if(auth.isAuthenticated){
          return true
        } else{
          router.navigate(['/account/login'], {queryParams: {returnUrl: state.url}})
          //queryParams: { returnUrl: state.url } – dodajesz do adresu URL coś takiego jak: /account/login?returnUrl=/orders
          //Kiedy uzytkownik sie zalogowal zostanie przekirowany tam gdzie chcial byc
          return false
        }
      })
    )
  }

};
