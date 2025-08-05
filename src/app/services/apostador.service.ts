// Localização: src/app/core/services/apostador.service.ts

import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap, filter, take, map } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { ApiResponse, PreservedCollection, isPreservedCollection } from '@models/common/api-response.model';
import { ApostadorDto } from '@models/apostador/apostador-dto.model';
import { AuthService } from '@auth/auth.service'; // Importa o AuthService

@Injectable({
  providedIn: 'root'
})
export class ApostadorService {
  private apiUrl = `${environment.apiUrl.endsWith('/') ? environment.apiUrl.slice(0, -1) : environment.apiUrl}/api/Apostador`;

  constructor(private http: HttpClient, private authService: AuthService) { } // Injeta AuthService

  /**
   * Obtém os dados do apostador logado.
   * Espera até que o AuthService esteja pronto e tenha um userId válido.
   * @returns Um Observable de ApiResponse contendo o ApostadorDto.
   */
  getDadosApostador(): Observable<ApiResponse<ApostadorDto>> {
    console.log('[ApostadorService] getDadosApostador: Aguardando autenticação e userId.');
    return this.authService.isAuthReady$.pipe(
      filter(isReady => isReady), // Espera até que o AuthService esteja pronto
      take(1), // Pega apenas a primeira emissão 'true'
      //switchMap(() => this.authService.currentUserUID$), // Agora pega o UID
      filter(userId => {
        if (!userId) {
          console.error('[ApostadorService] getDadosApostador: userId é nulo após isAuthReady. Não é possível buscar dados do apostador.');
          return false;
        }
        return true;
      }),
      take(1), // Pega o primeiro userId válido
      switchMap(userId => {
        console.log(`[ApostadorService] getDadosApostador: userId obtido: ${userId}. Chamando API.`);
        // A API espera o userId como parte da URL ou query param.
        // Assumindo que o endpoint é /api/Apostador/{userId} ou similar.
        // Se for um POST ou GET sem ID na URL, você pode passar o userId no corpo ou como query param.
        // Para este exemplo, vou assumir que o backend pode inferir o apostador logado pelo token JWT.
        // Se o backend precisar do userId explicitamente, você pode ajustar a URL ou params.
        return this.http.get<ApiResponse<any>>(`${this.apiUrl}/Dados`).pipe( // Assumindo que /Dados retorna o apostador logado
          map(apiResponse => {
            let extractedData: ApostadorDto | null = null;
            if (apiResponse.success && apiResponse.data) {
              // Verifica se a resposta é uma PreservedCollection e extrai o primeiro valor
              if (isPreservedCollection<ApostadorDto>(apiResponse.data)) {
                extractedData = (apiResponse.data.$values && apiResponse.data.$values.length > 0) ? apiResponse.data.$values[0] : null;
                console.log('[ApostadorService] Dados do apostador recebidos como PreservedCollection. Extraindo o primeiro valor.');
              } else {
                extractedData = apiResponse.data as ApostadorDto;
                console.log('[ApostadorService] Dados do apostador recebidos diretamente.');
              }
            }
            return { ...apiResponse, data: extractedData } as ApiResponse<ApostadorDto>;
          }),
          catchError(this.handleError)
        );
      })
    );
  }

  /**
   * Manipulador de erros HTTP genérico para o ApostadorService.
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('[ApostadorService] Erro na requisição HTTP:', error);
    let errorMessage = 'Ocorreu um erro desconhecido ao buscar dados do apostador.';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Erro do cliente: ${error.error.message}`;
    } else {
      if (error.status === 404) {
        errorMessage = 'Recurso não encontrado.';
      } else if (error.status === 401) {
        errorMessage = 'Não autorizado. Faça login novamente.';
      } else if (error.status === 403) {
        errorMessage = 'Acesso negado. Você não tem permissão para esta ação.';
      } else if (error.error && error.error.message) {
        errorMessage = `Erro do servidor: ${error.error.message}`;
      } else if (error.message) {
        errorMessage = `Erro: ${error.message}`;
      } else {
        errorMessage = `Erro do servidor (Status: ${error.status}): ${error.statusText || ''}`;
      }
    }
    return throwError(() => new Error(errorMessage));
  }
}
