// CampeonatoListItemDto.cs
// DTO específico para a listagem de campeonatos.
// Localização: ApostasApp.Core.Application.DTOs.Campeonatos

using System;
using ApostasApp.Core.Domain.Models.Campeonatos; // Para TiposCampeonato

namespace ApostasApp.Core.Application.DTOs.Campeonatos
{
    public class CampeonatoListItemDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataInic { get; set; } // Note 'DataInic' (original da entidade)
        public DateTime DataFim { get; set; }
        public int NumRodadas { get; set; }
        public TiposCampeonato Tipo { get; set; } // O enum direto aqui, será mapeado para string depois
        public bool Ativo { get; set; }
        public decimal? CustoAdesao { get; set; }
    }
}
