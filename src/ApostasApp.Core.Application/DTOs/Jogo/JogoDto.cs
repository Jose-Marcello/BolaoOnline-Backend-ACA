// ApostasApp.Core.Application.DTOs/Jogos/JogoDto.cs
using System;
using ApostasApp.Core.Domain.Models.Jogos; // Para o enum StatusJogo, se aplicável

namespace ApostasApp.Core.Application.DTOs.Jogos
{
    /// <summary>
    /// DTO para transferir dados de Jogo da camada de Aplicação para a camada de Apresentação.
    /// </summary>
    public class JogoDto
    {
        public string Id { get; set; }
        public string RodadaId { get; set; }

        // IDs das Equipes no Campeonato (referência para a entidade de junção)
        public string EquipeCasaCampeonatoId { get; set; } // <<-- NOVO: ID da EquipeCampeonato para a casa -->>
        public string EquipeVisitanteCampeonatoId { get; set; } // <<-- NOVO: ID da EquipeCampeonato para o visitante -->>

        // <<-- CORRIGIDO: Denormalizado Nome e Escudo das Equipes -->>
        public string EquipeCasaNome { get; set; }
        public string EquipeCasaEscudoUrl { get; set; } // Usando "EscudoUrl" para clareza
        public string EquipeVisitanteNome { get; set; }
        public string EquipeVisitanteEscudoUrl { get; set; } // Usando "EscudoUrl" para clareza

        public string EstadioId { get; set; }
        public string EstadioNome { get; set; }
        public DateTime DataHora { get; set; }
        public string Status { get; set; } // Ou public StatusJogo Status { get; set; }
        public int? PlacarCasa { get; set; }
        public int? PlacarFora { get; set; }
    }
}