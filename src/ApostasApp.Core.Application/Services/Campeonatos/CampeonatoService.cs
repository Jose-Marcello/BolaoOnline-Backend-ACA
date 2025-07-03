// Localização: ApostasApp.Core.Application.Services.Campeonatos/CampeonatoService.cs
using ApostasApp.Core.Application.DTOs.Campeonatos;
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.Services.Base;
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos;
using ApostasApp.Core.Application.Services.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Campeonatos; // Importado para usar o modelo de domínio Campeonato
using ApostasApp.Core.Domain.Models.Financeiro; // Para TipoTransacao
using ApostasApp.Core.Domain.Models.Notificacoes; // Para Notificacao (entidade de domínio)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace ApostasApp.Core.Application.Services.Campeonatos
{
    public class CampeonatoService : BaseService, ICampeonatoService
    {
        private readonly ICampeonatoRepository _campeonatoRepository;
        private readonly IApostadorRepository _apostadorRepository;
        private readonly IApostadorCampeonatoRepository _apostadorCampeonatoRepository;
        private readonly IFinanceiroService _financeiroService;
        private readonly IMapper _mapper;

        public CampeonatoService(
            ICampeonatoRepository campeonatoRepository,
            IApostadorRepository apostadorRepository,
            IApostadorCampeonatoRepository apostadorCampeonatoRepository,
            IFinanceiroService financeiroService,
            IUnitOfWork uow,
            INotificador notificador,
            IMapper mapper) : base(notificador, uow)
        {
            _campeonatoRepository = campeonatoRepository;
            _apostadorRepository = apostadorRepository;
            _apostadorCampeonatoRepository = apostadorCampeonatoRepository;
            _financeiroService = financeiroService;
            _mapper = mapper;
        }

        // <<-- CORRIGIDO: Implementação dos métodos da interface -->>

        public async Task<bool> Adicionar(CampeonatoDto campeonatoDto)
        {
            var campeonato = _mapper.Map<Campeonato>(campeonatoDto);
            _campeonatoRepository.Adicionar(campeonato);
            return await CommitAsync();
        }

        public async Task<bool> Atualizar(CampeonatoDto campeonatoDto)
        {
            var campeonato = _mapper.Map<Campeonato>(campeonatoDto);
            _campeonatoRepository.Atualizar(campeonato);
            return await CommitAsync();
        }

        public async Task<bool> Remover(Campeonato campeonato) //Guid id)
        {
            _campeonatoRepository.Remover(campeonato);
            return await CommitAsync();
        }

        public async Task<CampeonatoDto> ObterPorId(Guid id)
        {
            var campeonato = await _campeonatoRepository.ObterPorId(id);
            return _mapper.Map<CampeonatoDto>(campeonato);
        }

        public async Task<IEnumerable<CampeonatoDto>> ObterTodos()
        {
            var campeonatos = await _campeonatoRepository.ObterTodos();
            return _mapper.Map<IEnumerable<CampeonatoDto>>(campeonatos);
        }

        // <<-- FIM DA CORREÇÃO DOS MÉTODOS DA INTERFACE -->>


        public async Task<ApiResponse<IEnumerable<CampeonatoDto>>> GetAvailableCampeonatos(string? userId)
        {
            var apiResponse = new ApiResponse<IEnumerable<CampeonatoDto>>(false, null);
            HashSet<string> campeonatosAderidosIds = new HashSet<string>();

            if (!string.IsNullOrEmpty(userId))
            {
                var adesoesDoUsuario = await _apostadorCampeonatoRepository.ObterAdesoesPorUsuarioIdAsync(userId);
                campeonatosAderidosIds = new HashSet<string>(
                    adesoesDoUsuario.Select(ac => ac.CampeonatoId.ToString())
                );
            }

            var todosCampeonatos = await _campeonatoRepository.ObterListaDeCampeonatosAtivos();

            var campeonatosDto = todosCampeonatos.Select(c => new CampeonatoDto
            {
                Id = c.Id.ToString(),
                Nome = c.Nome,
                DataInicio = c.DataInic,
                DataFim = c.DataFim,
                NumRodadas = c.NumRodadas,
                Tipo = c.Tipo.ToString(),
                Ativo = c.Ativo,
                CustoAdesao = c.CustoAdesao.HasValue ? (decimal)c.CustoAdesao.Value : 0m,
                AderidoPeloUsuario = !string.IsNullOrEmpty(userId) && campeonatosAderidosIds.Contains(c.Id.ToString())
            }).ToList();

            if (!campeonatosDto.Any())
            {
                Notificar("CAMPEONATOS_NAO_ENCONTRADOS", "Alerta", "Nenhum campeonato disponível encontrado na base de dados.");
                apiResponse.Success = true;
                apiResponse.Data = new List<CampeonatoDto>();
            }
            else
            {
                apiResponse.Success = true;
                apiResponse.Data = campeonatosDto;
            }
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> AderirCampeonatoAsync(Guid apostadorId, Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<bool>(false, false);

            var campeonato = await _campeonatoRepository.ObterPorId(campeonatoId);
            if (campeonato == null)
            {
                Notificar("CAMPEONATO_NAO_ENCONTRADO_ADESAO", "Erro", "Campeonato não encontrado.");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }

            var adesaoExistente = await _apostadorCampeonatoRepository.ObterApostadorDoCampeonato(campeonatoId, apostadorId);
            if (adesaoExistente != null)
            {
                Notificar("JA_ADERIU_CAMPEONATO", "Alerta", "Você já aderiu a este campeonato.");
                apiResponse.Success = true;
                apiResponse.Data = true;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }

            var apostador = await _apostadorRepository.ObterPorId(apostadorId);
            if (apostador == null)
            {
                Notificar("APOSTADOR_NAO_ENCONTRADO_ADESAO", "Erro", "Apostador não encontrado.");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }

            if (campeonato.CustoAdesao.HasValue && campeonato.CustoAdesao.Value > 0)
            {
                var debitoResponse = await _financeiroService.DebitarSaldoAsync(
                    apostadorId,
                    campeonato.CustoAdesao.Value,
                    TipoTransacao.AdesaoCampeonato,
                    $"Adesão ao campeonato: {campeonato.Nome}");

                if (!debitoResponse.Success)
                {
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
            }

            var novaAdesao = new ApostadorCampeonato(apostadorId, campeonato.Id)
            {
                DataInscricao = DateTime.Now,
                CustoAdesaoPago = campeonato.CustoAdesao.HasValue && campeonato.CustoAdesao.Value > 0
            };

            _apostadorCampeonatoRepository.Adicionar(novaAdesao);

            var saved = await CommitAsync();

            if (saved)
            {
                apiResponse.Success = true;
                apiResponse.Data = true;
                Notificar("ADESAO_SUCESSO", "Sucesso", "Adesão ao campeonato realizada com sucesso!");
            }
            else
            {
                Notificar("ADESAO_FALHA", "Erro", "Não foi possível vincular o apostador ao campeonato.");
            }
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        public async Task<ApiResponse<CampeonatoDto?>> GetDetalhesCampeonato(Guid id)
        {
            var apiResponse = new ApiResponse<CampeonatoDto?>(false, null);
            var campeonato = await _campeonatoRepository.ObterPorId(id);

            if (campeonato == null)
            {
                Notificar("CAMPEONATO_DETALHES_NAO_ENCONTRADO", "Alerta", $"Campeonato com ID '{id}' não encontrado.");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }

            apiResponse.Success = true;
            apiResponse.Data = _mapper.Map<CampeonatoDto>(campeonato); // Usando o mapper para mapear a entidade para DTO
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }
    }
}
