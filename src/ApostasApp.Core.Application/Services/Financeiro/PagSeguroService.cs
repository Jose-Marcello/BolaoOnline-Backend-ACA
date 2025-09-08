// Localização: ApostasApp.Core.Application.Services/PagSeguroService.cs

using ApostasApp.Core.Application.Services.Interfaces;
using ApostasApp.Core.Domain.Models.Configuracoes;
using ApostasApp.Core.Domain.Models.Financeiro;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ApostasApp.Core.Application.Services
{
    public class PagSeguroService : IPagSeguroService
    {
        private readonly HttpClient _httpClient;
        private readonly PagSeguroSettings _settings;

        public PagSeguroService(HttpClient httpClient, IOptions<PagSeguroSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;

            // Use o token diretamente para autenticação
            _httpClient.BaseAddress = new Uri("https://api.sandbox.pagseguro.com/charges");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public async Task<PixResponseDto> CriarPagamentoPixAsync(decimal valor, string descricao, string idExterno)
        {
            // A API do PagSeguro espera o valor em centavos
            var valorEmCentavos = (int)(valor * 100);

            var requestBody = new
            {
                amount = new { value = valorEmCentavos },
                reference_id = idExterno,
                description = descricao,
                payment_method = "pix",
                qr_codes = new[]
                {
                    new { amount = new { value = valorEmCentavos } }
                },
                // Pagador, é necessário para a validação. Dados genéricos costumam funcionar no Sandbox.
                customer = new
                {
                    name = "Comprador de Teste",
                    email = "teste@sandbox.com"
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            //var response = await _httpClient.PostAsync("", httpContent);
            var response = await _httpClient.PostAsync("charges", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                // Ajustar a deserialização de acordo com a resposta da API do PagSeguro
                // O PagSeguro retorna um formato diferente de resposta
                var pagseguroResponse = JsonSerializer.Deserialize<dynamic>(responseBody);

                return new PixResponseDto
                {
                    QrCodeBase64 = pagseguroResponse.qr_codes[0].qrcode, // A API do PagSeguro retorna o base64 diretamente
                    PixCopiaECola = pagseguroResponse.qr_codes[0].text,
                    ChaveTransacao = pagseguroResponse.id // ID da transação no PagSeguro
                };
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Erro ao criar pagamento no PagSeguro. Status: {response.StatusCode}. Erro: {errorBody}");
            }
        }
    }
}