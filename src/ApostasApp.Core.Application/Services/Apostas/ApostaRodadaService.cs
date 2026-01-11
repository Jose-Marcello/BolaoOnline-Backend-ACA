using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.ApostasRodada;
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
using AutoMapper;
using Microsoft.EntityFrameworkCore; // ESSENCIAL para .Include e .ToListAsync
using Microsoft.Extensions.Logging;
using System.Linq;                   // ESSENCIAL para .Select e .FirstOrDefault

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

    public async Task<ApiResponse<IEnumerable<ApostaJogoEdicaoDto>>> ObterApostasDoApostadorNaRodadaParaEdicao(Guid rodadaId, Guid apostaRodadaId)
    {

      var apiResponse = new ApiResponse<IEnumerable<ApostaJogoEdicaoDto>>(false, null);

      try
      {
        var jogosComPalpites = await _apostaRodadaRepository.ObterJogosComPalpites(apostaRodadaId, rodadaId);

        var apostasParaEdicao = jogosComPalpites.Select(p => new ApostaJogoEdicaoDto
      {
        Id = p.Id, // ID do Palpite (como string)
        IdJogo = p.Id, // ID do Jogo

        // Nomes corretos conforme o ApostaJogoEdicaoDto
        EquipeMandante = p.EquipeCasaNome,
        EscudoMandante = p.EquipeCasaEscudoUrl,

        EquipeVisitante = p.EquipeVisitanteNome,
        EscudoVisitante = p.EquipeVisitanteEscudoUrl,

        PlacarApostaCasa = p.PlacarApostaCasa,
        PlacarApostaVisita = p.PlacarApostaVisita,

        EstadioNome = p.EstadioNome,
        DataJogo = p.DataHoraReal.ToString("dd/MM"),
        HoraJogo = p.HoraJogo, // Já formatado no repositório
        DiaSemana = p.DataHoraReal.ToString("ddd").ToUpper()
      }).ToList();

            
        /*
        var apostasParaEdicao = jogosComPalpites.Select(jogo => new ApostaJogoEdicaoDto
        {
          Id = apostaRodadaId.ToString(),
          IdJogo = jogo.Id,
          EquipeMandante = jogo.EquipeCasaNome ?? "N/A",
          EscudoMandante = jogo.EquipeCasaEscudoUrl,
          EquipeVisitante = jogo.EquipeVisitanteNome ?? "N/A",
          EscudoVisitante = jogo.EquipeVisitanteEscudoUrl,
          EstadioNome = jogo.EstadioNome,
          DataJogo = jogo.DataHoraReal.ToString("dd/MM"),
          DiaSemana = jogo.DataHoraReal.ToString("ddd", new System.Globalization.CultureInfo("pt-BR")).ToUpper().Replace(".", ""),
          HoraJogo = jogo.HoraJogo,
          PlacarApostaCasa = jogo.PlacarApostaCasa,
          PlacarApostaVisita = jogo.PlacarApostaVisita,
          Enviada = true
        }).ToList();
        */

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

    public async Task<ApiResponse<ApostaRodadaResultadosDto>> ObterResultadosDaRodada(Guid rodadaId, Guid apostaRodadaId)
    {
      var apiResponse = new ApiResponse<ApostaRodadaResultadosDto>(false, null);
      try
      {
        // Adicionado Include de Estadio para resolver o 'estadioNome'
        var jogos = await _jogoRepository.Buscar(j => j.RodadaId == rodadaId)
            .Include(j => j.EquipeCasa).ThenInclude(ec => ec.Equipe)
            .Include(j => j.EquipeVisitante).ThenInclude(ev => ev.Equipe)
            .Include(j => j.Estadio) // <-- IMPORTANTE
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

              // RESOLVENDO OS ESCUDOS (Usando a propriedade do seu banco)
              EscudoMandante = j.EquipeCasa?.Equipe?.Escudo,
              EscudoVisitante = j.EquipeVisitante?.Equipe?.Escudo,

              PlacarRealCasa = j.PlacarCasa,
              PlacarRealVisita = j.PlacarVisita,
              PlacarApostaCasa = palpite?.PlacarApostaCasa,
              PlacarApostaVisita = palpite?.PlacarApostaVisita,

              // MAPEAR OS CAMPOS QUE ESTAVAM NULL
              EstadioNome = j.Estadio?.Nome ?? "Estádio não informado",
              DataJogo = j.DataJogo.ToString("dd/MM"), // Formata como string para o DTO
              // Formatação segura de TimeSpan (HoraJogo)
              HoraJogo = j.HoraJogo != null ? j.HoraJogo.ToString(@"hh\:mm") : "00:00",
              StatusJogo = j.Status.ToString(), // Ou uma lógica de "Finalizado/Agendado"

              Pontuacao = palpite?.Pontos ?? 0
            };
          }).ToList()
        };
        apiResponse.Success = true;
        return apiResponse;
      }
      catch (Exception) { return apiResponse; }
    }

    public async Task<ApiResponse<ApostaRodadaDto>> SalvarApostas(SalvarApostaRequestDto salvarApostaDto)
    {
      var apiResponse = new ApiResponse<ApostaRodadaDto>();
      try
      {
        var apostaRodadaId = Guid.Parse(salvarApostaDto.Id);
        var apostaRodada = await _apostaRodadaRepository.Buscar(ar => ar.Id == apostaRodadaId).FirstOrDefaultAsync();

        if (apostaRodada == null)
        {
          apiResponse.Success = false;
          apiResponse.Message = "Aposta não encontrada.";
          return apiResponse;
        }

        apostaRodada.Enviada = true;
        apostaRodada.DataHoraSubmissao = DateTime.UtcNow;
        _apostaRodadaRepository.Atualizar(apostaRodada);

        foreach (var pDto in salvarApostaDto.ApostasJogos)
        {
          var jogoIdGuid = Guid.Parse(pDto.JogoId);
          var palpite = await _palpiteRepository.ObterPalpiteDaAposta(apostaRodada.Id, jogoIdGuid);

          if (palpite != null)
          {
            palpite.PlacarApostaCasa = pDto.PlacarCasa;
            palpite.PlacarApostaVisita = pDto.PlacarVisitante;
            _palpiteRepository.Atualizar(palpite);
          }
        }

        if (await CommitAsync())
        {
          var apostaCompleta = await _apostaRodadaRepository.Buscar(ar => ar.Id == apostaRodadaId)
                                .Include(ar => ar.Palpites)
                                .FirstOrDefaultAsync();

          apiResponse.Data = _mapper.Map<ApostaRodadaDto>(apostaCompleta);
          apiResponse.Success = true;
        }
      }
      catch (Exception ex)
      {
        apiResponse.Success = false;
        apiResponse.Message = $"Erro técnico: {ex.Message}";
        _logger.LogError(ex, "Falha em SalvarApostas");
      }
      return apiResponse;
    }

    public async Task<ApiResponse<ApostaRodadaDto>> ExecutarTransacaoApostaAvulsa(CriarApostaAvulsaRequestDto requestDto)
    {
      var apostadorId = Guid.Parse(requestDto.ApostadorId);
      var campeonatoId = Guid.Parse(requestDto.CampeonatoId);

      var apostador = await _apostadorRepository.ObterPorIdComSaldo(apostadorId);
      if (apostador.Saldo.Valor < requestDto.CustoAposta)
      {
        Notificar("Erro", "Saldo insuficiente.");
        return new ApiResponse<ApostaRodadaDto>(false, null);
      }

      await _financeiroService.DebitarSaldoAsync(apostador.Id, requestDto.CustoAposta, TipoTransacao.ApostaRodada, "Aposta Avulsa");

      var ac = await _apostadorCampeonatoRepository.Buscar(x => x.ApostadorId == apostador.Id && x.CampeonatoId == campeonatoId).FirstOrDefaultAsync();

      var totalAvulsas = await _apostaRodadaRepository.CountAsync(a =>
          (ac != null ? a.ApostadorCampeonatoId == ac.Id : a.ApostadorId == apostador.Id) &&
          a.EhApostaCampeonato == false);

      var identificador = $"APOSTA AVULSA #{totalAvulsas + 1}";

      return await GerarApostaRodada(
          ac?.Id.ToString(),
          apostador.Id.ToString(),
          requestDto.RodadaId,
          false,
          identificador,
          requestDto.CustoAposta 
     );
    }

    public async Task<ApiResponse<IEnumerable<ApostaRodadaDto>>> ObterApostasRodadaPorApostador(Guid rodadaId, Guid? acId, Guid apId)
    {
      var apiResponse = new ApiResponse<IEnumerable<ApostaRodadaDto>>(false, null);
      try
      {
        var apostas = await _apostaRodadaRepository.ObterApostasComDetalhes(rodadaId, acId, apId);
        var rodada = await _rodadaRepository.ObterPorId(rodadaId);
        var dtos = new List<ApostaRodadaDto>();

        if (apostas != null && apostas.Any())
        {
          foreach (var aposta in apostas)
          {
            var dto = _mapper.Map<ApostaRodadaDto>(aposta);
            if (rodada != null)
            {
              dto.NumeroRodada = rodada.NumeroRodada;
              dto.DataInicio = rodada.DataInic;
              dto.DataFim = rodada.DataFim;
            }
            dto.PodeEditar = aposta.ApostadorId == apId;
            dtos.Add(dto);
          }
        }
        // REMOVA OU COMENTE ESTE BLOCO ABAIXO:
        /* else if (rodada != null)
        {
            dtos.Add(new ApostaRodadaDto { ... });
        } 
        */

        apiResponse.Data = dtos; // Agora retornará [] se não houver apostas
        apiResponse.Success = true;
        return apiResponse;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao obter apostas.");
        return apiResponse;
      }
    }

    public async Task<ApiResponse<ApostaRodadaDto>> GerarApostaRodada(string acId, string apId, string rId, bool ehCamp, string ident, decimal custo)
    {
      var jogos = await _jogoRepository.ObterJogosDaRodadaComPlacaresEEquipes(Guid.Parse(rId));
      Guid? acGuid = string.IsNullOrEmpty(acId) ? (Guid?)null : Guid.Parse(acId);
      var apostadorId = Guid.Parse(apId);
      var rodadaId = Guid.Parse(rId);
      var novaAposta = new ApostaRodada(acGuid, apostadorId, rodadaId)
      {
        IdentificadorAposta = ident,
        EhApostaCampeonato = ehCamp,
        EhApostaIsolada = !ehCamp,
        CustoPagoApostaRodada = custo, // 2. Use o parâmetro aqui em vez do valor fixo 10
        Enviada = false
      };

      await _apostaRodadaRepository.Adicionar(novaAposta);
      var palpites = jogos.Select(j => new Palpite
      {
        ApostaRodadaId = novaAposta.Id,
        JogoId = j.Id,
        Pontos = 0
      }).ToList();

      await _palpiteRepository.AdicionarRange(palpites);

      if (await CommitAsync())
        return new ApiResponse<ApostaRodadaDto> { Success = true, Data = _mapper.Map<ApostaRodadaDto>(novaAposta) };

      return new ApiResponse<ApostaRodadaDto>(false, null);
    }

    public async Task<ApiResponse<ApostaRodadaStatusDto>> ObterStatusApostaRodadaParaUsuario(Guid rId, Guid? acId, Guid apId)
    {
      var aposta = await _apostaRodadaRepository.ObterStatusApostaRodada(rId, acId, apId);
      var dto = aposta != null ? _mapper.Map<ApostaRodadaStatusDto>(aposta) : new ApostaRodadaStatusDto { StatusAposta = 0 };
      if (aposta != null) dto.StatusAposta = 1;
      return new ApiResponse<ApostaRodadaStatusDto> { Success = true, Data = dto };
    }

    // --- MÉTODOS DE TOTAIS ---
    public async Task<ApostasAvulsasTotaisDto> ObterTotaisApostasAvulsas(Guid rId)
    {
      var t = await _apostaRodadaRepository.ObterTotaisApostasAvulsas(rId);
      return new ApostasAvulsasTotaisDto { NumeroDeApostas = t.NumeroDeApostas, ValorTotal = t.ValorTotal };
    }

    /*
    public async Task<ApostasCampeonatoTotaisDto> ObterTotaisCampeonato(Guid cId)
    {
      var t = await _apostaRodadaRepository.ObterTotaisCampeonato(cId);
      return new ApostasCampeonatoTotaisDto { NumeroDeApostadores = t.NumeroDeApostadores, ValorTotalArrecadado = t.ValorTotalArrecadado };
    }
    */

    public async Task<IEnumerable<JogoPalpiteResultado>> ObterJogosComPalpites(Guid aId, Guid rId) => await _apostaRodadaRepository.ObterJogosComPalpites(aId, rId);
    public async Task<ApiResponse> MarcarApostaRodadaComoSubmetida(ApostaRodada a) { a.Enviada = true; a.DataHoraSubmissao = DateTime.UtcNow; await _apostaRodadaRepository.Atualizar(a); return new ApiResponse(await CommitAsync(), null); }
    public async Task<ApiResponse> Adicionar(ApostaRodada a) { await _apostaRodadaRepository.Adicionar(a); return new ApiResponse(await CommitAsync(), null); }
    public async Task<ApiResponse> Atualizar(ApostaRodada a) { await _apostaRodadaRepository.Atualizar(a); return new ApiResponse(await CommitAsync(), null); }
  }
}
