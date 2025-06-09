import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { errorInterceptor } from './coere/interceptors/error.interceptor';
import { loadingInterceptor } from './coere/interceptors/loading.interceptor';

export const appConfig: ApplicationConfig = {
  //eventCoalescing: true – optymalizacja: łączy zdarzenia razem, by zmniejszyć ilość wykrytych zmian i przyspieszyć renderowanie
  providers: [provideZoneChangeDetection({ eventCoalescing: true }), 
    provideRouter(routes), //Wstrzykuje routing do aplikacji
    //routes to tablica tras zdefiniowana np. w app.routes.ts

    // rejestruje wszystkie potrzebne zależności Angulara, aby móc używać HttpClient w aplikacji.
    provideHttpClient(withInterceptors([errorInterceptor, loadingInterceptor]))
  ] 
};
