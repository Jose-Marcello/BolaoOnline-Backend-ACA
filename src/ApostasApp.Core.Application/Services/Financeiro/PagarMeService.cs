// Localização: ApostasApp.Core.Application.Services/PagarMeService.cs

using ApostasApp.Core.Application.Services.Interfaces;
using ApostasApp.Core.Domain.Models.Configuracoes;
using ApostasApp.Core.Domain.Models.Financeiro;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ApostasApp.Core.Application.Services
{
    public class PagarMeService : IPagarMeService
    {
        private readonly HttpClient _httpClient;
        private readonly PagarMeSettings _settings;

        public PagarMeService(HttpClient httpClient, IOptions<PagarMeSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;

            // Configurações base da API do Pagar.me
            _httpClient.BaseAddress = new Uri("https://api.pagar.me/core/v5/");
            // O Pagar.me usa um padrão diferente, o 'Basic' com a secret key
            var authString = $"{_settings.SecretKey}:";
            var authBytes = Encoding.UTF8.GetBytes(authString);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Criar o Pix na API do Pagar.me
        public async Task<PixResponseDto> CriarPagamentoPixAsync(decimal valor, string descricao, string idExterno)
        {
            var requestBody = new
            {
                amount = (int)(valor * 100), // Pagar.me usa o valor em centavos
                payment_method = "pix",
                description = descricao,
                // O Pagar.me usa 'customer', 'reference_key', etc., de forma diferente
                // Você pode adicionar mais detalhes aqui, como dados do cliente, se precisar
                customer = new
                {
                    name = "Cliente de Exemplo", // Substitua com os dados do apostador
                    email = "exemplo@bolaoonline.com",
                    type = "individual"
                },
                pix = new
                {
                    expires_in = 3600 // Tempo de expiração em segundos (1 hora)
                },
                // Adicione a sua referência externa para o Pagar.me
                metadata = new
                {
                    external_reference = idExterno
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("charges", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var pagarmeResponse = JsonSerializer.Deserialize<dynamic>(responseBody);

                // O Pagar.me retorna os dados do Pix em um formato diferente do Mercado Pago
                return new PixResponseDto
                {
                    QrCodeBase64 = pagarmeResponse.charges[0].last_transaction.qr_code_base64,
                    PixCopiaECola = pagarmeResponse.charges[0].last_transaction.qr_code,
                    ChaveTransacao = pagarmeResponse.charges[0].last_transaction.id.ToString() // ID da transação no Pagar.me
                };
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Erro ao criar pagamento no Pagar.me. Status: {response.StatusCode}. Erro: {errorBody}");
            }
        }
    }
}