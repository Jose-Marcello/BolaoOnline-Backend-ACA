// ApostasApp.Core.Application.DTOs/Rodadas/RodadaDto.cs
using ApostasApp.Core.Application.DTOs.Campeonatos; // Para CampeonatoDto
using System;

namespace ApostasApp.Core.Application.DTOs.Rodadas
{
    public class RodadaDto
    {
        public string Id { get; set; } // ID da Rodada como string
        public string CampeonatoId { get; set; } // <<-- ADICIONADO: ID do Campeonato como string -->>
        public int NumeroRodada { get; set; }
        public DateTime DataInic { get; set; }
        public DateTime DataFim { get; set; }
        public int NumJogos { get; set; }
        public string Status { get; set; } // Status da Rodada como string

        public decimal? CustoApostaRodada { get; set; } // Custo da Aposta Avulsa (se houver)

        // Propriedade de navegação para o DTO do Campeonato (se for carregado)
        public CampeonatoDto Campeonato { get; set; }
    }
}
