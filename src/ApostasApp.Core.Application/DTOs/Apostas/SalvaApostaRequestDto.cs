// ApostasApp.Core.Application.DTOs.Apostas/SalvarApostaRequestDto.cs
// DTO para receber os dados de uma aposta a ser salva/atualizada
using ApostasApp.Core.Application.DTOs.Apostas.ApostasApp.Core.Application.DTOs.Apostas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; // Necessário para JsonPropertyName

namespace ApostasApp.Core.Application.DTOs.Apostas
{
    public class SalvarApostaRequestDto
    {
        [Required(ErrorMessage = "O ID do apostador no campeonato é obrigatório.")]
        [JsonPropertyName("apostadorCampeonatoId")] // Mapeia 'apostadorCampeonatoId' do frontend
        public string ApostadorCampeonatoId { get; set; }

        [Required(ErrorMessage = "O ID da rodada é obrigatório.")]
        [JsonPropertyName("rodadaId")] // Mapeia 'rodadaId' do frontend
        public string RodadaId { get; set; }

        // <<-- CORRIGIDO: Nome da propriedade e tipo do DTO interno -->>
        [Required(ErrorMessage = "Pelo menos um palpite é obrigatório.")]
        [JsonPropertyName("apostasJogos")] // Mapeia 'apostasJogos' do frontend para 'ApostasJogos'
        public List<ApostaJogoRequestDto> ApostasJogos { get; set; } = new List<ApostaJogoRequestDto>();

        // <<-- CORRIGIDO: Nome da propriedade para EhCampeonato e mapeamento -->>
        [JsonPropertyName("ehCampeonato")] // Mapeia 'ehCampeonato' do frontend
        public bool EhCampeonato { get; set; }

        [JsonPropertyName("identificadorAposta")] // Mapeia 'identificadorAposta' do frontend
        public string IdentificadorAposta { get; set; }

        // Removidas as propriedades 'EhApostaIsolada' e 'Enviada' pois não são enviadas pelo frontend
        // e podem causar confusão se não forem usadas ou inicializadas corretamente.
        // Se forem necessárias, o frontend precisará enviá-las.
    }
}
