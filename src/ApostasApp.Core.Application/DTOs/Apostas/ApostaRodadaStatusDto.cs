using System;

namespace ApostasApp.Core.Application.DTOs.Apostas
{
    // DTO para retornar o status da aposta na rodada para o usuário
    public class ApostaRodadaStatusDto
    {
        public Guid? ApostaRodadaId { get; set; } // ID da ApostaRodada, pode ser null se não houver aposta
        public bool Enviada { get; set; }
        public DateTime? DataHoraAposta { get; set; }
    }
}
