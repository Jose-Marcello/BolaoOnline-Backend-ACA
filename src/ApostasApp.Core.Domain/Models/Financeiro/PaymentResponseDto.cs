// MercadoPagoDtos.cs
using System.Text.Json.Serialization;

namespace ApostasApp.Core.Domain.Models.Financeiro
{
    // DTOs para a resposta da API do Mercado Pago

    public class PaymentResponseDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("status_detail")]
        public string StatusDetail { get; set; }

        [JsonPropertyName("external_reference")]
        public string ExternalReference { get; set; }

        [JsonPropertyName("point_of_interaction")]
        public PointOfInteractionDto PointOfInteraction { get; set; }
    }

    public class PointOfInteractionDto
    {
        [JsonPropertyName("transaction_data")]
        public TransactionDataDto TransactionData { get; set; }
    }

    public class TransactionDataDto
    {
        [JsonPropertyName("qr_code_base64")]
        public string QrCodeBase64 { get; set; }

        [JsonPropertyName("qr_code")]
        public string QrCode { get; set; }
    }

    // DTO que você já tem para enviar ao frontend
    public class PixResponseDto
    {
        public string QrCodeBase64 { get; set; }
        public string PixCopiaECola { get; set; }
        public string ChaveTransacao { get; set; }
    }
}