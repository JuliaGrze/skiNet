import { ApplicationConfig, inject, provideAppInitializer, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { loadingInterceptor } from './core/interceptors/loading.interceptor';
import { InitService } from './core/services/init.service';
import { firstValueFrom, lastValueFrom } from 'rxjs';
import { authInterceptor } from './core/interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  //eventCoalescing: true – optymalizacja: łączy zdarzenia razem, by zmniejszyć ilość wykrytych zmian i przyspieszyć renderowanie
  providers: [provideZoneChangeDetection({ eventCoalescing: true }), 
    provideRouter(routes), //Wstrzykuje routing do aplikacji
    //routes to tablica tras zdefiniowana np. w app.routes.ts

    // rejestruje wszystkie potrzebne zależności Angulara, aby móc używać HttpClient w aplikacji.
    provideHttpClient(withInterceptors([errorInterceptor, loadingInterceptor, authInterceptor])),

    InitService,
    provideAppInitializer(() => {
      const initService = inject(InitService);
      //lastValueFrom(...) zamienia Observable na Promise, żeby Angular mógł "poczekać" na zakończenie tej inicjalizacji
      //.finally(...) — po zakończeniu pobierania koszyka (sukces lub błąd) usuwa splash screen (jeśli taki element istnieje
      return lastValueFrom(initService.init()).finally(() => {
        //Splash (ang. splash screen) to ekran powitalny, który widzi użytkownik, zanim Twoja aplikacja (np. Angular, mobilna, desktopowa) się w pełni załaduje
        const splash = document.getElementById('initial-splash')
        if (splash) splash.remove()
      });
    }),
  ] 
};
