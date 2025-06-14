import { HttpInterceptorFn } from '@angular/common/http';

//Dodajemy ciasteczka przy requestahc automatycznie
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const clonedRequest = req.clone({
    withCredentials: true
  })
  return next(clonedRequest);
};
