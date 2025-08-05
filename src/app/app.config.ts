// Localização: src/app/app.config.ts

import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http'; // Importar provideHttpClient e withInterceptors
import { provideAnimations } from '@angular/platform-browser/animations'; // Importar provideAnimations

import { routes } from './app.routes';
import { jwtInterceptor } from './core/interceptors/jwt.interceptor'; // <<-- DESCOMENTADO E USANDO SEU INTERCEPTOR -->>

import { AuthService } from './auth/auth.service'; // <<-- Importar o AuthService para fornecê-lo

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideAnimations(), // Habilitar animações do Angular Material
    // <<-- REGISTRO CORRETO DO INTERCEPTOR FUNCIONAL -->>
    provideHttpClient(withInterceptors([jwtInterceptor])), 
    AuthService // <<-- Fornecer o AuthService no nível da aplicação
  ]
};
