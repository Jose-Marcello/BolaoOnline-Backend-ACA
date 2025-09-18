using ApostasApp.Core.Domain.Interfaces.Relatorios;
using System;

namespace ApostasApp.Core.Infrastructure.Data.Models
{
    public class ConferenciaPalpiteDataModel : IConferenciaPalpite
    {
        public string ApelidoApostador { get; set; }
        public string IdentificadorAposta { get; set; }
        public DateTime DataHoraEnvio { get; set; }
        public string NomeEquipeCasa { get; set; }
        public int PlacarPalpiteCasa { get; set; }
        public int PlacarPalpiteVisita { get; set; }
        public string NomeEquipeVisita { get; set; }
        public DateTime DataJogo { get; set; }
        public string HoraJogo { get; set; }

    }
}