using System;

namespace ApostasApp.Core.Application.DTOs.Apostas
{
    public class ApostaJogoDto
    {
        public Guid Id { get; set; } // ID do Palpite
        public Guid IdJogo { get; set; }
        public string EquipeMandante { get; set; }
        public string SiglaMandante { get; set; }
        public string EscudoMandante { get; set; } // URL do escudo
        public string PlacarMandante { get; set; } // Pode ser string para lidar com "" quando não enviado
        public string EquipeVisitante { get; set; }
        public string SiglaVisitante { get; set; }
        public string EscudoVisitante { get; set; } // URL do escudo
        public string PlacarVisitante { get; set; } // Pode ser string para lidar com "" quando não enviado
        public string DataJogo { get; set; } // Data formatada
        public string HoraJogo { get; set; } // Hora formatada
    }
}