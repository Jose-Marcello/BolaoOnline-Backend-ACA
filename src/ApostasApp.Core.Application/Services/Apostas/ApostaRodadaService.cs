using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.ApostasRodada;
using ApostasApp.Core.Application.DTOs.Jogos;
using ApostasApp.Core.Application.DTOs.Palpites;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Application.Services.Interfaces.Apostas;
using ApostasApp.Core.Application.Services.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Interfaces.Apostas;
using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Interfaces.Jogos;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Domain.Models.Interfaces.Rodadas;
using ApostasApp.Core.Domain.Models.Rodadas;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Apostas
{
  public class ApostaRodadaService : BaseService, IApostaRodadaService
  {
    private readonly IApostaRodadaRepository _apostaRodadaRepository;
    private readonly ICampeonatoRepository _campeonatoRepository;
    private readonly IApostadorRepository _apostadorRepository;
    private readonly IPalpiteRepository _palpiteRepository;
    private readonly IRodadaRepository _rodadaRepository;
    private readonly IJogoRepository _jogoRepository;
    private readonly IApostadorCampeonatoRepository _apostadorCampeonatoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ApostaRodadaService> _logger;
    private readonly IFinanceiroService _financeiroService;

    public ApostaRodadaService(
        IFinanceiroService financeiroService,
        IApostaRodadaRepository apostaRodadaRepository,
        ICampeonatoRepository campeonatoRepository,
        IApostadorRepository apostadorRepository,
        IPalpiteRepository palpiteRepository,
        IRodadaRepository rodadaRepository,
        IJogoRepository jogoRepository,
        IApostadorCampeonatoRepository apostadorCampeonatoRepository,
        IMapper mapper,
        INotificador notificador,
        IUnitOfWork uow,
        ILogger<ApostaRodadaService> logger) : base(notificador, uow)
    {
      _financeiroService = financeiroService;
      _apostaRodadaRepository = apostaRodadaRepository;
      _campeonatoRepository = campeonatoRepository;
      _apostadorRepository = apostadorRepository;
      _palpiteRepository = palpiteRepository;
      _rodadaRepository = rodadaRepository;
      _jogoRepository = jogoRepository;
      _apostadorCampeonatoRepository = apostadorCampeonatoRepository;
      _mapper = mapper;
      _logger = logger;
    }

    // --- GRID 2: CONSULTA PARA EDICAO ---
    public async Task<ApiResponse<IEnumerable<ApostaJogoEdicaoDto>>> ObterApostasDoApostadorNaRodadaParaEdicao(Guid rodadaId, Guid apostaRodadaId)
    {
      var apiResponse = new ApiResponse<IEnumerable<ApostaJogoEdicaoDto>>(false, null);
      try
      {
        var apostaRodada = await _apostaRodadaRepository.Buscar(ar => ar.Id == apostaRodadaId)
            .Include(ar => ar.Palpites)
            .FirstOrDefaultAsync();

        var jogosDaRodada = await _jogoRepository.ObterJogosDaRodadaComPlacaresEEquipes(rodadaId);

        var apostasParaEdicao = jogosDaRodada.Select(jogo => {
          var palpite = apostaRodada?.Palpites.FirstOrDefault(p => p.JogoId == jogo.Id);
          return new ApostaJogoEdicaoDto
          {
            Id = palpite?.Id.ToString() ?? Guid.NewGuid().ToString(),
            IdJogo = jogo.Id.ToString(),
            EquipeMandante = jogo.EquipeCasa?.Equipe?.Nome ?? "N/A",
            SiglaMandante = jogo.EquipeCasa?.Equipe?.Sigla ?? "??",
            EscudoMandante = jogo.EquipeCasa?.Equipe?.Escudo,
            EquipeVisitante = jogo.EquipeVisitante?.Equipe?.Nome ?? "N/A",
            SiglaVisitante = jogo.EquipeVisitante?.Equipe?.Sigla ?? "??",
            EscudoVisitante = jogo.EquipeVisitante?.Equipe?.Escudo,
            EstadioNome = jogo.Estadio?.Nome,
            DataJogo = jogo.DataJogo.ToString("yyyy-MM-dd"),
            HoraJogo = jogo.HoraJogo.ToString(@"hh\:mm"),
            StatusJogo = jogo.Status.ToString(),
            PlacarApostaCasa = palpite?.PlacarApostaCasa,
            PlacarApostaVisita = palpite?.PlacarApostaVisita,
            Enviada = apostaRodada?.Enviada ?? false
          };
        }).OrderBy(x => x.DataJogo).ThenBy(x => x.HoraJogo).ToList();

        apiResponse.Data = apostasParaEdicao;
        apiResponse.Success = true;
        return apiResponse;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao carregar edição.");
        return apiResponse;
      }
    }

    // --- GRID 3: CONSULTA DE RESULTADOS ---
    public async Task<ApiResponse<ApostaRodadaResultadosDto>> ObterResultadosDaRodada(Guid rodadaId, Guid apostaRodadaId)
    {
      var apiResponse = new ApiResponse<ApostaRodadaResultadosDto>(false, null);
      try
      {
        var jogos = await _jogoRepository.Buscar(j => j.RodadaId == rodadaId)
            .Include(j => j.EquipeCasa).ThenInclude(ec => ec.Equipe)
            .Include(j => j.EquipeVisitante).ThenInclude(ev => ev.Equipe)
            .ToListAsync();

        var aposta = await _apostaRodadaRepository.Buscar(ar => ar.Id == apostaRodadaId)
            .Include(ar => ar.Palpites)
            .FirstOrDefaultAsync();

        apiResponse.Data = new ApostaRodadaResultadosDto
        {
          ApostaRodadaId = aposta?.Id.ToString(),
          PontuacaoTotalRodada = aposta?.PontuacaoTotalRodada ?? 0,
          JogosComResultados = jogos.Select(j => {
            var palpite = aposta?.Palpites.FirstOrDefault(p => p.JogoId == j.Id);
            return new ApostaJogoResultadosDto
            {
              IdJogo = j.Id.ToString(),
              EquipeMandante = j.EquipeCasa?.Equipe?.Nome,
              EquipeVisitante = j.EquipeVisitante?.Equipe?.Nome,
              PlacarRealCasa = j.PlacarCasa,
              PlacarRealVisita = j.PlacarVisita,
              PlacarApostaCasa = palpite?.PlacarApostaCasa,
              PlacarApostaVisita = palpite?.PlacarApostaVisita,
              Pontuacao = palpite?.Pontos ?? 0
            };
          }).ToList()
        };
        apiResponse.Success = true;
        return apiResponse;
      }
      catch (Exception ex) { return apiResponse; }
    }

    // --- CORE: SALVAR E TRANSAÇÕES ---
    public async Task<ApiResponse<ApostaRodadaDto>> SalvarApostas(SalvarApostaRequestDto salvarApostaDto)
    {
      var apiResponse = new ApiResponse<ApostaRodadaDto>(false, null);
      try
      {
        var apostaRodadaId = Guid.Parse(salvarApostaDto.Id); // Correção ID
        var apostaRodada = await _apostaRodadaRepository.Buscar(ar => ar.Id == apostaRodadaId).Include(ar => ar.Palpites).FirstOrDefaultAsync();

        if (apostaRodada != null)
        {
          apostaRodada.Enviada = true;
          apostaRodada.DataHoraSubmissao = DateTime.UtcNow;

          foreach (var pDto in salvarApostaDto.ApostasJogos)
          {
            var palpite = apostaRodada.Palpites.FirstOrDefault(p => p.JogoId == Guid.Parse(pDto.JogoId));
            if (palpite != null)
            {
              palpite.PlacarApostaCasa = pDto.PlacarCasa;
              palpite.PlacarApostaVisita = pDto.PlacarVisitante;
            }
          }
          if (await CommitAsync())
          {
            apiResponse.Data = _mapper.Map<ApostaRodadaDto>(apostaRodada);
            apiResponse.Success = true;
          }
        }
        return apiResponse;
      }
      catch (Exception) { return apiResponse; }
    }

    public async Task<ApiResponse<ApostaRodadaDto>> ExecutarTransacaoApostaAvulsa(CriarApostaAvulsaRequestDto requestDto)
    {
      var apostador = await _apostadorRepository.ObterPorIdComSaldo(Guid.Parse(requestDto.ApostadorId));
      if (apostador.Saldo.Valor < requestDto.CustoAposta)
      {
        Notificar("Erro", "Saldo insuficiente.");
        return new ApiResponse<ApostaRodadaDto>(false, null);
      }

      await _financeiroService.DebitarSaldoAsync(apostador.Id, requestDto.CustoAposta, TipoTransacao.ApostaRodada, "Aposta Avulsa");

      var ac = await _apostadorCampeonatoRepository.Buscar(x => x.ApostadorId == apostador.Id && x.CampeonatoId == Guid.Parse(requestDto.CampeonatoId)).FirstOrDefaultAsync();

      return await GerarApostaRodada(ac.Id.ToString(), requestDto.RodadaId, false, "Aposta Avulsa");
    }


    public async Task<ApiResponse<IEnumerable<ApostaRodadaDto>>> ObterApostasRodadaPorApostador(Guid rodadaId, Guid? apostadorCampeonatoId)
    {
      var apiResponse = new ApiResponse<IEnumerable<ApostaRodadaDto>>(false, null);
      try
      {
        // 1. Buscamos as apostas existentes
        var apostas = await _apostaRodadaRepository.ObterApostasComDetalhes(rodadaId, apostadorCampeonatoId ?? Guid.Empty);

        // 2. BUSCA ESSENCIAL: Buscamos os dados da Rodada para preencher os campos Numero, DataInicio, etc.
        // Se não buscarmos a Rodada, os campos de cabeçalho no DTO ficam zerados/vazios.
        var rodada = await _rodadaRepository.ObterPorId(rodadaId);

        var dtos = new List<ApostaRodadaDto>();

        if (apostas != null && apostas.Any())
        {
          foreach (var aposta in apostas)
          {
            var dto = _mapper.Map<ApostaRodadaDto>(aposta);

            // Garantimos que os dados da rodada pai sejam repassados para o DTO
            if (rodada != null)
            {
              dto.NumeroRodada = rodada.NumeroRodada;
              dto.DataInicio = rodada.DataInic;
              dto.DataFim = rodada.DataFim;
              dto.DescricaoRodada = $"Rodada {rodada.NumeroRodada}";
            }
            dtos.Add(dto);
          }
        }
        else if (rodada != null)
        {
          // Se não tem aposta ainda, mandamos um DTO vazio apenas com os dados da rodada
          // Isso permite que o Grid 1 mostre "Rodada X" mesmo sem apostas.
          dtos.Add(new ApostaRodadaDto
          {
            RodadaId = rodada.Id.ToString(),
            NumeroRodada = rodada.NumeroRodada,
            DataInicio = rodada.DataInic,
            DataFim = rodada.DataFim,
            DescricaoRodada = $"Rodada {rodada.NumeroRodada}"
          });
        }

        apiResponse.Data = dtos;
        apiResponse.Success = true;
        return apiResponse;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao obter dados da rodada para o Grid 1");
        apiResponse.Message = "Erro ao carregar dados da rodada.";
        return apiResponse;
      }
    }




    public async Task<ApiResponse<ApostaRodadaDto>> GerarApostaRodada(string acId, string rId, bool ehCamp, string ident)
    {
      var jogos = await _jogoRepository.ObterJogosDaRodadaComPlacaresEEquipes(Guid.Parse(rId));
      var novaAposta = new ApostaRodada { ApostadorCampeonatoId = Guid.Parse(acId), RodadaId = Guid.Parse(rId), EhApostaCampeonato = ehCamp, IdentificadorAposta = ident, DataCriacao = DateTime.Now };

      await _apostaRodadaRepository.Adicionar(novaAposta);
      var palpites = jogos.Select(j => new Palpite { ApostaRodadaId = novaAposta.Id, JogoId = j.Id }).ToList();
      await _palpiteRepository.AdicionarRange(palpites);

      if (await CommitAsync()) return new ApiResponse<ApostaRodadaDto> { Success = true, Data = _mapper.Map<ApostaRodadaDto>(novaAposta) };
      return new ApiResponse<ApostaRodadaDto>(false, null);
    }

    // --- RESTANTE DOS MÉTODOS ---
    public async Task<ApiResponse<IEnumerable<ApostaRodadaDto>>> ObterApostasRodadaPorApostador(Guid rId, Guid acId)
    {
      var lista = await _apostaRodadaRepository.ObterApostasComDetalhes(rId, acId);
      return new ApiResponse<IEnumerable<ApostaRodadaDto>> { Success = true, Data = _mapper.Map<IEnumerable<ApostaRodadaDto>>(lista) };
    }

    public async Task<ApiResponse<ApostaRodadaStatusDto>> ObterStatusApostaRodadaParaUsuario(Guid rId, Guid acId)
    {
      var aposta = await _apostaRodadaRepository.ObterStatusApostaRodada(rId, acId);
      var dto = aposta != null ? _mapper.Map<ApostaRodadaStatusDto>(aposta) : new ApostaRodadaStatusDto { StatusAposta = 0 };
      if (aposta != null) dto.StatusAposta = 1;
      return new ApiResponse<ApostaRodadaStatusDto> { Success = true, Data = dto };
    }

    public async Task<ApostasAvulsasTotaisDto> ObterTotaisApostasAvulsas(Guid rId)
    {
      var t = await _apostaRodadaRepository.ObterTotaisApostasAvulsas(rId);
      return new ApostasAvulsasTotaisDto { NumeroDeApostas = t.NumeroDeApostas, ValorTotal = t.ValorTotal };
    }

    public async Task<ApostasCampeonatoTotaisDto> ObterTotaisCampeonato(Guid cId)
    {
      var t = await _apostaRodadaRepository.ObterTotaisCampeonato(cId);
      return new ApostasCampeonatoTotaisDto { NumeroDeApostadores = t.NumeroDeApostadores, ValorTotalArrecadado = t.ValorTotalArrecadado };
    }

    public async Task<IEnumerable<JogoPalpiteResultado>> ObterJogosComPalpites(Guid aId, Guid rId) => await _apostaRodadaRepository.ObterJogosComPalpites(aId, rId);
    public async Task<ApiResponse> MarcarApostaRodadaComoSubmetida(ApostaRodada a) { a.Enviada = true; a.DataHoraSubmissao = DateTime.UtcNow; await _apostaRodadaRepository.Atualizar(a); return new ApiResponse(await CommitAsync(), null); }
    public async Task<ApiResponse> Adicionar(ApostaRodada a) { await _apostaRodadaRepository.Adicionar(a); return new ApiResponse(await CommitAsync(), null); }
    public async Task<ApiResponse> Atualizar(ApostaRodada a) { await _apostaRodadaRepository.Atualizar(a); return new ApiResponse(await CommitAsync(), null); }

   
  }
}
