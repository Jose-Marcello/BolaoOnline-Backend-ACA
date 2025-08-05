// Localização: src/app/services/apostador-campeonato.service.ts - V4+HANDLE ERROR
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { ApiResponse } from '@models/common/api-response.model'; // Ajustado para path relativo
import { ApostadorCampeonatoDto } from '@models/apostador-campeonato/apostador-campeonato-dto.model'; // Ajustado para path relativo e nome padrão
import { environment } from '@environments/environment'; // Ajustado para path relativo

@Injectable({
  providedIn: 'root'
})
export class ApostadorCampeonatoService {
  private apiUrl = `${environment.apiUrl}/ApostadorCampeonato`; // Ajuste a URL base conforme seu backend

  constructor(private http: HttpClient) { }

  /**
   * Obtém a associação Apostador-Campeonato para um usuário e campeonato específicos.
   * @param userId ID do usuário.
   * @param campeonatoId ID do campeonato.
   * @returns Um Observable de ApiResponse contendo o ApostadorCampeonatoDto.
   */
  obterApostadorCampeonatoPorUsuarioECampeonato(userId: string, campeonatoId: string): Observable<ApiResponse<ApostadorCampeonatoDto>> {
    return this.http.get<ApiResponse<ApostadorCampeonatoDto>>(`${this.apiUrl}/ObterPorUsuarioECampeonato`, {
      params: {
        usuarioId: userId,
        campeonatoId: campeonatoId
      }
    });
  }

  /**
   * Manipulador de erros HTTP genérico para o RodadaService.
  */ 
  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('[RodadaService] Erro na requisição HTTP:', error);
    let errorMessage = 'Ocorreu um erro desconhecido ao buscar dados da rodada.';
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

  // Você pode adicionar outros métodos aqui, como aderir a um campeonato, etc.
}
