// DTO para representar os dados de um palpite de jogo para visualização (com placares reais e pontuação)
using System;

namespace ApostasApp.Core.Application.DTOs.Apostas
{
    public class ApostaJogoVisualizacaoDto
    {
        public Guid Id { get; set; } // ID do Palpite
        public Guid IdJogo { get; set; }
        public string EquipeMandante { get; set; }
        public string SiglaMandante { get; set; }
        public string EscudoMandante { get; set; }
        public int? PlacarRealCasa { get; set; }
        public int? PlacarApostaCasa { get; set; }
        public string EquipeVisitante { get; set; }
        public string SiglaVisitante { get; set; }
        public string EscudoVisitante { get; set; }
        public int? PlacarRealVisita { get; set; }
        public int? PlacarApostaVisita { get; set; }
        public string DataJogo { get; set; }
        public string HoraJogo { get; set; }
        public string StatusJogo { get; set; } // String do enum StatusJogo
        public bool Enviada { get; set; }
        public int Pontuacao { get; set; }
    }
}