using ApostasApp.Core.Application.DTOs.Campeonatos;
using ApostasApp.Core.Application.DTOs.Equipes;
using ApostasApp.Core.Application.DTOs.Estadios;
using ApostasApp.Core.Application.DTOs.Rodadas;
using ApostasApp.Core.Domain.Models.Jogos; // Importar o namespace do enum StatusJogo
using System;

namespace ApostasApp.Core.Application.DTOs.Jogos
{
    /// <summary>
    /// DTO para exibir os detalhes completos de um jogo.
    /// Contém informações do jogo, rodada, campeonato, equipes, estádio e status.
    /// </summary>
    public class JogoDetalheDto
    {
        public string Id { get; set; }
        public DateTime DataJogo { get; set; }
        public TimeSpan HoraJogo { get; set; }
        public int GolsCasa { get; set; }
        public int GolsVisitante { get; set; }
        public bool JogoEncerrado { get; set; }

        // Nova propriedade para o status do jogo
        public StatusJogo Status { get; set; }

        // Relacionamentos
        public RodadaDto Rodada { get; set; }
        public EquipeCampeonatoDto EquipeCasa { get; set; }
        public EquipeCampeonatoDto EquipeVisitante { get; set; }
        public EstadioDto Estadio { get; set; }
    }
}
