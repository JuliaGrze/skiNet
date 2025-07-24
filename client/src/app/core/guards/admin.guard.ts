import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { SnackbarService } from '../services/snackbar.service';

// Chroni dostęp do trasy tylko dla admina
export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService)
  const router = inject(Router)
  const snack = inject(SnackbarService)

  // Sprawdzamy, czy użytkownik jest adminem
  if(accountService.isAdmin()){
    // Jeśli jest adminem — pozwól wejść na trasę
    return true;
  } else {
    // Jeśli nie — wyświetl komunikat (np. snack bar z błędem)
    snack.error('Nope')
    // Przekieruj użytkownika na stronę /shop (albo dowolną inną)
    router.navigateByUrl('/shop')
    // Zablokuj dostęp do chronionej trasy
    return false;
  }
};
