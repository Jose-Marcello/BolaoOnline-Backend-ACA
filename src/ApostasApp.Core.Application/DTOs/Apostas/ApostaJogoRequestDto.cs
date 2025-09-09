// ApostasApp.Core.Application.DTOs.Apostas/ApostaJogoRequestDto.cs
// Renomeado de PalpiteRequestDto para ApostaJogoRequestDto para consistência com o frontend
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; // Necessário para JsonPropertyName

namespace ApostasApp.Core.Application.DTOs.Apostas
{
    // ApostasApp.Core.Application.DTOs.Apostas/ApostaJogoRequestDto.cs
    // DTO para representar um palpite individual para a requisição de salvar aposta.
    // Alinhado com a interface 'ApostaJogoRequest' do frontend.
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization; // Necessário para JsonPropertyName

    namespace ApostasApp.Core.Application.DTOs.Apostas
    {
        public class ApostaJogoRequestDto // Nome que espelha o frontend ApostaJogoRequest
        {
            [Required(ErrorMessage = "O ID do jogo é obrigatório.")]
            [JsonPropertyName("jogoId")] // Mapeia 'jogoId' (camelCase do frontend) para 'JogoId' (PascalCase do backend)
            public string JogoId { get; set; }

            [Range(0, 99, ErrorMessage = "O placar do time da casa deve ser entre 0 e 99.")]
            [JsonPropertyName("placarCasa")] // Mapeia 'placarCasa' (camelCase do frontend) para 'PlacarCasa' (PascalCase do backend)
            public int? PlacarCasa { get; set; } // int? para aceitar números e nulls do frontend

            [Range(0, 99, ErrorMessage = "O placar do time de fora deve ser entre 0 e 99.")]
            [JsonPropertyName("placarVisitante")] // Mapeia 'placarVisitante' (camelCase do frontend) para 'PlacarVisitante' (PascalCase do backend)
            public int? PlacarVisitante { get; set; } // int? para aceitar números e nulls do frontend
        }
    }


    /*
    public class ApostaJogoRequestDto // Renomeado para refletir o nome do frontend
    {
        // 'IdPalpite' não é enviado pelo frontend nesta requisição, pode ser mantido ou removido se não for usado.
        // Se for para um palpite existente, pode ser Guid? ou string.
        public string IdPalpite { get; set; }

        [Required(ErrorMessage = "O ID do jogo é obrigatório.")]
        [JsonPropertyName("jogoId")] // Mapeia 'jogoId' do frontend para 'IdJogo' do backend
        public string IdJogo { get; set; }

        [Range(0, 99, ErrorMessage = "O placar do time da casa deve ser entre 0 e 99.")]
        [JsonPropertyName("placarCasa")] // Mapeia 'placarCasa' do frontend para 'PlacarApostaCasa'
        public int? PlacarApostaCasa { get; set; } // int? para aceitar null do frontend

        [Range(0, 99, ErrorMessage = "O placar do time de fora deve ser entre 0 e 99.")]
        [JsonPropertyName("placarVisitante")] // Mapeia 'placarVisitante' do frontend para 'PlacarApostaVisita'
        public int? PlacarApostaVisita { get; set; } // int? para aceitar null do frontend
    }

    */
}
