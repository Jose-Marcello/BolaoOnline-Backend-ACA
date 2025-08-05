// Localização: src/app/app.routes.ts

import { Routes } from '@angular/router';
import { AuthGuard } from '@auth/guards/auth.guard';
import { LoginComponent } from '@auth/components/login/login';
import { RegisterComponent } from '@auth/components/register/register';
import { DashboardLayoutComponent } from '@components/dashboard-layout/dashboard-layout.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  {
    path: '',
    component: DashboardLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('@components/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'apostar/:campeonatoId',
        loadComponent: () => import('@components/aposta-rodada/aposta-rodada-form.component').then(m => m.ApostarRodadaFormComponent)
      },
      {
        path: 'apostar/:campeonatoId/:rodadaId',
        loadComponent: () => import('@components/aposta-rodada/aposta-rodada-form.component').then(m => m.ApostarRodadaFormComponent)
      },
      {
        path: 'apostar/:campeonatoId/:rodadaId/:apostaRodadaId',
        loadComponent: () => import('@components/aposta-rodada/aposta-rodada-form.component').then(m => m.ApostarRodadaFormComponent)
      },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    ]
  },

  { path: '**', redirectTo: 'login' }
];

/*
import { Routes } from '@angular/router';
import { AuthGuard } from '@auth/guards/auth.guard';
import { DashboardLayoutComponent } from '@components/dashboard-layout/dashboard-layout.component';
import { LoginComponent } from '@auth/components/login/login';
import { DashboardComponent } from '@components/dashboard/dashboard.component';
//import { MeusPalpitesComponent } from '@components/meus-palpites/meus-palpites.component';
//import { RankingComponent } from '@components/ranking/ranking.component';
//import { FinanceiroComponent } from '@components/financeiro/financeiro.component';
//import { ConfiguracoesComponent } from '@components/configuracoes/configuracoes.component';
import { ApostarRodadaFormComponent } from '@components/aposta-rodada/aposta-rodada-form.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: DashboardLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: 'dashboard', component: DashboardComponent },
      //{ path: 'meus-palpites', component: MeusPalpitesComponent },
      //{ path: 'ranking', component: RankingComponent },
      //{ path: 'financeiro', component: FinanceiroComponent },
      //{ path: 'configuracoes', component: ConfiguracoesComponent },
      // <<-- ROTAS PARA APOSTA DE RODADA (ORDEM AJUSTADA) -->>
      // A rota mais específica deve vir primeiro
      { 
        path: 'campeonato/:campeonatoId/apostar-rodada/:rodadaId', 
        component: ApostarRodadaFormComponent 
      },
      // A rota mais genérica (sem rodadaId) deve vir depois
      { 
        path: 'campeonato/:campeonatoId/apostar-rodada', 
        component: ApostarRodadaFormComponent 
      },
    ]
  },
  { path: '**', redirectTo: 'login' } // Rota curinga para redirecionar para o login
];
*/