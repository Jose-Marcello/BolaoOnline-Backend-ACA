// ApostasApp.Core.Application.DTOs.Apostas/SalvarPalpiteRequestDto.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; // Necessário para JsonPropertyName

namespace ApostasApp.Core.Application.DTOs.Apostas
{
    /// <summary>
    /// DTO para a requisição de adicionar ou atualizar um palpite.
    /// Contém os dados necessários para registrar um palpite de jogo.
    /// </summary>
    public class SalvarPalpiteRequestDto
    {
        // O frontend envia 'jogoId'
        [Required(ErrorMessage = "O ID do Jogo é obrigatório.")]
        [JsonPropertyName("jogoId")] // Mapeia 'jogoId' do frontend para 'JogoId' do backend
        public string JogoId { get; set; }

        // O frontend não envia 'apostadorCampeonatoId' para cada palpite individualmente
        // Este campo é para o DTO principal (SalvarApostaRequestDto), não para cada item da lista.
        // Se este DTO for usado em outro contexto onde 'ApostadorCampeonatoId' é necessário,
        // considere criar um DTO mais específico ou torná-lo opcional aqui.
        // Por enquanto, vou remover o [Required] e o JsonPropertyName para evitar problemas
        // se ele não for enviado para cada item do array.
        // [Required(ErrorMessage = "O ID do Apostador Campeonato é obrigatório.")]
        // [JsonPropertyName("apostadorCampeonatoId")] // Removido, pois não é enviado por item
        public string ApostadorCampeonatoId { get; set; } // Mantido, mas não [Required] ou JsonPropertyName

        // <<-- CORRIGIDO: Tipos para int? e JsonPropertyName -->>
        [Required(ErrorMessage = "O placar da casa é obrigatório.")]
        [Range(0, 99, ErrorMessage = "O placar da casa deve ser entre 0 e 99.")]
        [JsonPropertyName("placarCasa")] // Mapeia 'placarCasa' do frontend para 'PlacarApostaCasa'
        public int? PlacarApostaCasa { get; set; } // int? para aceitar null do frontend

        // <<-- CORRIGIDO: Tipos para int? e JsonPropertyName -->>
        [Required(ErrorMessage = "O placar do visitante é obrigatório.")]
        [Range(0, 99, ErrorMessage = "O placar do visitante deve ser entre 0 e 99.")]
        [JsonPropertyName("placarVisitante")] // Mapeia 'placarVisitante' do frontend para 'PlacarApostaVisita'
        public int? PlacarApostaVisita { get; set; } // int? para aceitar null do frontend

        // Pontuação será calculada pelo sistema, não enviada pelo usuário
        // DataHoraAposta será preenchida no serviço, não enviada pelo usuário
    }
}
