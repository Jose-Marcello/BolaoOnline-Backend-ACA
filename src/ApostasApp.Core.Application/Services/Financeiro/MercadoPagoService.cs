// Localização ideal: ApostasApp.Core.Application.Services/MercadoPagoService.cs
// (Você pode criar uma subpasta para serviços externos se quiser)

using ApostasApp.Core.Application.Services.Interfaces;
using ApostasApp.Core.Domain.Models.Configuracoes;
using ApostasApp.Core.Domain.Models.Financeiro;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ApostasApp.Core.Application.Services
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly HttpClient _httpClient;
        private readonly MercadoPagoSettings _settings;



        public MercadoPagoService(HttpClient httpClient, IOptions<MercadoPagoSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;


            _httpClient.BaseAddress = new Uri("https://api.mercadopago.com/v1/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.AccessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        
        // MercadoPagoService.cs
        public async Task<PixResponseDto> CriarPagamentoPixAsync(decimal valor, string descricao, string idExterno)
        {
            var requestBody = new
            {
                transaction_amount = valor,
                description = descricao,
                payment_method_id = "pix",
                external_reference = idExterno,
                payer = new
                {
                    email = "test.user@example.com", // E-mail de teste genérico
                    first_name = "Test",
                    last_name = "User"
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // **NOVO: Adicione este cabeçalho!**
            var idempotencyKey = Guid.NewGuid().ToString();
            _httpClient.DefaultRequestHeaders.Add("X-Idempotency-Key", idempotencyKey);

            var response = await _httpClient.PostAsync("payments", httpContent);

            _httpClient.DefaultRequestHeaders.Remove("X-Idempotency-Key");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var paymentResponse = JsonSerializer.Deserialize<PaymentResponseDto>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return new PixResponseDto
                {
                    QrCodeBase64 = paymentResponse.PointOfInteraction.TransactionData.QrCodeBase64,
                    PixCopiaECola = paymentResponse.PointOfInteraction.TransactionData.QrCode,
                    ChaveTransacao = paymentResponse.ExternalReference
                };
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Erro ao criar pagamento no PagSegro. Status: {response.StatusCode}. Erro: {errorBody}");
            }
        }
    }
}