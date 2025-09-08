// Localização: ApostasApp.Core.Application.Services.Apostadores/ApostadorService.cs
using ApostasApp.Core.Application.DTOs.Apostadores; // Para ApostadorDto
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Application.Services.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Campeonatos; // Para o modelo de domínio ApostadorCampeonato
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Domain.Models.Usuarios; // Para o modelo de domínio Usuario
using AutoMapper; // Necessário para IMapper
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; // Necessário para ClaimTypes
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Apostadores
{
    /// <summary>
    /// ApostadorService é responsável pela lógica de negócio relacionada a apostadores.
    /// Ele herda de BaseService, que gerencia o IUnitOfWork e o INotificador.
    /// </summary>
    public class ApostadorService : BaseService, IApostadorService
    {
        private readonly IApostadorRepository _apostadorRepository;
        private readonly IApostadorCampeonatoRepository _apostadorCampeonatoRepository;
        private readonly ISaldoRepository _saldoRepository;
        private readonly ILogger<ApostadorService> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _uow;

        public ApostadorService(
            IApostadorRepository apostadorRepository,
            IApostadorCampeonatoRepository apostadorCampeonatoRepository,
            ISaldoRepository saldoRepository,
            INotificador notificador,
            IUnitOfWork uow,
            ILogger<ApostadorService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(notificador, uow)
        {
            _apostadorRepository = apostadorRepository;
            _apostadorCampeonatoRepository = apostadorCampeonatoRepository;
            _saldoRepository = saldoRepository;
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _uow = uow;
        }

        /// <summary>
        /// Adiciona um novo apostador.
        /// </summary>
        /// <param name="apostador">A entidade Apostador a ser adicionada.</param>
        public async Task Adicionar(Apostador apostador)
        {
            try
            {
                // CHAMA ExecutarValidacao da BaseService
                //if (!ExecutarValidacao(new ApostadorValidation(), apostador))
                //{
                //    return;
                //}

                await _apostadorRepository.Adicionar(apostador);

                // Criar saldo inicial para o novo apostador
                var saldoInicial = new Saldo(apostador.Id, 0); // Saldo inicial de 0
                _saldoRepository.Adicionar(saldoInicial);

                await CommitAsync(); // CHAMA Commit da BaseService
                Notificar("Sucesso", "Apostador adicionado com sucesso!");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao adicionar APOSTADOR (DbUpdateException).");
                Notificar("Erro", "Ocorreu um erro de banco de dados ao adicionar o apostador. Tente novamente.");
            }
            catch (ObjectDisposedException ex)
            {
                _logger.LogError(ex, "Erro ao adicionar APOSTADOR (ObjectDisposedException).");
                Notificar("Erro", "Ocorreu um erro interno ao adicionar o apostador. Tente novamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar APOSTADOR.");
                Notificar("Erro", $"Ocorreu um erro inesperado ao adicionar o apostador: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza um apostador existente.
        /// </summary>
        /// <param name="apostador">A entidade Apostador a ser atualizada.</param>
        public async Task Atualizar(Apostador apostador)
        {
            try
            {
                // CHAMA ExecutarValidacao da BaseService
                //if (!ExecutarValidacao(new ApostadorValidation(), apostador)) return;

                await _apostadorRepository.Atualizar(apostador);
                await CommitAsync(); // CHAMA Commit da BaseService
                Notificar("Sucesso", "Apostador atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar apostador.");
                Notificar("Erro", $"Ocorreu um erro inesperado ao atualizar o apostador: {ex.Message}");
            }
        }

        /// <summary>
        /// Remove um apostador pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do apostador a ser removido.</param>
        public async Task Remover(Guid id)
        {
            try
            {
                var apostador = await _apostadorRepository.ObterPorId(id);
                if (apostador == null)
                {
                    Notificar("Alerta", "Apostador não encontrado para remoção.");
                    return;
                }

                await _apostadorRepository.Remover(apostador);
                await CommitAsync(); // CHAMA Commit da BaseService
                Notificar("Sucesso", "Apostador removido com sucesso!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover apostador.");
                Notificar("Erro", $"Ocorreu um erro inesperado ao remover o apostador: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém um apostador pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do apostador.</param>
        /// <returns>O apostador encontrado, ou null se não existir.</returns>
        public async Task<Apostador> ObterPorId(Guid id)
        {
            try
            {
                return await _apostadorRepository.ObterPorId(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter apostador por ID.");
                Notificar("Erro", $"Ocorreu um erro inesperado ao obter apostador por ID: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtém todos os apostadores.
        /// </summary>
        /// <returns>Uma coleção de apostadores.</returns>
        public async Task<IEnumerable<Apostador>> ObterTodos()
        {
            try
            {
                return await _apostadorRepository.ObterTodos();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todos os apostadores.");
                Notificar("Erro", $"Ocorreu um erro inesperado ao obter todos os apostadores: {ex.Message}");
                return Enumerable.Empty<Apostador>();
            }
        }

        /// <summary>
        /// Obtém um apostador pelo ID do usuário do Identity associado (UsuarioId),
        /// incluindo suas propriedades de navegação Saldo, Usuario e ApostadoresCampeonatos.
        /// </summary>
        /// <param name="userId">O ID do usuário do Identity.</param>
        /// <returns>A entidade Apostador encontrada, ou null se não existir.</returns>
        public async Task<Apostador> GetApostadorByUserIdAsync(string userId)
        {
            try
            {
                // A propriedade UsuarioId no modelo Apostador é uma string, então a comparação direta é válida.
                return await _apostadorRepository.Buscar(a => a.UsuarioId == userId)
                                                .Include(a => a.Usuario) // Incluir o objeto Usuario para Apelido/Email
                                                .Include(a => a.Saldo) // Incluir o objeto Saldo
                                                .Include(a => a.ApostadoresCampeonatos) // Incluir a coleção de ApostadoresCampeonatos
                                                    .ThenInclude(ac => ac.Campeonato) // Incluir o Campeonato dentro de ApostadoresCampeonatos
                                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter apostador por UserId.");
                Notificar("Erro", $"Ocorreu um erro inesperado ao obter apostador por UserId: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtém os dados completos do apostador logado para exibição no dashboard.
        /// Este método obtém o ID do usuário logado via HttpContextAccessor.
        /// </summary>
        /// <returns>ApiResponse contendo ApostadorDto com saldo e campeonatos aderidos.</returns>
        public async Task<ApiResponse<ApostadorDto>> GetDadosApostador()
        {
            var apiResponse = new ApiResponse<ApostadorDto>(false, null);
            try
            {
                // Obter o ID do usuário autenticado a partir do HttpContext
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
                {
                    Notificar("Erro", "Usuário não autenticado ou ID de usuário não encontrado.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                string userId = userIdClaim.Value;

                // Chamamos o método do serviço que vai fazer a conversão e a busca no repositório
                var apostador = await ObterApostadorPorUsuarioId(userId);

                if (apostador == null)
                {
                    Notificar("Alerta", "Apostador não encontrado para o usuário logado.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                // Mapeia o modelo de domínio para o DTO
                var apostadorDto = _mapper.Map<ApostadorDto>(apostador);

                // Define os dados de sucesso na resposta
                apiResponse.Success = true;
                apiResponse.Data = apostadorDto;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados do apostador para o dashboard.");
                Notificar("Erro", $"Erro interno ao obter dados do apostador: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        // O método auxiliar no serviço, que faz a conversão e chama o repositório
        public async Task<Apostador?> ObterApostadorPorUsuarioId(string userIdString)
        {
            if (string.IsNullOrEmpty(userIdString))
            {
                Notificar("Erro", "ID do usuário não pode ser nulo ou vazio.");
                return null;
            }

            // <<-- AQUI ESTÁ A CONVERSÃO DE STRING PARA GUID NO SERVIÇO -->>
            if (!Guid.TryParse(userIdString, out Guid userIdGuid))
            {
                Notificar("Erro", "ID do usuário inválido.");
                return null;
            }

            // Passamos o Guid para o repositório
            return await _apostadorRepository.ObterApostadorPorUsuarioId(userIdString);
            //return await _apostadorRepository.ObterApostadorPorUsuarioIdAsync(userIdGuid);
        }


        // <<-- NOVA IMPLEMENTAÇÃO DE MÉTODO -->>
        /// <summary>
        /// Obtém o ID do ApostadorCampeonato para um dado usuário e campeonato.
        /// Retorna o ID se encontrado, ou null se não houver adesão.
        /// </summary>
        /// <param name="userId">ID do usuário (Auth).</param>
        /// <param name="campeonatoId">ID do campeonato.</param>
        /// <returns>ApiResponse contendo o ApostadorCampeonatoId (string) ou null.</returns>
        public async Task<ApiResponse<string>> ObterApostadorCampeonatoIdParaUsuarioECampeonato(string userId, Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<string>(false, null);
            try
            {
                _logger.LogInformation($"[ObterApostadorCampeonatoIdParaUsuarioECampeonato] Buscando ApostadorCampeonato para UserId: {userId}, CampeonatoId: {campeonatoId}");

                // 1. Encontre o Apostador associado ao UserId
                var apostador = await _apostadorRepository.ObterApostadorPorUsuarioId(userId);

                if (apostador == null)
                {
                    _logger.LogInformation($"[ObterApostadorCampeonatoIdParaUsuarioECampeonato] Apostador não encontrado para UserId: {userId}.");
                    Notificar("Alerta", "Apostador não encontrado para o usuário fornecido.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse; // Retorna null no Data, indicando que não foi encontrado
                }

                // 2. Agora, busque a adesão específica do Apostador a este Campeonato
                var apostadorCampeonato = await _apostadorCampeonatoRepository.ObterApostadorCampeonatoPorApostadorECampeonato(apostador.Id, campeonatoId);

                //Buscar(ac =>
                //                            ac.ApostadorId == apostador.Id &&
                //                            ac.CampeonatoId == campeonatoId)
                //                            .FirstOrDefaultAsync();

                if (apostadorCampeonato == null)
                {
                    _logger.LogInformation($"[ObterApostadorCampeonatoIdParaUsuarioECampeonato] Nenhuma adesão encontrada para ApostadorId: {apostador.Id}, CampeonatoId: {campeonatoId}.");
                    Notificar("Alerta", "Usuário não aderiu a este campeonato.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    // Data permanece null, indicando que não foi encontrado
                }
                else
                {
                    _logger.LogInformation($"[ObterApostadorCampeonatoIdParaUsuarioECampeonato] Adesão encontrada. ApostadorCampeonatoId: {apostadorCampeonato.Id}");
                    apiResponse.Success = true;
                    apiResponse.Data = apostadorCampeonato.Id.ToString();
                }
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter ApostadorCampeonatoId para UserId: {userId}, CampeonatoId: {campeonatoId}.");
                Notificar("Erro", $"Erro interno ao obter adesão ao campeonato: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }


        public async Task<bool> AtualizarPerfilAsync(string userId, UpdatePerfilRequestDto request)
        {
            // 1. Busca o apostador existente no banco de dados usando o repositório
            var apostador = await _apostadorRepository.ObterApostadorPorUsuarioId(userId);

            if (apostador == null)
            {
                return false; // Apostador não encontrado, retorna falha
            }

            // 2. Atualiza as propriedades com os dados do DTO da requisição
            apostador.Usuario.Apelido = request.Apelido;
            apostador.Usuario.Celular = request.Celular;
            apostador.Usuario.FotoPerfil = request.FotoPerfil;

            // 3. Notifica o Unit of Work / repositório sobre a atualização
            _apostadorRepository.Atualizar(apostador);

            // 4. Salva as alterações no banco de dados
            //return await _uow.SaveChangesAsync(); // Retorna true se houver alterações salvas, false caso contrário
            return await CommitAsync();
                        
        }

    }
}



