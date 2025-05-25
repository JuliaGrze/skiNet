import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  //eventCoalescing: true – optymalizacja: łączy zdarzenia razem, by zmniejszyć ilość wykrytych zmian i przyspieszyć renderowanie
  providers: [provideZoneChangeDetection({ eventCoalescing: true }), 
    provideRouter(routes)] //Wstrzykuje routing do aplikacji
    //routes to tablica tras zdefiniowana np. w app.routes.ts
};
