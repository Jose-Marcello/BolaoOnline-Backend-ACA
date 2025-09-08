using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.Financeiro
{
    /// <summary>
    /// Representa o resultado de uma operação de pagamento ou atualização de status.
    /// </summary>
    public class SimulacaoPagamentoResultadoDto
    {
        [Required]
        public bool Sucesso { get; set; }

        public string Mensagem { get; set; }

        /// <summary>
        /// O ID da transação à qual este resultado se refere.
        /// </summary>
        public string IdTransacao { get; set; }
    }
}