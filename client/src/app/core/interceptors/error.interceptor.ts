import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { SnackbarService } from '../../core/services/snackbar.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router)
  const snackbar = inject(SnackbarService)

  //pipe() to metoda RxJS używana na Observable
  //Pozwala przetwarzać, modyfikować, filtrować lub obsługiwać błędy na tych strumieniach
  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {

      //handling various errors

      //BAD REQUEST = 400
      if(err.status == 400){
        //errors - dictionary bledow i wysyepuje Jeśli wyslane jest zadanie z niepoprawnymi danymi do kontrolera z [ApiController] i .net zwraca 400 i taki slownik
        if(err.error.errors){ 
          const modelStateErrors = []
          for(const key in err.error.errors){
            if(err.error.errors[key]){
              //Dla każdego klucza (nazwa pola), dodajesz tablicę komunikatów do modelStateErrors
              modelStateErrors.push(err.error.errors[key])
            }
          }

          //modelStateErrors to tablica tablic (string[][]), więc .flat() robi z tego jedną tablicę string[]
          //throw (bez throwError) rzuca błąd – ale to przerwie wykonywanie funkcji w tym miejscu
          throw modelStateErrors.flat()
        } else{
          snackbar.error(err.error.title || err.error)
        }
      }

      if(err.status == 401){
        snackbar.error(err.error.title || err.error)
      }

      //NOT FOUND = 404
      if(err.status == 404){
        router.navigateByUrl('/not-found')
      }

      //SERVER ERROR = 500
      if(err.status == 500){
        //NavigationExtras to specjalny obiekt Angulara dla nawigacji
        //stawiasz w nim pole state, gdzie możesz przekazać dowolny obiekt (tu: error)
        const navigationExtras: NavigationExtras = {state: {error: err.error}}
        router.navigateByUrl('/server-error', navigationExtras)
      }

      // Zawsze przekaż błąd dalej, jeśli chcesz obsłużyć go lokalnie też
      return throwError(() => err)
    })
  );

};
