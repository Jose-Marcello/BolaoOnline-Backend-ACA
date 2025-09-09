using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.Financeiro
{
    /// <summary>
    /// Representa a resposta após a geração de um PIX para depósito.
    /// </summary>
    public class SimulaPixResponseDto
    {
        [Required]
        public bool Sucesso { get; set; }

        public string Mensagem { get; set; }

        /// <summary>
        /// ID da transação no sistema de pagamento externo (simulado ou real).
        /// </summary>
        public string IdTransacao { get; set; }

        /// <summary>
        /// O QR Code do PIX em formato Base64.
        /// </summary>
        public string QrCodeBase64 { get; set; }

        /// <summary>
        /// A string do QR Code do PIX (o "copia e cola").
        /// </summary>
        public string QrCodeString { get; set; }
    }
}