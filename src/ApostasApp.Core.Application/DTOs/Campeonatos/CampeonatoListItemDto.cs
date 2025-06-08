// ApostasApp.Core.Application.DTOs.Campeonatos/CampeonatoListItemDto.cs
using ApostasApp.Core.Domain.Models.Campeonatos;
using System;

namespace ApostasApp.Core.Application.DTOs.Campeonatos
{
    public class CampeonatoListItemDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty; // Inicialize para evitar nulls
        public DateTime DataInic { get; set; }
        public DateTime DataFim { get; set; }
        public int NumRodadas { get; set; }
        public decimal? CustoAdesao { get; set; } // Pode ser nulo se não houver custo
        public bool Ativo { get; set; }
        // Outras propriedades relevantes para a exibição no Dashboard, se houver
        public TiposCampeonato Tipo { get; set; }

    }
}