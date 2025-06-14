import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { delay, finalize } from 'rxjs';
import { BusyService } from '../../core/services/busy.service';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {

  const busyService = inject(BusyService)

  //start loading
  busyService.busy()

  return next(req).pipe(
    delay(500),
    finalize(() => busyService.idle()) // finalize zawsze się wywoła – na błąd i na sukces
  )
};
