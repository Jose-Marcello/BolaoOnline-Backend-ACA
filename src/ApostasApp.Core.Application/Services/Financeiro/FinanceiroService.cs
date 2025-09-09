// Localização: ApostasApp.Core.Application.Services.Financeiro/FinanceiroService.cs

using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Application.Services.Interfaces;
using ApostasApp.Core.Application.Services.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Configuracoes;
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Domain.Models.Notificacoes; // Para NotificationDto
using AutoMapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Financeiro
{
    public class FinanceiroService : BaseService, IFinanceiroService
    {
        private readonly ISaldoRepository _saldoRepository;
        private readonly ITransacaoFinanceiraRepository _transacaoFinanceiraRepository;
        private readonly IApostadorRepository _apostadorRepository;
        private readonly IMapper _mapper;
        //private readonly IPagarMeService _pagarMeService;
        //private readonly IMercadoPagoService _mercadoPagoService;
        //private readonly IPagSeguroService _pagSeguroService;
        private readonly PixSettings _pixSettings;


        public FinanceiroService(
            ISaldoRepository saldoRepository,
            ITransacaoFinanceiraRepository transacaoFinanceiraRepository,
            IApostadorRepository apostadorRepository,
            IUnitOfWork uow,
            INotificador notificador,
            //IMercadoPagoService mercadoPagoService,
            //IPagarMeService pagarMeService,
            //IPagSeguroService pagSeguroService,
            IOptions<PixSettings> pixSettings,
            IMapper mapper) : base(notificador, uow)
        {
            _saldoRepository = saldoRepository;
            _transacaoFinanceiraRepository = transacaoFinanceiraRepository;
            _apostadorRepository = apostadorRepository;
            _mapper = mapper;
            //_pagSeguroService = pagSeguroService;
            _pixSettings = pixSettings.Value;
            //_pagarMeService = pagarMeService;
        }

        /// <summary>
        /// Obtém o saldo atual de um apostador.
        /// </summary>
        /// <param name="apostadorId">O ID do apostador.</param>
        /// <returns>Um ApiResponse com o DTO do saldo atual.</returns>
        public async Task<ApiResponse<SaldoDto>> ObterSaldoAtualAsync(Guid apostadorId)
        {
            var apiResponse = new ApiResponse<SaldoDto>(); // Instancia o ApiResponse<T>
            var saldo = await _saldoRepository.ObterSaldoPorApostadorId(apostadorId);

            if (saldo == null)
            {
                Notificar("Alerta", "Apostador não possui saldo registrado. Saldo inicializado como zero.");
                apiResponse.Success = false;
                apiResponse.Message = "Apostador não possui saldo registrado. Saldo inicializado como zero.";
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Data = default(SaldoDto); // Define o valor padrão para SaldoDto
            }
            else
            {
                apiResponse.Success = true;
                apiResponse.Message = "Saldo obtido com sucesso.";
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Data = _mapper.Map<SaldoDto>(saldo);
            }
            return apiResponse;
        }

      
/// Credita um valor ao saldo de um apostador e registra a transação.
/// </summary>
/// <param name="apostadorId">O ID do apostador.</param>
/// <param name="valor">O valor a ser creditado.</param>
/// <param name="tipoTransacao">O tipo da transação (ex: Deposito).</param>
/// <param name="descricao">A descrição da transação.</param>
/// <returns>Um ApiResponse indicando o sucesso do crédito.</returns>
public async Task<ApiResponse<bool>> CreditarSaldoAsync(Guid apostadorId, decimal valor, TipoTransacao tipoTransacao, string descricao)
        {
            var apiResponse = new ApiResponse<bool>();

            if (valor <= 0)
            {
                Notificar("Erro", "O valor do crédito deve ser maior que zero.");
                apiResponse.Success = false;
                apiResponse.Message = "O valor do crédito deve ser maior que zero.";
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }

            var saldo = await _saldoRepository.ObterSaldoPorApostadorId(apostadorId);

            if (saldo == null)
            {
                saldo = new Saldo(apostadorId, 0);
                _saldoRepository.Adicionar(saldo);
            }

            saldo.Adicionar(valor);
            _saldoRepository.Atualizar(saldo);

            var transacao = new TransacaoFinanceira(
                apostadorId,
                tipoTransacao,
                valor,
                descricao
            );
            transacao.SaldoId = saldo.Id;
            _transacaoFinanceiraRepository.Adicionar(transacao);

            // Agora, e somente agora, a transação é persistida
            // Esta é a parte que estava faltando
            if (await _uow.CommitAsync())
            {
                Notificar("Sucesso", "Crédito e transação salvos com sucesso.");
                apiResponse.Success = true;
                apiResponse.Message = "Depósito realizado com sucesso!";
                apiResponse.Data = true; // Para o front-end saber que a operação foi bem-sucedida
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            else
            {
                Notificar("Erro", "Erro ao salvar o depósito. Tente novamente.");
                apiResponse.Success = false;
                apiResponse.Message = "Erro ao processar o depósito.";
                apiResponse.Data = false;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }



        /// <summary>
        /// Debita um valor do saldo de um apostador e registra a transação.
        /// </summary>
        /// <param name="apostadorId">O ID do apostador.</param>
        /// <param name="valor">O valor a ser debitado.</param>
        /// <param name="tipoTransacao">O tipo da transação (ex: AdesaoCampeonato).</param>
        /// <param name="descricao">A descrição da transação.</param>
        /// <returns>Um ApiResponse indicando o sucesso do débito.</returns>
        public async Task<ApiResponse<bool>> DebitarSaldoAsync(Guid apostadorId, decimal valor, TipoTransacao tipoTransacao, string descricao)
        {
            var apiResponse = new ApiResponse<bool>(); // Instancia o ApiResponse<bool>

            if (valor <= 0)
            {
                Notificar("Erro", "O valor do débito deve ser maior que zero.");
                apiResponse.Success = false;
                apiResponse.Message = "O valor do débito deve ser maior que zero.";
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Data = false;
                return apiResponse;
            }

            var saldo = await _saldoRepository.ObterSaldoPorApostadorId(apostadorId);

            if (saldo == null)
            {
                Notificar("Erro", "Apostador não possui saldo para debitar. Por favor, deposite um valor antes.");
                apiResponse.Success = false;
                apiResponse.Message = "Apostador não possui saldo para debitar.";
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Data = false;
                return apiResponse;
            }

            var debitoEfetuado = saldo.Debitar(valor);

            if (!debitoEfetuado)
            {
                Notificar("Alerta", "Saldo insuficiente para realizar o débito.");
                apiResponse.Success = false;
                apiResponse.Message = "Saldo insuficiente para realizar o débito.";
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Data = false;
                return apiResponse;
            }

            _saldoRepository.Atualizar(saldo);
            var transacao = new TransacaoFinanceira(
                apostadorId,
                tipoTransacao,
                -valor,
                descricao
            );
            transacao.SaldoId = saldo.Id;
            _transacaoFinanceiraRepository.Adicionar(transacao);

            Notificar("Sucesso", "Débito e transação preparados para persistência.");
            apiResponse.Success = true;
            apiResponse.Message = "Débito e transação preparados para persistência.";
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            apiResponse.Data = true;
            return apiResponse;
        }

        /// <summary>
        /// Credita um valor ao saldo de um apostador e registra a transação.
        /// </summary>
        /// <param name="apostadorId">O ID do apostador.</param>
        /// <param name="valor">O valor a ser creditado.</param>
        /// <param name="tipoTransacao">O tipo da transação (ex: Deposito).</param>
        /// <param name="descricao">A descrição da transação.</param>
        /// <returns>Um ApiResponse indicando o sucesso do crédito.</returns>

        // Em FinanceiroService.cs
        //PIX DIRETO

        public async Task<ApiResponse<PixResponseDto>> GerarPixParaDepositoAsync(DepositarRequestDto request)
        {
            var apiResponse = new ApiResponse<PixResponseDto>();

            try
            {
                // PASSO 1: Gerar os dados do PIX.
                // Se a lógica aqui falhar, o banco de dados não será afetado.
                var pixResponse = new PixResponseDto
                {
                    QrCodeBase64 = _pixSettings.QrCodeBase64,
                    PixCopiaECola = _pixSettings.ChavePixCopiaECola,
                    ChaveTransacao = Guid.NewGuid().ToString() // Gerar a referência única aqui
                };

                // PASSO 2: Agora que a geração do PIX foi bem-sucedida, criar a transação no banco de dados.
                // Buscar o saldo do apostador.
                var saldo = await _saldoRepository.ObterSaldoPorApostadorId(request.ApostadorId);

                // Se o saldo não existir, crie-o.
                if (saldo == null)
                {
                    saldo = new Saldo(request.ApostadorId, 0);
                    _saldoRepository.Adicionar(saldo);
                }

                // Criar a transação PENDENTE e vincular ao saldo.
                var transacao = new TransacaoFinanceira(
                    request.ApostadorId,
                    TipoTransacao.CreditoPix,
                    request.Valor,
                    "Depósito via PIX - Aguardando confirmação manual"
                );
                transacao.ExternalReference = pixResponse.ChaveTransacao;
                transacao.Status = "Aguardando Pagamento";
                transacao.SaldoId = saldo.Id; // <-- ATRIBUIÇÃO CORRETA

                _transacaoFinanceiraRepository.Adicionar(transacao);
                await _uow.CommitAsync();

                // Retornar a resposta de sucesso com os dados do PIX.
                apiResponse.Success = true;
                apiResponse.Message = "Dados do PIX gerados. Aguardando pagamento.";
                apiResponse.Data = pixResponse;
            }
            catch (Exception ex)
            {
                // Se algo na lógica acima falhar, nenhuma alteração será salva no banco de dados.
                Notificar("Erro", $"Falha ao gerar dados do PIX: {ex.Message}");
                apiResponse.Success = false;
                apiResponse.Message = "Erro ao processar o depósito. Tente novamente.";
            }

            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }


        public async Task<ApiResponse<bool>> CreditarSaldoViaWebhookAsync(string externalReference, decimal valor)
        {
            var apiResponse = new ApiResponse<bool>();

            // Primeiro, encontre a transação pendente usando a referência externa (a chave do PIX)
            var transacao = await _transacaoFinanceiraRepository.ObterPorReferenciaExterna(externalReference);

            if (transacao == null)
            {
                Notificar("Erro", "Transação não encontrada para a referência informada.");
                apiResponse.Success = false;
                apiResponse.Message = "Transação não encontrada.";
                return apiResponse;
            }

            if (transacao.Status == "Aguardando Pagamento")
            {
                // Agora, encontre o saldo do apostador
                // A TransacaoFinanceira já possui o SaldoId, que está associado ao ApostadorId.
                var saldo = await _saldoRepository.ObterPorId(transacao.SaldoId);

                if (saldo == null)
                {
                    // O apostador não tem um saldo, isso pode ser um caso de erro ou precisa ser criado
                    Notificar("Erro", "Saldo do apostador não encontrado.");
                    apiResponse.Success = false;
                    apiResponse.Message = "Erro ao creditar saldo.";
                    return apiResponse;
                }

                // 1. Atualize o saldo
                saldo.Adicionar(valor);
                _saldoRepository.Atualizar(saldo);

                // 2. Atualize o status da transação
                transacao.Status = "Concluído";
                _transacaoFinanceiraRepository.Atualizar(transacao);

                // 3. Salve tudo na mesma unidade de trabalho
                if (await _uow.CommitAsync())
                {
                    Notificar("Sucesso", "Saldo e transação atualizados com sucesso.");
                    apiResponse.Success = true;
                    apiResponse.Message = "Saldo creditado e transação confirmada.";
                    return apiResponse;
                }
            }
            else
            {
                Notificar("Erro", "A transação já foi processada.");
                apiResponse.Success = false;
                apiResponse.Message = "Transação já foi processada.";
            }

            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }


        // Em FinanceiroService.cs
        public async Task<ApiResponse<DepositoPixDto>> EfetuarDepositoPixAsync(DepositoPixDto deposito)
        {
            var apiResponse = new ApiResponse<DepositoPixDto>();

            try
            {
                // 1. Cria a transação usando o construtor correto para a sua classe
                var transacao = new TransacaoFinanceira(
                    deposito.ApostadorId,
                    TipoTransacao.CreditoPix,
                    deposito.Valor,
                    "Depósito via PIX - Aguardando pagamento"
                );

                // Atribui o Status diretamente, sem o método SetDataConfirmacao
                transacao.Status = "Concluído";

                _transacaoFinanceiraRepository.Adicionar(transacao);

                // 2. Atualiza o saldo do apostador
                var saldo = await _saldoRepository.ObterSaldoPorApostadorId(deposito.ApostadorId);

                if (saldo == null)
                {
                    saldo = new Saldo(deposito.ApostadorId, deposito.Valor);
                    _saldoRepository.Adicionar(saldo);
                }
                else
                {
                    saldo.Adicionar(deposito.Valor);
                    _saldoRepository.Atualizar(saldo);
                }

                // 3. Salva no banco de dados
                if (await _uow.CommitAsync())
                {
                    apiResponse.Data = deposito;
                    apiResponse.Message = "Depósito Pix (Teste) criado e saldo atualizado com sucesso.";
                    apiResponse.Success = true;
                }
                else
                {
                    Notificar("Erro", "Erro ao processar o depósito.");
                    apiResponse.Success = false;
                }
            }
            catch (Exception ex)
            {
                Notificar("Erro", $"Falha ao gerar dados do PIX: {ex.Message}");
                apiResponse.Success = false;
                apiResponse.Message = "Erro ao processar o depósito. Tente novamente.";
            }

            return apiResponse;
        }

        public Task<ApiResponse<SimulaPixResponseDto>> GerarPixSimuladoParaDepositoAsync(DepositarRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}


// Em FinanceiroService.cs
/*PAGSEGURO/***
 * 
public async Task<ApiResponse<PixResponseDto>> GerarPixParaDepositoAsync(DepositarRequestDto request)
{
    var apiResponse = new ApiResponse<PixResponseDto>();
    var externalReference = Guid.NewGuid().ToString();

    try
    {
        // 1. Cria a transação no seu banco de dados
        var transacao = new TransacaoFinanceira(
            request.ApostadorId,
            TipoTransacao.CreditoPix,
            request.Valor,
            "Depósito via PIX - Aguardando confirmação manual"
        );
        transacao.ExternalReference = externalReference;
        transacao.Status = "Aguardando Pagamento";

        // **Ajuste:** Não é mais necessário obter o Saldo para gerar a transação
        // O saldo só será alterado quando o pagamento for confirmado manualmente

        _transacaoFinanceiraRepository.Adicionar(transacao);

        await _uow.CommitAsync();

        // 2. Retorna os dados do PIX estático para o frontend
        var pixResponse = new PixResponseDto
        {
            QrCodeBase64 = _pixSettings.QrCodeBase64, // Pega do appsettings
            PixCopiaECola = _pixSettings.ChavePixCopiaECola, // Pega do appsettings
            ChaveTransacao = externalReference // Referência única para a sua transação
        };

        apiResponse.Success = true;
        apiResponse.Message = "Dados do PIX gerados. Aguardando pagamento.";
        apiResponse.Data = pixResponse;
    }
    catch (Exception ex)
    {
        Notificar("Erro", $"Falha ao gerar dados do PIX: {ex.Message}");
        apiResponse.Success = false;
        apiResponse.Message = "Erro ao processar o depósito. Tente novamente.";
    }

    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
    return apiResponse;
}



// FinanceiroService.cs
public async Task<ApiResponse<PixResponseDto>> GerarPixParaDepositoAsync(DepositarRequestDto request)
{
    var apiResponse = new ApiResponse<PixResponseDto>();
    var externalReference = Guid.NewGuid().ToString();

    try
    {
        // 1. **NOVO: Obtenha o Saldo do apostador**
        var saldo = await _saldoRepository.ObterSaldoPorApostadorId(request.ApostadorId);
        if (saldo == null)
        {
            // Tratar o caso em que o apostador não tem saldo
            Notificar("Erro", "Apostador não tem saldo. Não é possível gerar a transação.");
            apiResponse.Success = false;
            apiResponse.Message = "Apostador não tem saldo. Não é possível gerar a transação.";
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        // 2. Chama o serviço que se comunica com o gate de Pagamentos (atual:PagSeguro)
        var pixResponse = await _pagSeguroService.CriarPagamentoPixAsync(
            request.Valor,
            "Depósito na plataforma",
            externalReference);

        // 3. Salva a transação no seu banco de dados
        var transacao = new TransacaoFinanceira(
            request.ApostadorId,
            TipoTransacao.CreditoPix,
            request.Valor,
            "Depósito via PIX - Aguardando pagamento"
        );
        transacao.ExternalReference = externalReference;
        transacao.Status = "Aguardando Pagamento";

        // **NOVO: Atribua o SaldoId aqui!**
        transacao.SaldoId = saldo.Id;

        _transacaoFinanceiraRepository.Adicionar(transacao);

        await _uow.CommitAsync();

        apiResponse.Success = true;
        apiResponse.Message = "Dados do PIX gerados com sucesso. Aguardando pagamento.";
        apiResponse.Data = pixResponse;
    }
    catch (HttpRequestException ex)
    {
        Notificar("Erro", $"Falha ao gerar PIX com o PagSegur: {ex.Message}");
        apiResponse.Success = false;
        apiResponse.Message = "Erro ao processar o depósito. Tente novamente.";
    }

    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
    return apiResponse;
}
}
}*/
// ***** PIX SIMULADO  *******
/*  
public async Task<ApiResponse<PixResponseDto>> GerarPixParaDepositoAsync(DepositarRequestDto request)
{
    // Lógica para gerar a transação PIX.
    // Em um cenário real, você faria uma chamada HTTP para a API de um PSP (Mercado Pago, etc.).

    // Aqui, vamos apenas simular a resposta para que o front-end possa ser testado.
    await Task.Delay(1500); // Simula o tempo de latência da chamada externa.

    var pixResponse = new PixResponseDto
    {
        // Esses dados seriam gerados pela API do PSP.
        // O QR Code pode ser gerado a partir de um texto. Usamos um exemplo aqui.
        // Uma string de base64 de um QR Code válido para o texto "Hello, World!"

        QrCodeBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAZAAAAGQAQMAAAC6caSPAAAABlBMVEX///8AAABVwtN+AAAACXBIWXMAAA7EAAAOxAGVKw4bAAADZElEQVR4nO2bvZWsMAyFNWcCQkqgFEqD0iiFEggJOOhZ98qG3X2ba1gpgRnrI5El6wdEUh4jk1IWkTcuvZ6iy6B2MTlNZ+tc7UgkLrLQokXpwF1fLqqrmNI8QHcet46LXSKRERjaSZFh47VXIuVWZ+iW31siD0WGvfi+tA3T76ZbkJf9mcgnIXD2Yn0E7rIJuhMkovmSyEcg8NvL+pCxWP+l5sHF+k33R+RP5NMRpRiCRGzzi22Y2S/j/1O4REIhl9D3S4bdcmq3vvwqiURCJq7a5Wir9h8c2aN5+fXW4ux2TCfyKGS1TTH7hhmxV74d4r6oiYRGXMnkoNm5Cbhaa2SzviEM44mERKaFGdMIXdjbFgetVZIgtRbUvb5hEomKoPto/gx7o0oqD9ivutfMfuXQiTwHKUrm1/NI31/Y7liwYk/aW+dKrJ11JBIWQcH7QkeDfQ69fN+kHsUM4833EwmIVA+GgWF9nsh69bEm9C7aJkjkMYjLpGxljKv7PlvQGBLaIjaM1l2USEhkxP0sg97it/k+m5DdeZl91Hs2nkg0xAQ5dYcpPRGTk/OD1oS03kUiz0Im6jZBC1qkjXZN2LLCcxIJjCxwep8O1tS6HNOdk3Y+Iz4Y3CUSFylncMcwvr89tfYJr7a3bjynrv3kRIIiRZdVUmthIEKzPGIfq0oiT0SwU4pfv+5jQU+tS1pW3+OwTnQikRHxdytMGuLdRwyGSnzYbGvUFnQiUZGBHWTmycWD/U2azp/SqqT7hknkGQimCWcdKRWV7d1aVp5a+1hQ6qwwkaCIncFi2TOi+bj69wYICDN7kWhT/+xbJhILacnVwCk9C95ROdr1YkkwTUjkccikLKxKGL+mCbZhTHdafaf0rKOb7ycSEVkYxu0ohu/Xj792nw+d4iHg/tZNIiEReLA5qevWLrOVR9qGChbNb98mJBIWUVrfEbvo1VZ2mVs+lshDkCYD3Rv3bSxY30/2t+hus6REwiGTm7Y5PT/3ujpXLJ26o/6ZSFiE9vY+4+XBQKyBha6WT/z6RJ6GDPyKc/g6FuQqSXtNUvXWuUokNsJ2h345ilcRj9+KQW8iH4LAkV23lkeM5u7l3x05kY9HpJK/jwU9jI+bJBIYUYoXSwu/rb4dxe1TsOtETiQkkvJ35R+zrnGRObnCEAAAAABJRU5ErkJggg==",
        PixCopiaECola = "00020126580014br.gov.bcb.pix013621999734777-6268-08dd8c07d71c5204000053039865802BR5911Zé Marcello6007MARICÁ61081234567862070503***6304C74B",
        ChaveTransacao = Guid.NewGuid().ToString()
    };

    // Não registramos a transação financeira neste momento!
    // A transação só é registrada quando o webhook do PSP confirma o pagamento.

    Notificar("Sucesso", "Dados do PIX gerados com sucesso. Aguardando pagamento.");

    return new ApiResponse<PixResponseDto>
    {
        Success = true,
        Message = "Dados do PIX gerados com sucesso. Aguardando pagamento.",
        Data = pixResponse,
        Notifications = ObterNotificacoesParaResposta().ToList()
    };
}
}
*/



