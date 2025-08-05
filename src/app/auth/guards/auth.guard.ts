
// Localização: src/app/core/guards/auth.guard.ts (usando V5)

import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '@auth/auth.service'; // Importa o AuthService
import { map, take } from 'rxjs/operators';
import { Observable } from 'rxjs';

/**
 * Guard de rota para proteger rotas que exigem autenticação.
*/
export const AuthGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
): Observable<boolean> => {
  const authService = inject(AuthService);
  const router = inject(Router);

  console.log('[AuthGuard] Verificando acesso para a rota:', state.url);

  // Usa take(1) para pegar o valor atual e completar o observable, evitando loops
  // Usa map para transformar o valor de isAuthenticated$ em um booleano para o guard
  return authService.isAuthenticated$.pipe(
    take(1), // Pega o valor atual e completa o Observable
    map(isAuthenticated => {
      if (isAuthenticated) {
        console.log('[AuthGuard] Usuário autenticado. Acesso permitido.');
        return true;
      } else {
        console.warn('[AuthGuard] Usuário NÃO autenticado. Redirecionando para /login.');
        router.navigate(['/login']);
        return false;
      }
    })
  );
};

//V4
// Localização: src/app/auth/guards/auth.guard.ts
/*
import { inject } from '@angular/core';
//import { CanActivateFn, Router } from '@angular/router';
import { CanActivateFn, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '@auth/auth.service'; // Ajuste o caminho conforme necessário
import { map, take } from 'rxjs/operators';

export const AuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.loggedIn$.pipe(
    take(1), // Pega o valor atual e completa
    map(isLoggedIn => {
      // Usar os novos métodos públicos do AuthService para obter os valores armazenados
      const token = authService.getStoredToken();
      const userId = authService.getStoredUserId();

      if (isLoggedIn && token && userId) {
        return true;
      } else {
        router.navigate(['/login']);
        return false;
      }
    })
  );
};
*/