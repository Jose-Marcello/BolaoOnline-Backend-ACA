// ApostasApp.Core.Application.DTOs.Apostas/ApostaJogoVisualizacaoDto.cs
using System;

namespace ApostasApp.Core.Application.DTOs.Apostas
{
    public class ApostaJogoEdicaoDto
    {
        public string Id { get; set; } // <<-- CORRIGIDO: ID do Palpite como string -->>
        public string IdJogo { get; set; } // <<-- CORRIGIDO: ID do Jogo como string -->>
       
        public string EquipeMandante { get; set; }
        public string SiglaMandante { get; set; }
        public string EscudoMandante { get; set; }

        public string EquipeVisitante { get; set; }
        public string SiglaVisitante { get; set; }
        public string EscudoVisitante { get; set; }

        public int? PlacarApostaCasa { get; set; }
        public int? PlacarApostaVisita { get; set; }
        
       public string DataJogo { get; set; }
        public string HoraJogo { get; set; }
        
        public string StatusJogo { get; set; } // String do enum StatusJogo
        public string EstadioNome { get; set; } 

        public bool Enviada { get; set; }
      
    }
}
