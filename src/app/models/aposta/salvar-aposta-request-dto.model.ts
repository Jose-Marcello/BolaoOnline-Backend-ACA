// Localização: src/app/models/aposta/salvar-aposta-request.model.ts

export interface SalvarApostaRequestDto {
  id?: string; // Adicionado para permitir atualização de aposta existente
  campeonatoId: string;
  rodadaId: string;
  apostadorCampeonatoId: string; // <<-- PADRONIZADO PARA 'o' AQUI -->>
  ehApostaIsolada: boolean;
  identificadorAposta: string;
  apostasJogos: ApostaJogoRequest[];
  ehCampeonato: boolean; // <<-- CORRIGIDO: Propriedade ehCampeonato -->>
}

export interface ApostaJogoRequest {
  jogoId: string;
  placarCasa: number;
  placarVisitante: number;
}
