// RodadaDto.cs
// Definição do DTO para Rodada, alinhado com a entidade e necessidade do frontend.
// Localização: ApostasApp.Core.Application.DTOs.Rodadas

using ApostasApp.Core.Application.DTOs.Campeonatos; // Para CampeonatoDto
using System; // Necessário para Guid

namespace ApostasApp.Core.Application.DTOs.Rodadas
{
    public class RodadaDto
    {
        public string Id { get; set; }
        public int NumeroRodada { get; set; } // Mudado de 'Number' para 'NumeroRodada'
        public string Nome { get; set; } // Adicionado 'Nome' para corresponder ao DTO do Angular
        public int NumJogos { get; set; }
        public string Status { get; set; } // Ex: "Corrente", "Finalizada"
        public DateTime DataInicio { get; set; } // Adicionado para exibir no Angular
        public DateTime DataFim { get; set; } // Adicionado para exibir no Angular
        public int ChampionshipId { get; set; } // Mantido para referência ao Campeonato
        public CampeonatoDto Campeonato { get; set; } // Campeonato associado à rodada
    }
}
