using ApostasApp.Core.Domain.Interfaces.RankingRodadas;
using ApostasApp.Core.Domain.Models;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Models.Interfaces.Usuarios;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.InfraStructure.Data.Repository.Apostadores;
using ApostasApp.Core.InfraStructure.Data.Repository.Campeonatos;
using ApostasApp.Core.Presentation.ViewModels;
using ApostasApp.Infrastructure.Data.Repository;
using AutoMapper;
using DApostasApp.Core.Domain.Models.RankingRodadas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ApostasApp.Core.Presentation.Controllers
{
    [Route("RankingRodada")]
    public class RankingRodadaController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioService;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ApostadorCampeonatoController> _logger;


        public RankingRodadaController(IMapper mapper,
                                   IUsuarioService usuarioService,
                                   IUnitOfWork uow,
                                   ILogger<ApostadorCampeonatoController> logger,
                                   INotificador notificador) : base(notificador)
                             
        {
            _mapper = mapper;
            _usuarioService = usuarioService;
            _uow = uow;
            _logger = logger;

        }

        [HttpGet("ListarRodadas")]
        public async Task<IActionResult> ListarRodadasComRanking()
        {
            try
            {
                // 1. Obter o ID do usuário logado
                var userId = await _usuarioService.GetLoggedInUserId();

                if (string.IsNullOrEmpty(userId))
                {
                    TempData["Notificacao"] = "Usuário não logado.";
                    return RedirectToAction("Login", "Account");
                }

                var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
                var campeonato = await campeonatoRepository.ObterCampeonatoAtivo();

                if (campeonato == null)
                {
                    TempData["Notificacao"] = "Nenhum campeonato ativo encontrado.";
                    return View("CampeonatoNaoEncontrado");
                }

                // 2. Obter o ApostadorCampeonato para o usuário logado e o campeonato ativo
                var apostadorCampeonatoRepository = _uow.GetRepository<ApostadorCampeonato>() as ApostadorCampeonatoRepository;
                var apostadorCampeonato = await apostadorCampeonatoRepository.ObterApostadorCampeonatoPorApostadorECampeonato(userId, campeonato.Id);

                if (apostadorCampeonato == null)
                {
                    TempData["Notificacao"] = "Apostador não associado a um campeonato ativo.";
                    return RedirectToAction("PainelUsuario", "Account"); // Ou outra página apropriada
                }

                // 3. Obter as rodadas com ranking
                var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
                var rodadas = await rodadaRepository.ObterRodadasComRanking(campeonato.Id);

                // 4. Mapear para o ViewModel da Rodada (se necessário)
                var rodadasViewModel = _mapper.Map<IEnumerable<RodadaViewModel>>(rodadas);

                // 5. Criar o ViewModel principal para a View e passar para ela
                var model = new RodadasComApostadorViewModel
                {
                    Rodadas = rodadasViewModel,
                    ApostadorCampeonatoId = apostadorCampeonato.Id, // <--- AGORA O ID ESTÁ AQUI!
                    CampeonatoNome = campeonato.Nome // Para o título da View
                };

                // Você pode remover esta linha se o CampeonatoNome estiver no Model
                TempData["Campeonato"] = campeonato.Nome;

                return View(model);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao atualizar o banco de dados.");
                return View("ErroBancoDeDados");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar rodadas com ranking.");
                return View("ErroGenerico");
            }
        }


        [Route("ExibirRanking/{id:guid}")]
        public async Task<IActionResult> ExibirRanking(Guid Id, string returnUrl)
        {
            var rodada = await ObterRodada(Id);

            if (rodada == null)
            {
                return NotFound();
            }

            IEnumerable<RankingRodada> enumRanking = null;
            var userIdLogado = await _usuarioService.GetLoggedInUserId();

            if (userIdLogado != null)
            {
                var apostadorRepository = _uow.GetRepository<Apostador>() as ApostadorRepository;
                var apostadorLogado = _mapper.Map<Apostador>(await apostadorRepository.ObterApostadorPorUsuarioId(userIdLogado));

                var apelidoUsuarioLogado = apostadorLogado.Usuario.Apelido;

                var rankingRodadaRepository = _uow.GetRepository<RankingRodada>() as RankingRodadaRepository;

                enumRanking = await rankingRodadaRepository.ObterRankingDaRodada(Id);

                var listaRanking = enumRanking.ToList();

                int? posicaoApostador = null;
                int? pontuacaoPrimeiro = listaRanking.FirstOrDefault()?.Pontuacao;
                string diferencaParaPrimeiro = "";

                for (int i = 0; i < listaRanking.Count; i++)
                {
                    if (listaRanking[i].ApostadorCampeonato.Apostador.Usuario.Apelido == apelidoUsuarioLogado)
                    {
                        posicaoApostador = i + 1;
                        if (pontuacaoPrimeiro.HasValue)
                        {
                            int diferencaPontos = pontuacaoPrimeiro.Value - listaRanking[i].Pontuacao;
                            diferencaParaPrimeiro = $" {i} posições / ({diferencaPontos}) pontos";
                        }
                        break;
                    }
                }

                ViewBag.PosicaoDoApostador = posicaoApostador;
                ViewBag.DiferencaParaPrimeiro = diferencaParaPrimeiro;
                ViewBag.TotalDeApostadores = listaRanking.Count;
            }
            else
            {
                ViewBag.PosicaoDoApostador = null;
                ViewBag.DiferencaParaPrimeiro = "Usuário não encontrado.";
            }

            TempData["Campeonato"] = rodada.Campeonato.Nome;
            TempData["Rodada"] = rodada.NumeroRodada;
            TempData["IdRodada"] = rodada.Id;

            // Passe a returnUrl para a ViewBag
            ViewBag.ReturnUrl = returnUrl;

            var rankingRodadaViewModel = _mapper.Map<IEnumerable<RankingRodadaViewModel>>(enumRanking);

            return View(rankingRodadaViewModel);
        }


        [HttpGet("RankingRodada/ListarJogosDaRodada/{id:guid}")]
        //[Route("ListarJogosDaRodada/{id:guid}")]
        public async Task<IActionResult> ListarJogosDaRodada(Guid id)
        {
            //aqui pensar numa alteração: Permitir o gerenciamento de todas as RODADAS NÃO INICIADAS ...

            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodada(id));

            if (rodada == null)
            {
                return NotFound();
            }

            TempData["Campeonato"] = rodada.Campeonato.Nome;
            TempData["Rodada"] = rodada.NumeroRodada;
            TempData["NumJogos"] = rodada.NumJogos;

            var jogoRepository = _uow.GetRepository<Jogo>() as JogoRepository;
            var jogoViewModel = _mapper.Map<IEnumerable<JogoViewModel>>(await jogoRepository.ObterJogosDaRodada(id));

            //TempData["Lancados"] = jogoViewModel.Count();

            return View(jogoViewModel);
        }


        private async Task<RodadaViewModel> ObterRodada(Guid id)
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaCampeonato(id));

            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            rodada.Campeonatos = _mapper.Map<IEnumerable<CampeonatoViewModel>>(await campeonatoRepository.ObterTodos());
            return rodada;
        }

        [HttpGet("BuscarRankingRodada/{id}")]
        public async Task<JsonResult> BuscarRankingDaRodada(Guid Id)
        {
            var rankingRodadaRepository = _uow.GetRepository<RankingRodada>() as RankingRodadaRepository;
            var listaRanking = await rankingRodadaRepository.ObterRankingDaRodada(Id);

            var data = listaRanking.Select(ranking => new
            {
                Id = ranking.Id,

                Posicao = ranking.Posicao,
                Apelido = ranking.ApostadorCampeonato.Apostador.Usuario.Apelido,
                Pontuacao = ranking.Pontuacao,


            }).ToList();

            return Json(new { data });
        }


        private async Task<ApostadorCampeonatoViewModel> ObterApostadorCampeonato(Guid Id)
        {
            var apostadorCampeonatoRepository = _uow.GetRepository<ApostadorCampeonato>() as ApostadorCampeonatoRepository;
            var apostadorCampeonato = _mapper.Map<ApostadorCampeonatoViewModel>
                (await apostadorCampeonatoRepository.ObterApostadorCampeonatoDoApostador(Id));

            return apostadorCampeonato;

        }

    }
}
