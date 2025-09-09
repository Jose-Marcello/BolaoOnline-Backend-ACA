// ApostasApp.Core.Application.DTOs/Rodadas/RodadaDto.cs
using ApostasApp.Core.Application.DTOs.Campeonatos; // Para CampeonatoDto
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Rodadas;
using System;

namespace ApostasApp.Core.Application.DTOs.RankingRodadas
{
    public class RankingRodadaDto
    {
        public Guid RodadaId { get; set; }
        public Guid ApostadorCampeonatoId { get; set; }
        public Guid ApostadorId { get; set; } // <<-- ADICIONE ESTE CAMPO
        public string NomeApostador { get; set; } // <<-- ADICIONE ESTE CAMPO
        public int Pontuacao { get; set; }
        public int Posicao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public string Apelido { get; set; }
        public string FotoPerfil { get; set; }

    }
}
