import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { delay, finalize, identity } from 'rxjs';
import { BusyService } from '../../core/services/busy.service';
import { environment } from '../../../environments/environment';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {

  const busyService = inject(BusyService)

  //start loading
  busyService.busy()

  return next(req).pipe(
    //Jeśli production (czyli środowisko produkcyjne), używa funkcji identity – czyli nie wprowadza opóźnienia
    //Jeśli nie production (czyli development), wtedy delay(500)
    (environment.producttion ? identity : delay(500)),
    finalize(() => busyService.idle()) // finalize zawsze się wywoła – na błąd i na sukces
  )
};
