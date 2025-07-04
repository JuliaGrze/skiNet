import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { OrderService } from '../services/order.service';

 //Guard sprawdza flagę `orderComplete`, która jest ustawiana
 // na true tylko po prawidłowym zakończeniu płatności 
export const orderCompleteGuard: CanActivateFn = (route, state) => {
  const orderService = inject(OrderService)
  const router = inject(Router)

  // Jeśli flaga jest true, wpuszczamy na stronę
  if(orderService.orderComplete){
    return true
  }else{ // Jeśli NIE, przekieruj na /shop
    router.navigateByUrl('/shop')
    return false
  }
};
