// ApostasApp.Core.Application.DTOs/ApostasRodada/ApostaRodadaDto.cs
using ApostasApp.Core.Application.DTOs.Palpites; // Para PalpiteDto
using ApostasApp.Core.Application.DTOs.ApostadorCampeonatos; // <<-- NOVO: Para ApostadorCampeonatoDto -->>
using System;
using System.Collections.Generic;

namespace ApostasApp.Core.Application.DTOs.ApostasRodada
{
    /// <summary>
    /// DTO para representar uma submissão completa de apostas para uma rodada (ApostaRodada).
    /// </summary>
    public class ApostaRodadaDto
    {
        public string Id { get; set; }
        public string ApostadorCampeonatoId { get; set; }
        public string RodadaId { get; set; }
        public string IdentificadorAposta { get; set; }
        public DateTime? DataHoraSubmissao { get; set; }
        public bool EhApostaCampeonato { get; set; }
        public bool EhApostaIsolada { get; set; }
        public decimal? CustoPagoApostaRodada { get; set; }
        public int PontuacaoTotalRodada { get; set; }
        public bool Enviada { get; set; }

        // DTO aninhado para os detalhes do apostador no campeonato (se necessário para exibição)
        public ApostadorCampeonatoDto ApostadorCampeonato { get; set; } // <<-- Adicionado aqui -->>

        // Coleção de palpites individuais para esta aposta de rodada
        public IEnumerable<PalpiteDto> Palpites { get; set; }
    }
}