// ApostasApp.Core.Application.DTOs.Apostas/ApostaJogoDto.cs
// DTO para enviar os dados dos jogos e palpites existentes do backend para o frontend (visualização).
using System;
using System.Text.Json.Serialization; // Necessário para JsonPropertyName

namespace ApostasApp.Core.Application.DTOs.Apostas
{
    public class ApostaJogoDto
    {
        // ID do palpite existente (se houver), ou Guid.Empty.ToString() para novo
        [JsonPropertyName("id")] // Mapeia para 'id' no frontend se necessário
        public string Id { get; set; }

        // ID do jogo
        [JsonPropertyName("jogoId")] // Mapeia para 'jogoId' no frontend
        public string IdJogo { get; set; }

        // Informações da Equipe Mandante
        [JsonPropertyName("equipeCasaNome")]
        public string EquipeMandante { get; set; }
        [JsonPropertyName("siglaMandante")]
        public string SiglaMandante { get; set; }
        [JsonPropertyName("escudoCasaUrl")] // Mapeia para 'escudoCasaUrl' no frontend
        public string EscudoMandante { get; set; }

        // Placar do palpite da casa (string, pois o método de serviço retorna "" ou ToString())
        [JsonPropertyName("apostaCasa")] // Mapeia para 'apostaCasa' no frontend
        public string PlacarMandante { get; set; }

        // Informações da Equipe Visitante
        [JsonPropertyName("equipeVisitanteNome")]
        public string EquipeVisitante { get; set; }
        [JsonPropertyName("siglaVisitante")]
        public string SiglaVisitante { get; set; }
        [JsonPropertyName("escudoVisitanteUrl")] // Mapeia para 'escudoVisitanteUrl' no frontend
        public string EscudoVisitante { get; set; }

        // Placar do palpite do visitante (string, pois o método de serviço retorna "" ou ToString())
        [JsonPropertyName("apostaVisitante")] // Mapeia para 'apostaVisitante' no frontend
        public string PlacarVisitante { get; set; }

        // Data e Hora do Jogo
        [JsonPropertyName("dataJogo")]
        public string DataJogo { get; set; }
        [JsonPropertyName("horaJogo")]
        public string HoraJogo { get; set; }

        // Propriedades adicionais que o frontend ApostaJogoVisualizacaoDto espera (podem ser nulas no backend)
        [JsonPropertyName("campeonatoId")]
        public string CampeonatoId { get; set; } // Pode ser preenchido se necessário
        [JsonPropertyName("rodadaId")]
        public string RodadaId { get; set; } // Pode ser preenchido se necessário
        [JsonPropertyName("placarOficialCasa")]
        public int? PlacarOficialCasa { get; set; } // Não preenchido por este método, mas pode ser útil
        [JsonPropertyName("placarOficialVisitante")]
        public int? PlacarOficialVisitante { get; set; } // Não preenchido por este método, mas pode ser útil
        [JsonPropertyName("statusApostaJogo")]
        public string StatusApostaJogo { get; set; } // Não preenchido por este método, mas pode ser útil
        [JsonPropertyName("pontosGanhos")]
        public int? PontosGanhos { get; set; } // Não preenchido por este método, mas pode ser útil
    }
}
