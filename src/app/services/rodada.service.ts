// Localização: src/app/core/services/rodada/rodada.service.ts - V4+HANDLE

// Localização: src/app/services/rodada.service.ts

import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError} from 'rxjs';
import { catchError, map } from 'rxjs/operators'; // Re-importa 'map'
import { ApiResponse, PreservedCollection } from '@models/common/api-response.model';
import { RodadaDto } from '@models/rodada/rodada-dto.model';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RodadaService {
  // <<-- CORREÇÃO AQUI: Adiciona '/api/Rodada' -->>
  // Garante que environment.apiUrl não tenha barra no final, adiciona '/api', e depois '/Rodada'
  private apiUrl = `${environment.apiUrl.endsWith('/') ? environment.apiUrl.slice(0, -1) : environment.apiUrl}/api/Rodada`;

  constructor(private http: HttpClient) { }

  /**
   * Lista todas as rodadas com status 'Em Apostas' para um campeonato específico.
   * Este método chama o endpoint ListarEmApostas no backend.
   * @param campeonatoId O ID do campeonato.
   * @returns Um Observable de ApiResponse contendo uma lista de RodadaDto.
   */
  getRodadasEmAposta(campeonatoId: string): Observable<ApiResponse<RodadaDto[]>> {
    console.log(`RodadaService: getRodadasEmAposta chamado para Campeonato ID: ${campeonatoId}`);
    return this.http.get<ApiResponse<RodadaDto[]>>(`${this.apiUrl}/ListarEmApostas/${campeonatoId}`);
  }

  /**
   * Lista todas as rodadas com status 'Corrente' para um campeonato específico.
   * @param campeonatoId O ID do campeonato.
   * @returns Um Observable de ApiResponse contendo uma lista de RodadaDto.
   */
  getRodadasCorrentes(campeonatoId: string): Observable<ApiResponse<RodadaDto[]>> {
    console.log(`RodadaService: getRodadasCorrentes chamado para Campeonato ID: ${campeonatoId}`);
    return this.http.get<ApiResponse<RodadaDto[]>>(`${this.apiUrl}/ListarCorrentes/${campeonatoId}`);
  }

  /**
   * Lista todas as rodadas com status 'Finalizada' para um campeonato específico.
   * @param campeonatoId O ID do campeonato.
   * @returns Um Observable de ApiResponse contendo uma lista de RodadaDto.
   */
  getRodadasFinalizadas(campeonatoId: string): Observable<ApiResponse<RodadaDto[]>> {
    console.log(`RodadaService: getRodadasFinalizadas chamado para Campeonato ID: ${campeonatoId}`);
    return this.http.get<ApiResponse<RodadaDto[]>>(`${this.apiUrl}/ListarFinalizadas/${campeonatoId}`);
  }

  /**
   * Lista todas as rodadas (independentemente do status) para um campeonato específico.
   * @param campeonatoId O ID do campeonato.
   * @returns Um Observable de ApiResponse contendo uma lista de RodadaDto.
   */
  getRodadasTodas(campeonatoId: string): Observable<ApiResponse<RodadaDto[]>> {
    console.log(`RodadaService: getRodadasTodas chamado para Campeonato ID: ${campeonatoId}`);
    return this.http.get<ApiResponse<RodadaDto[]>>(`${this.apiUrl}/ListarTodas/${campeonatoId}`);
  }

  // Outros métodos do serviço de rodada (se existirem) devem ser mantidos aqui
  // Exemplo:
  obterRodadaPorId(id: string): Observable<ApiResponse<RodadaDto>> {
    return this.http.get<ApiResponse<RodadaDto>>(`${this.apiUrl}/${id}`);
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
}

/*
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators'; // Re-importa 'map'
import { ApiResponse, PreservedCollection, isPreservedCollection } from '@models/common/api-response.model';
import { RodadaDto } from '@models/rodada/rodada-dto.model';
import { environment } from '@environments/environment';
// REMOVIDO: import { handleApiResponse } from '@utils/api-response-handler';

@Injectable({
  providedIn: 'root'
})
export class RodadaService {
  private apiUrl = `${environment.apiUrl.endsWith('/') ? environment.apiUrl.slice(0, -1) : environment.apiUrl}/api/Rodada`;

  constructor(private http: HttpClient) { }

  /**
   * Lista todas as rodadas com status 'Em Apostas' para um campeonato específico.
   * @param campeonatoId O ID do campeonato.
   * @returns Um Observable de ApiResponse contendo uma lista de RodadaDto.
   
  getRodadasEmAposta(campeonatoId: string): Observable<ApiResponse<RodadaDto[]>> {
    console.log(`[RodadaService] getRodadasEmAposta chamado para Campeonato ID: ${campeonatoId}`);
    // Tipo de retorno esperado da API: ApiResponse<PreservedCollection<RodadaDto>>
    return this.http.get<ApiResponse<PreservedCollection<RodadaDto>>>(`${this.apiUrl}/ListarEmApostas/${campeonatoId}`).pipe(
      map(apiResponse => {
        const result: ApiResponse<RodadaDto[]> = { ...apiResponse, data: [] };
        if (apiResponse.success && apiResponse.data && isPreservedCollection<RodadaDto>(apiResponse.data)) {
          result.data = apiResponse.data.$values || [];
          console.log('[RodadaService] Rodadas em aposta recebidas como PreservedCollection. Extraindo valores.');
        } else if (apiResponse.success && Array.isArray(apiResponse.data)) {
          result.data = (apiResponse.data as RodadaDto[]);
          console.warn('[RodadaService] Dados recebidos diretamente como array (getRodadasEmAposta), sem PreservedCollection.');
        } else {
          console.warn('[RodadaService] Resposta da API sem dados ou formato inesperado para getRodadasEmAposta:', apiResponse);
        }
        return result;
      }),
      catchError(this.handleError)
    );
  }

  /**
   * Lista todas as rodadas com status 'Corrente' para um campeonato específico.
   * @param campeonatoId O ID do campeonato.
   * @returns Um Observable de ApiResponse contendo uma lista de RodadaDto.
   
  getRodadasCorrentes(campeonatoId: string): Observable<ApiResponse<RodadaDto[]>> {
    console.log(`[RodadaService] getRodadasCorrentes chamado para Campeonato ID: ${campeonatoId}`);
    // Tipo de retorno esperado da API: ApiResponse<PreservedCollection<RodadaDto>>
    return this.http.get<ApiResponse<PreservedCollection<RodadaDto>>>(`${this.apiUrl}/ListarCorrentes/${campeonatoId}`).pipe(
      map(apiResponse => {
        const result: ApiResponse<RodadaDto[]> = { ...apiResponse, data: [] };
        if (apiResponse.success && apiResponse.data && isPreservedCollection<RodadaDto>(apiResponse.data)) {
          result.data = apiResponse.data.$values || [];
          console.log('[RodadaService] Rodadas correntes recebidas como PreservedCollection. Extraindo valores.');
        } else if (apiResponse.success && Array.isArray(apiResponse.data)) {
          result.data = (apiResponse.data as RodadaDto[]);
          console.warn('[RodadaService] Dados recebidos diretamente como array (getRodadasCorrentes), sem PreservedCollection.');
        } else {
          console.warn('[RodadaService] Resposta da API sem dados ou formato inesperado para getRodadasCorrentes:', apiResponse);
        }
        return result;
      }),
      catchError(this.handleError)
    );
  }

  /**
   * Lista todas as rodadas com status 'Finalizada' para um campeonato específico.
   * @param campeonatoId O ID do campeonato.
   * @returns Um Observable de ApiResponse contendo uma lista de RodadaDto.
   
  getRodadasFinalizadas(campeonatoId: string): Observable<ApiResponse<RodadaDto[]>> {
    console.log(`[RodadaService] getRodadasFinalizadas chamado para Campeonato ID: ${campeonatoId}`);
    // Tipo de retorno esperado da API: ApiResponse<PreservedCollection<RodadaDto>>
    return this.http.get<ApiResponse<PreservedCollection<RodadaDto>>>(`${this.apiUrl}/ListarFinalizadas/${campeonatoId}`).pipe(
      map(apiResponse => {
        const result: ApiResponse<RodadaDto[]> = { ...apiResponse, data: [] };
        if (apiResponse.success && apiResponse.data && isPreservedCollection<RodadaDto>(apiResponse.data)) {
          result.data = apiResponse.data.$values || [];
          console.log('[RodadaService] Rodadas finalizadas recebidas como PreservedCollection. Extraindo valores.');
        } else if (apiResponse.success && Array.isArray(apiResponse.data)) {
          result.data = (apiResponse.data as RodadaDto[]);
          console.warn('[RodadaService] Dados recebidos diretamente como array (getRodadasFinalizadas), sem PreservedCollection.');
        } else {
          console.warn('[RodadaService] Resposta da API sem dados ou formato inesperado para getRodadasFinalizadas:', apiResponse);
        }
        return result;
      }),
      catchError(this.handleError)
    );
  }

  /**
   * Lista todas as rodadas (independentemente do status) para um campeonato específico.
   * @param campeonatoId O ID do campeonato.
   * @returns Um Observable de ApiResponse contendo uma lista de RodadaDto.
   
  getRodadasTodas(campeonatoId: string): Observable<ApiResponse<RodadaDto[]>> {
    console.log(`[RodadaService] getRodadasTodas chamado para Campeonato ID: ${campeonatoId}`);
    // Tipo de retorno esperado da API: ApiResponse<PreservedCollection<RodadaDto>>
    return this.http.get<ApiResponse<PreservedCollection<RodadaDto>>>(`${this.apiUrl}/ListarTodas/${campeonatoId}`).pipe(
      map(apiResponse => {
        const result: ApiResponse<RodadaDto[]> = { ...apiResponse, data: [] };
        if (apiResponse.success && apiResponse.data && isPreservedCollection<RodadaDto>(apiResponse.data)) {
          result.data = apiResponse.data.$values || [];
          console.log('[RodadaService] Todas as rodadas recebidas como PreservedCollection. Extraindo valores.');
        } else if (apiResponse.success && Array.isArray(apiResponse.data)) {
          result.data = (apiResponse.data as RodadaDto[]);
          console.warn('[RodadaService] Dados recebidos diretamente como array (getRodadasTodas), sem PreservedCollection.');
        } else {
          console.warn('[RodadaService] Resposta da API sem dados ou formato inesperado para getRodadasTodas:', apiResponse);
        }
        return result;
      }),
      catchError(this.handleError)
    );
  }

  /**
   * Obtém uma rodada específica pelo ID.
   * @param id O ID da rodada.
   * @returns Um Observable de ApiResponse contendo o RodadaDto.
   
  obterRodadaPorId(id: string): Observable<ApiResponse<RodadaDto>> {
    console.log(`[RodadaService] obterRodadaPorId chamado para Rodada ID: ${id}`);
    // O backend retorna ApiResponse<RodadaDto> diretamente, sem PreservedCollection aninhada para um único item.
    // Baseado na sua imagem anterior, o endpoint /api/Rodada/{id} retorna diretamente RodadaDto.
    return this.http.get<ApiResponse<RodadaDto>>(`${this.apiUrl}/${id}`).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Manipulador de erros HTTP genérico para o RodadaService.
   
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
}

*/