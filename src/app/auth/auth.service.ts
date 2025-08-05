// Localização: src/app/core/services/auth/auth.service.ts

import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, throwError, timer, Subscription } from 'rxjs';
import { catchError, map, tap, switchMap, repeat, filter } from 'rxjs/operators';
import { environment } from '@environments/environment';

import { LoginRequestDto } from '@auth/models/login-request.model';
import { RegisterRequestDto } from '@auth/models/register-request.model';
import { LoginResponse } from '@auth/models/login-response.model'; // Sua interface de resposta de login
import { ApiResponse, isPreservedCollection } from '@models/common/api-response.model'; // Para o wrapper ApiResponse

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl;

  // Chaves para localStorage
  private readonly TOKEN_KEY = 'jwt_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly USER_ID_KEY = 'user_id';
  private readonly TOKEN_EXPIRATION_KEY = 'token_expiration';
  private readonly APELIDO_KEY = 'user_apelido';

  // BehaviorSubjects para o estado de autenticação
  private _isAuthenticated = new BehaviorSubject<boolean>(false);
  isAuthenticated$ = this._isAuthenticated.asObservable(); // Indica se o usuário está logado

  private _currentUserUid = new BehaviorSubject<string | null>(null);
  currentUserUid$ = this._currentUserUid.asObservable(); // O ID do usuário logado

  private _isAuthReady = new BehaviorSubject<boolean>(false);
  isAuthReady$ = this._isAuthReady.asObservable(); // Indica se o AuthService terminou sua inicialização

  private refreshSubscription: Subscription | null = null;

  constructor(private http: HttpClient, private router: Router) {
    console.log('[AuthService] Constructor: Iniciando AuthService.');
    this.initializeAuth();
  }

  /**
   * Inicializa o serviço de autenticação ao carregar o aplicativo.
   * Verifica a presença de tokens no localStorage e define o estado inicial.
   */
  private initializeAuth(): void {
    console.log('[AuthService] initializeAuth: Iniciando verificação de token e userId no storage.');
    const token = this.getStoredToken();
    const userId = this.getStoredUserId();
    const refreshToken = this.getStoredRefreshToken();
    const expiration = this.getStoredTokenExpiration();

    if (token && userId && refreshToken && expiration) {
      console.log('[AuthService] Dados de autenticação completos encontrados no storage. Usuário potencialmente logado.');
      this._isAuthenticated.next(true);
      this._currentUserUid.next(userId);
      this.scheduleTokenRefresh(); // Tenta agendar o refresh se os dados existirem
    } else {
      console.log('[AuthService] Nenhum dado de autenticação completo encontrado no storage. Usuário não logado.');
      this.clearAuthData(); // Garante que dados parciais sejam removidos
      this._isAuthenticated.next(false);
      this._currentUserUid.next(null);
    }
    this._isAuthReady.next(true); // O serviço está pronto para ser usado, mesmo que não logado
    console.log('[AuthService] isAuthReady$ definido como true após verificação inicial.');
  }

  /**
   * Realiza a requisição de login para o backend.
   * @param credentials Objeto com email e senha.
   * @returns Um Observable da resposta da API (ApiResponse<LoginResponse>).
   */
  login(credentials: LoginRequestDto): Observable<ApiResponse<LoginResponse>> { // <<-- AGORA ESPERA ApiResponse<LoginResponse> -->>
    console.log('[AuthService] login: Tentando fazer login para:', credentials.email);
    // Endpoint de login ajustado para /api/Account/login
    return this.http.post<ApiResponse<LoginResponse>>(`${this.apiUrl}/api/Account/login`, credentials).pipe(
      tap(response => {
        // Desempacota LoginResponse se estiver dentro de PreservedCollection
        const loginResponseData = isPreservedCollection<LoginResponse>(response.data) ? response.data.$values[0] : response.data;

        if (response.success && loginResponseData?.loginSucesso) { // <<-- VERIFICA success E loginSucesso do data -->>
          console.log('[AuthService] Login bem-sucedido. Armazenando token e informações do usuário.');
          
          this.storeAuthData(
            loginResponseData.token,
            loginResponseData.refreshToken,
            loginResponseData.expiration,
            loginResponseData.userId,
            loginResponseData.apelido
          );
          this._isAuthenticated.next(true);
          this._currentUserUid.next(loginResponseData.userId);
          this.scheduleTokenRefresh(); // Agenda o refresh
          this.router.navigate(['/dashboard']); // Redireciona
        } else {
          // Lógica de erro mais robusta para login
          let errorMessage = response.message || '';
          if (response.errors) {
            errorMessage += (errorMessage ? '; ' : '') + (Array.isArray(response.errors) ? response.errors.join('; ') : response.errors);
          }
          if (loginResponseData?.erros) {
              errorMessage += (errorMessage ? '; ' : '') + (typeof loginResponseData.erros === 'string' ? loginResponseData.erros : (Array.isArray(loginResponseData.erros) ? loginResponseData.erros.join('; ') : ''));
          }
          if (!errorMessage) {
            errorMessage = 'Credenciais inválidas.'; // Fallback se nenhuma mensagem específica for encontrada
          }

          console.warn('[AuthService] Login falhou na resposta da API:', errorMessage);
          this.clearAuthData();
          this._isAuthenticated.next(false);
          this._currentUserUid.next(null);
          throw new Error(errorMessage);
        }
      }),
      catchError(this.handleError)
    );
  }

  /**
   * Realiza o registro de um novo usuário.
   * @param userData Objeto com os dados do usuário para registro.
   * @returns Um Observable da resposta da API (ApiResponse<LoginResponse>).
   */
  register(userData: RegisterRequestDto): Observable<ApiResponse<LoginResponse>> { // <<-- AGORA ESPERA ApiResponse<LoginResponse> -->>
    console.log('[AuthService] register: Tentando registrar usuário:', userData.email);
    // Endpoint de registro ajustado para /api/Account/register
    return this.http.post<ApiResponse<LoginResponse>>(`${this.apiUrl}/api/Account/register`, userData).pipe(
      tap(response => {
        // Desempacota LoginResponse se estiver dentro de PreservedCollection
        const loginResponseData = isPreservedCollection<LoginResponse>(response.data) ? response.data.$values[0] : response.data;

        if (response.success && loginResponseData?.loginSucesso) { // <<-- VERIFICA success E loginSucesso do data -->>
          console.log('[AuthService] Registro bem-sucedido. Realizando login automático.');
          
          this.storeAuthData(
            loginResponseData.token,
            loginResponseData.refreshToken,
            loginResponseData.expiration,
            loginResponseData.userId,
            loginResponseData.apelido
          );
          this._isAuthenticated.next(true);
          this._currentUserUid.next(loginResponseData.userId);
          this.scheduleTokenRefresh();
          this.router.navigate(['/dashboard']); // Redireciona
        } else {
          // Lógica de erro mais robusta para registro
          let errorMessage = response.message || '';
          if (response.errors) {
            errorMessage += (errorMessage ? '; ' : '') + (Array.isArray(response.errors) ? response.errors.join('; ') : response.errors);
          }
          if (loginResponseData?.erros) {
              errorMessage += (errorMessage ? '; ' : '') + (typeof loginResponseData.erros === 'string' ? loginResponseData.erros : (Array.isArray(loginResponseData.erros) ? loginResponseData.erros.join('; ') : ''));
          }
          if (!errorMessage) {
            errorMessage = 'Erro no registro.'; // Fallback
          }

          console.warn('[AuthService] Registro falhou na resposta da API:', errorMessage);
          this.clearAuthData();
          this._isAuthenticated.next(false);
          this._currentUserUid.next(null);
          throw new Error(errorMessage);
        }
      }),
      catchError(this.handleError)
    );
  }

  /**
   * Realiza o refresh do token JWT.
   * @returns Um Observable da resposta do refresh (ApiResponse<LoginResponse>).
   */
  refreshToken(): Observable<ApiResponse<LoginResponse>> {
    const refreshToken = this.getStoredRefreshToken();
    const token = this.getStoredToken();
    const userId = this.getStoredUserId();

    if (!refreshToken || !token || !userId) {
      console.warn('[AuthService] refreshToken: Refresh token, JWT ou User ID ausente. Realizando logout.');
      this.logout();
      return throwError(() => new Error('Refresh token, JWT ou User ID ausente.'));
    }

    console.log('[AuthService] refreshToken: Tentando refresh do token.');
    const refreshRequest = {
      accessToken: token,
      refreshToken: refreshToken,
      userId: userId
    };

    // Endpoint de refresh ajustado para /api/Account/refresh-token
    return this.http.post<ApiResponse<LoginResponse>>(`${this.apiUrl}/api/Account/refresh-token`, refreshRequest).pipe(
      tap(response => {
        // Extrai o LoginResponse aninhado para verificação de sucesso e erros
        let actualLoginResponse: LoginResponse | null = null;
        if (response.data) {
            if (isPreservedCollection<LoginResponse>(response.data)) {
                actualLoginResponse = response.data.$values && response.data.$values.length > 0 ? response.data.$values[0] : null;
            } else {
                actualLoginResponse = response.data; // Já é LoginResponse
            }
        }

        if (response.success && actualLoginResponse?.loginSucesso) {
          console.log('[AuthService] Refresh de token bem-sucedido. Atualizando token e informações.');
          
          this.storeAuthData(
            actualLoginResponse.token,
            actualLoginResponse.refreshToken,
            actualLoginResponse.expiration,
            actualLoginResponse.userId,
            actualLoginResponse.apelido
          );
          this._isAuthenticated.next(true);
          this._currentUserUid.next(actualLoginResponse.userId);
          this.scheduleTokenRefresh(); // Agenda o próximo refresh
        } else {
          let errorMessage = response.message || '';
          if (response.errors) {
            errorMessage += (errorMessage ? '; ' : '') + (Array.isArray(response.errors) ? response.errors.join('; ') : response.errors);
          }

          if (actualLoginResponse?.erros) {
              errorMessage += (errorMessage ? '; ' : '') + (typeof actualLoginResponse.erros === 'string' ? actualLoginResponse.erros : (Array.isArray(actualLoginResponse.erros) ? actualLoginResponse.erros.join('; ') : ''));
          }
          
          if (!errorMessage) {
            errorMessage = 'Falha no refresh do token.';
          }

          console.warn('[AuthService] Refresh de token falhou na resposta da API. Realizando logout. Erro: ' + errorMessage);
          this.logout();
          throw new Error(errorMessage);
        }
      }),
      catchError(error => {
        console.error('[AuthService] Erro no refresh do token:', error);
        this.logout(); // Logout em caso de erro no refresh
        return this.handleError(error);
      })
    );
  }

  /**
   * Armazena os dados de autenticação no localStorage.
   */
  private storeAuthData(token: string, refreshToken: string, expiration: string, userId: string, apelido: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
    localStorage.setItem(this.TOKEN_EXPIRATION_KEY, expiration);
    localStorage.setItem(this.USER_ID_KEY, userId);
    localStorage.setItem(this.APELIDO_KEY, apelido);
    console.log('[AuthService] Dados de autenticação armazenados no localStorage.');
  }

  /**
   * Remove os dados de autenticação do localStorage.
   */
  logout(): void {
    console.log('[AuthService] logout: Realizando logout.');
    this.stopTokenRefreshTimer(); // Para qualquer timer de refresh ativo
    this.clearAuthData();
    this._isAuthenticated.next(false);
    this._currentUserUid.next(null);
    this.router.navigate(['/login']); // Redireciona para a tela de login
  }

  /**
   * Limpa todos os dados de autenticação do localStorage.
   */
  private clearAuthData(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_ID_KEY);
    localStorage.removeItem(this.TOKEN_EXPIRATION_KEY);
    localStorage.removeItem(this.APELIDO_KEY);
    console.log('[AuthService] Dados de autenticação removidos do localStorage.');
  }

  /**
   * Obtém o token JWT do localStorage.
   */
  getStoredToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  /**
   * Obtém o refresh token do localStorage.
   */
  getStoredRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  /**
   * Obtém o ID do usuário do localStorage.
   */
  getStoredUserId(): string | null {
    return localStorage.getItem(this.USER_ID_KEY);
  }

  /**
   * Obtém o apelido do usuário do localStorage.
   */
  getStoredApelido(): string | null {
    return localStorage.getItem(this.APELIDO_KEY);
  }

  /**
   * Obtém a data de expiração do token do localStorage.
   */
  getStoredTokenExpiration(): string | null {
    return localStorage.getItem(this.TOKEN_EXPIRATION_KEY);
  }

  /**
   * Agenda o refresh do token com base no tempo de expiração.
   */
  private scheduleTokenRefresh(): void {
    this.stopTokenRefreshTimer(); // Garante que não há timers duplicados

    const expiration = this.getStoredTokenExpiration();
    if (!expiration) {
      console.warn('[AuthService] scheduleTokenRefresh: Data de expiração do token não encontrada. Não agendando refresh.');
      return;
    }

    try {
      const expirationDate = new Date(expiration);
      const now = new Date();
      // Calcula o tempo restante até a expiração em milissegundos
      const expiresInMs = expirationDate.getTime() - now.getTime();

      // Agenda o refresh para 5 minutos antes da expiração real
      const refreshBeforeExpirationMs = 5 * 60 * 1000; // 5 minutos em milissegundos
      let delayMs = expiresInMs - refreshBeforeExpirationMs;

      // Se o token já expirou ou está muito próximo de expirar, tenta o refresh em 10 segundos
      if (delayMs <= 0) {
        delayMs = 10 * 1000;
        console.warn('[AuthService] Token expirado ou muito próximo de expirar. Tentando refresh imediato em 10 segundos.');
      }

      console.log(`[AuthService] Agendando refresh de token para daqui a ${Math.floor(delayMs / 1000)} segundos (${Math.floor(delayMs / 1000 / 60)} minutos).`);

      this.refreshSubscription = timer(delayMs).pipe(
        switchMap(() => this.refreshToken()),
        repeat() // Repete o processo após cada refresh bem-sucedido
      ).subscribe({
        next: () => console.log('[AuthService] Token atualizado com sucesso via refresh agendado.'),
        error: (err) => {
          console.error('[AuthService] Erro ao tentar refresh do token agendado:', err);
          this.logout(); // Força logout em caso de falha no refresh
        }
      });
    } catch (e) {
      console.error('[AuthService] Erro ao parsear data de expiração do token para agendamento:', e);
      this.logout(); // Força logout se a data de expiração for inválida
    }
  }

  /**
   * Para o timer de refresh do token.
   */
  private stopTokenRefreshTimer(): void {
    if (this.refreshSubscription) {
      this.refreshSubscription.unsubscribe();
      this.refreshSubscription = null;
      console.log('[AuthService] Refresh token timer parado.');
    }
  }

  /**
   * Manipulador de erros HTTP genérico.
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('[AuthService] Erro na requisição HTTP:', error);
    let errorMessage = 'Ocorreu um erro desconhecido.';
    if (error.error instanceof ErrorEvent) {
      // Erro do lado do cliente ou de rede
      errorMessage = `Erro: ${error.error.message}`;
    } else {
      // Erro do lado do servidor
      if (error.status === 401) {
        errorMessage = 'Não autorizado. Por favor, faça login novamente.';
      } else if (error.status === 403) {
        errorMessage = 'Acesso negado. Você não tem permissão para esta ação.';
      } else if (error.error && error.error.message) {
        // Tenta pegar a mensagem de erro do corpo da resposta, se disponível
        errorMessage = `Erro do servidor: ${error.error.message}`;
      } else if (error.message) {
        errorMessage = `Erro: ${error.message}`;
      } else {
        errorMessage = `Erro do servidor (Status: ${error.status}): ${error.statusText || ''}`;
      }
    }
    // Retorna um Observable com o erro para que o componente possa tratá-lo
    return throwError(() => new Error(errorMessage));
  }
}
