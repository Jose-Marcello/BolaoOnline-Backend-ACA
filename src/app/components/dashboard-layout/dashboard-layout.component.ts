// Localização: src/app/components/dashboard-layout/dashboard-layout.component.ts

import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { Observable, Subscription, combineLatest, of } from 'rxjs';
import { filter, switchMap, tap, map, catchError} from 'rxjs/operators';


import { AuthService } from '@auth/auth.service';
import { ApostadorService } from '@services/apostador.service';
import { ApostadorDto } from '@models/apostador/apostador-dto.model';
import { ApiResponse, isPreservedCollection, PreservedCollection } from '@models/common/api-response.model'; // Importa isPreservedCollection e PreservedCollection

@Component({
  selector: 'app-dashboard-layout',
  standalone: true,
  imports: [
    CommonModule,
    MatSidenavModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatListModule,
    MatMenuModule, // Adicionar MatMenuModule
    RouterOutlet,
    RouterLink
  ],
  templateUrl: './dashboard-layout.component.html',
  styleUrls: ['./dashboard-layout.component.scss']
})
export class DashboardLayoutComponent implements OnInit, OnDestroy {
  isSidenavOpen = false;
  isAuthenticated$: Observable<boolean>;
  currentUserUid$: Observable<string | null>;
  loggedInUser$: Observable<ApostadorDto | null>; // <<-- EXPOR APOSTADOR DTO COMPLETO -->>

  private authSubscription: Subscription = new Subscription();

  constructor(
    private authService: AuthService,
    private apostadorService: ApostadorService,
    private router: Router
  ) {
    console.log('[DashboardLayoutComponent] Constructor: Iniciado.');
    this.isAuthenticated$ = this.authService.isAuthenticated$;
    this.currentUserUid$ = this.authService.currentUserUid$;
    
    // Combina isAuthReady e currentUserUid para carregar os dados do apostador
    this.loggedInUser$ = combineLatest([
      this.authService.isAuthReady$.pipe(filter(isReady => isReady)),
      this.authService.currentUserUid$.pipe(filter(uid => !!uid))
    ]).pipe(
      tap(([isAuthReady, userId]) => {
        console.log(`[DashboardLayoutComponent] Auth Ready: ${isAuthReady}, User ID: ${userId}`);
        if (isAuthReady && userId) {
          console.log(`[DashboardLayoutComponent] Auth Ready e userId presente (${userId}). Carregando dados do apostador.`);
        }
      }),
      switchMap(([isAuthReady, userId]) => {
        if (isAuthReady && userId) {
          return this.apostadorService.getDadosApostador().pipe(
            map(response => {
              if (response.success && response.data) {
                const apostadorData = Array.isArray(response.data) && response.data.length > 0 ? response.data[0] : response.data;
                console.log('[DashboardLayoutComponent] Dados do apostador carregados:', apostadorData?.apelido);
                return apostadorData;
              }
              console.warn('[DashboardLayoutComponent] Falha ao carregar dados do apostador:', response.message || 'Dados ausentes.');
              return null;
            }),
            catchError(error => {
              console.error('[DashboardLayoutComponent] Erro no pipe de getDadosApostador:', error);
              return of(null);
            })
          );
        }
        return of(null);
      })
    );
  }

  ngOnInit(): void {
    console.log('[DashboardLayoutComponent] ngOnInit: Iniciado.');
  }

  ngOnDestroy(): void {
    console.log('[DashboardLayoutComponent] ngOnDestroy: Desinscrevendo do authSubscription.');
    this.authSubscription.unsubscribe();
  }

  toggleSidenav(): void {
    this.isSidenavOpen = !this.isSidenavOpen;
  }

  logout(): void {
    console.log('[DashboardLayoutComponent] Realizando logout.');
    this.authService.logout();
  }
}
