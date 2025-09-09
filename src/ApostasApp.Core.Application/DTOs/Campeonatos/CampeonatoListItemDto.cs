// CampeonatoListItemDto.cs
// DTO específico para a listagem de campeonatos.
// Localização: ApostasApp.Core.Application.DTOs.Campeonatos

// Não precisamos mais do using para TiposCampeonato se o Tipo for string no DTO
// using ApostasApp.Core.Domain.Models.Campeonatos; // Apenas se o DTO ainda usar o enum

namespace ApostasApp.Core.Application.DTOs.Campeonatos
{
    public class CampeonatoListItemDto
    {
        public string Id { get; set; } // <<-- CORRIGIDO: ID como string -->>
        public string Nome { get; set; }
        public DateTime DataInic { get; set; }
        public DateTime DataFim { get; set; }
        public int NumRodadas { get; set; }
        public string Tipo { get; set; } // <<-- CORRIGIDO: Enum mapeado para string -->>
        public bool Ativo { get; set; }
        public decimal? CustoAdesao { get; set; }
    }
}
