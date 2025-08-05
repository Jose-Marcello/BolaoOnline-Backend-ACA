

// Localização: src/app/services/aposta.service.ts - V4+ HANDLE
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { environment } from '@environments/environment';
import { ApiResponse, PreservedCollection } from '@models/common/api-response.model';
import { SalvarApostaRequestDto } from '@models/aposta/salvar-aposta-request-dto.model';
import { ApostaRodadaDto } from '@models/aposta/aposta-rodada-dto.model';
import { ApostaJogoVisualizacaoDto } from '@models/aposta/aposta-jogo-visualizacao-dto.model'; // Importar ApostaJogoVisualizacaoDto

@Injectable({
  providedIn: 'root'
})
export class ApostaService {
  // Ajuste o caminho base da sua API de apostas para o controlador ApostaRodada
  private apiUrlApostaRodada = `${environment.apiUrl}api/ApostaRodada`; 
  // <<-- CORRIGIDO: URL para o controlador correto -->>
  private apiUrlAposta = `${environment.apiUrl}api/ApostadorCampeonato`; 

  constructor(private http: HttpClient) { }

  /**
   * Obtém as apostas de um usuário para uma rodada específica.
   * @param rodadaId O ID da rodada.
   * @param apostadorCampeonatoId O ID da associação apostador-campeonato.
   * @returns Um Observable com a resposta da API contendo as apostas da rodada do usuário.
   */
  getApostasPorRodadaEUsuario(rodadaId: string, apostadorCampeonatoId: string): Observable<ApiResponse<PreservedCollection<ApostaRodadaDto>>> {
    return this.http.get<ApiResponse<PreservedCollection<ApostaRodadaDto>>>(`${this.apiUrlApostaRodada}/ListarPorRodadaEUsuario?rodadaId=${rodadaId}&apostadorCampeonatoId=${apostadorCampeonatoId}`);
  }

  /**
   * Obtém os jogos de uma rodada com os palpites existentes para edição.
   * @param rodadaId O ID da rodada.
   * @param apostadorCampeonatoId O ID da associação apostador-campeonato.
   * @returns Um Observable com a resposta da API contendo uma coleção de ApostaJogoVisualizacaoDto.
   */
  getApostasParaEdicao(rodadaId: string, apostadorCampeonatoId: string): Observable<ApiResponse<PreservedCollection<ApostaJogoVisualizacaoDto>>> {
    return this.http.get<ApiResponse<PreservedCollection<ApostaJogoVisualizacaoDto>>>(`${this.apiUrlApostaRodada}/ParaEdicao?rodadaId=${rodadaId}&apostadorCampeamentoId=${apostadorCampeonatoId}`);
  }

  /**
   * Salva ou atualiza as apostas de uma rodada.
   * <<-- CORRIGIDO: Rota para o endpoint correto -->>
   * @param request O DTO com os dados da aposta a serem salvos.
   * @returns Um Observable com a resposta da API.
   */
  salvarApostas(request: SalvarApostaRequestDto): Observable<ApiResponse<ApostaRodadaDto>> {
    return this.http.post<ApiResponse<ApostaRodadaDto>>(`${this.apiUrlAposta}/SalvarApostas`, request);
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



// Localização: src/app/core/services/aposta/aposta.service.ts
/*
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators'; // Re-importa 'map'
import { environment } from '@environments/environment';
import { ApiResponse, PreservedCollection, isPreservedCollection } from '@models/common/api-response.model';
import { SalvarApostaRequestDto } from '@models/aposta/salvar-aposta-request-dto.model';
import { ApostaRodadaDto } from '@models/aposta/aposta-rodada-dto.model';
import { ApostaJogoVisualizacaoDto } from '@models/aposta/aposta-jogo-visualizacao-dto.model';
// REMOVIDO: import { handleApiResponse } from '@utils/api-response-handler';

@Injectable({
  providedIn: 'root'
})
export class ApostaService {
  private apiUrlApostaRodada = `${environment.apiUrl.endsWith('/') ? environment.apiUrl.slice(0, -1) : environment.apiUrl}/api/ApostaRodada`;
  private apiUrlAposta = `${environment.apiUrl.endsWith('/') ? environment.apiUrl.slice(0, -1) : environment.apiUrl}/api/ApostadorCampeonato`;

  constructor(private http: HttpClient) { }

  /**
   * Obtém as apostas de um usuário para uma rodada específica.
   * @param rodadaId O ID da rodada.
   * @param apostadorCampeonatoId O ID da associação apostador-campeonato (com 'o').
   * @returns Um Observable com a resposta da API contendo as apostas da rodada do usuário.
   
  getApostasPorRodadaEUsuario(rodadaId: string, apostadorCampeonatoId: string): Observable<ApiResponse<ApostaRodadaDto[]>> {
    const params = new HttpParams()
      .set('rodadaId', rodadaId)
      .set('apostadorCampeonatoId', apostadorCampeonatoId); // Backend espera 'e' na URL

    console.log(`[ApostaService] Chamando GET: ${this.apiUrlApostaRodada}/ListarPorRodadaEUsuario com params: ${params.toString()}`);
    // Tipo de retorno esperado da API: ApiResponse<PreservedCollection<ApostaRodadaDto[]>>
    return this.http.get<ApiResponse<PreservedCollection<ApostaRodadaDto[]>>>(`${this.apiUrlApostaRodada}/ListarPorRodadaEUsuario`, { params }).pipe(
      map(apiResponse => {
        let extractedData: ApostaRodadaDto[] = [];
        if (apiResponse.success && apiResponse.data && isPreservedCollection<ApostaRodadaDto[]>(apiResponse.data)) {
          // Extrai o array de DTOs que está dentro do $values da PreservedCollection
          extractedData = (apiResponse.data.$values && apiResponse.data.$values.length > 0) ? apiResponse.data.$values[0] : [];
          console.log('[ApostaService] Apostas por rodada e usuário recebidas como PreservedCollection de array. Extraindo o array interno.');
        } else if (apiResponse.success && Array.isArray(apiResponse.data)) {
          // Fallback caso a API retorne um array direto (sem PreservedCollection)
          extractedData = (apiResponse.data as ApostaRodadaDto[]);
          console.warn('[ApostaService] Dados recebidos diretamente como array (getApostasPorRodadaEUsuario), sem PreservedCollection.');
        } else {
          console.warn('[ApostaService] Resposta da API sem dados ou formato inesperado para ListarPorRodadaEUsuario:', apiResponse);
        }

        // Constrói um novo objeto ApiResponse com a 'data' já desempacotada para o tipo desejado
        return {
          ...apiResponse,
          data: extractedData
        } as ApiResponse<ApostaRodadaDto[]>; // Garante o tipo de retorno do Observable
      }),
      catchError(this.handleError)
    );
  }

  /**
   * Obtém os jogos de uma rodada com os palpites existentes para edição.
   * @param rodadaId O ID da rodada.
   * @param apostadorCampeonatoId O ID da associação apostador-campeonato (com 'o').
   * @returns Um Observable com a resposta da API contendo uma coleção de ApostaJogoVisualizacaoDto.
   
  getApostasParaEdicao(rodadaId: string, apostadorCampeonatoId: string): Observable<ApiResponse<ApostaJogoVisualizacaoDto[]>> {
    const params = new HttpParams()
      .set('rodadaId', rodadaId)
      .set('apostadorCampeonatoId', apostadorCampeonatoId); // Backend espera 'e' na URL

    console.log(`[ApostaService] Chamando GET: ${this.apiUrlApostaRodada}/ParaEdicao com params: ${params.toString()}`);
    // Tipo de retorno esperado da API: ApiResponse<PreservedCollection<ApostaJogoVisualizacaoDto[]>>
    return this.http.get<ApiResponse<PreservedCollection<ApostaJogoVisualizacaoDto[]>>>(`${this.apiUrlApostaRodada}/ParaEdicao`, { params }).pipe(
      map(apiResponse => {
        let extractedData: ApostaJogoVisualizacaoDto[] = [];
        if (apiResponse.success && apiResponse.data && isPreservedCollection<ApostaJogoVisualizacaoDto[]>(apiResponse.data)) {
          // Extrai o array de DTOs que está dentro do $values da PreservedCollection
          extractedData = (apiResponse.data.$values && apiResponse.data.$values.length > 0) ? apiResponse.data.$values[0] : [];
          console.log('[ApostaService] Jogos para edição recebidos como PreservedCollection de array. Extraindo o array interno.');
        } else if (apiResponse.success && Array.isArray(apiResponse.data)) {
          // Fallback caso a API retorne um array direto (sem PreservedCollection)
          extractedData = (apiResponse.data as ApostaJogoVisualizacaoDto[]);
          console.warn('[ApostaService] Dados recebidos diretamente como array (getApostasParaEdicao), sem PreservedCollection.');
        } else {
          console.warn('[ApostaService] Resposta da API sem dados ou formato inesperado para ParaEdicao:', apiResponse);
        }

        // Constrói um novo objeto ApiResponse com a 'data' já desempacotada para o tipo desejado
        return {
          ...apiResponse,
          data: extractedData
        } as ApiResponse<ApostaJogoVisualizacaoDto[]>; // Garante o tipo de retorno do Observable
      }),
      catchError(this.handleError)
    );
  }

  /**
   * Salva ou atualiza as apostas de uma rodada.
   * @param request O DTO com os dados da aposta a serem salvos.
   * @returns Um Observable com a resposta da API.
   
  salvarApostas(request: SalvarApostaRequestDto): Observable<ApiResponse<ApostaRodadaDto>> {
    console.log(`[ApostaService] Chamando POST: ${this.apiUrlAposta}/SalvarApostas`, request);
    return this.http.post<ApiResponse<ApostaRodadaDto>>(`${this.apiUrlAposta}/SalvarApostas`, request).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Manipulador de erros HTTP genérico para o ApostaService.
   
  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('[ApostaService] Erro na requisição HTTP:', error);
    let errorMessage = 'Ocorreu um erro desconhecido ao processar a aposta.';
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
