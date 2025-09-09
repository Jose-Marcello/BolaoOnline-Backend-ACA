// ApostasApp.Core.Application.DTOs.Apostas/ApostaRodadaStatusDto.cs
using System;
using System.Text.Json.Serialization; // Necessário para JsonPropertyName

namespace ApostasApp.Core.Application.DTOs.Apostas
{
    public class ApostaRodadaStatusDto
    {
        // Propriedades que o frontend espera
        [JsonPropertyName("statusAposta")] // Mapeia para 'statusAposta' no frontend
        public int StatusAposta { get; set; } // 1 = Pendente, 2 = Enviada
        public string IdentificadorAposta { get; set; }
        public string ApostadorCampeonatoId { get; set; }
        public string RodadaId { get; set; }

        [JsonPropertyName("dataAposta")] // Mapeia para 'dataAposta' no frontend
        public DateTime? DataAposta { get; set; } // Data e hora da submissão (pode ser nula)

        // Propriedades que o seu backend parece estar referenciando
        public Guid ApostaRodadaId { get; set; } // ID da ApostaRodada
        public bool Enviada { get; set; } // Status de envio (true/false)
        public DateTime? DataHoraSubmissao { get; set; } // Data/hora da submissão (pode ser nula)
    }
}
