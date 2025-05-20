using ApostasApp.Core.Domain.Models;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Estadios;
using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Domain.Services.Jogos;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.InfraStructure.Data.Repository.Campeonatos;
using ApostasApp.Core.Presentation.Controllers;
using ApostasApp.Core.Presentation.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;


namespace ApostasApp.Core.Presentation.Controllers
{
    [Route("Jogo")]
    public class JogoController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly IJogoService _jogoService;

        public JogoController(IMapper mapper,
                                  IUnitOfWork uow,
                                  IJogoService jogoService,
                                  INotificador notificador) : base(notificador)
        //IAppIdentityUser user) : base(notificador, user) 
        {
            _mapper = mapper;
            _jogoService = jogoService;
            _uow = uow;
        }


        [Route("dados-do-jogo/{id:guid}")]
        public async Task<IActionResult> DetalheJogo(Guid id)
        {
            //var jogo = await ObterJogo(id);

            var jogoRepository = _uow.GetRepository<Jogo>() as JogoRepository;
            var jogoDetalhe =  await jogoRepository.ObterJogoRodada(id);
             
            // Retorne os dados como JSON
            return Json(jogoDetalhe);
        }


        [Route("jogos-da_rodada_pronta_nao_iniciada")]
        public async Task<IActionResult> ManterJogos()
        {
            //aqui pensar numa alteração: Permitir o gerenciamento de todas as RODADAS NÃO INICIADAS ...

            var rodada = await ObterRodadaNaoIniciada();

            //var campeonato = rodada.Campeonato;

            if (rodada == null)
            {
                return NotFound();
            }

            TempData["Campeonato"] = rodada.Campeonato.Nome;
            TempData["Rodada"] = rodada.NumeroRodada;
            TempData["NumJogos"] = rodada.NumJogos;


            var jogoViewModel = await ObterJogosdaRodada(rodada.Id);

            TempData["Lancados"] = jogoViewModel.Count();

            return View(jogoViewModel);
        }

        /*private async Task<JogoViewModel> ObterJogo(Guid id)
        {
            var jogoRepository = _uow.GetRepository<Jogo>() as JogoRepository;
            var jogo = _mapper.Map<JogoViewModel>(await jogoRepository.ObterJogoRodada(id));

            var equipeCampeonatoRepository = _uow.GetRepository<EquipeCampeonato>() as EquipeCampeonatoRepository;
            jogo.EquipesCampeonato = _mapper.Map<IEnumerable<EquipeCampeonatoViewModel>>(await equipeCampeonatoRepository.ObterEquipesDoCampeonato(jogo.Rodada.CampeonatoId));

            var estadioRepository = _uow.GetRepository<Estadio>() as EstadioRepository;
            jogo.Estadios = _mapper.Map<IEnumerable<EstadioViewModel>>(await estadioRepository.ObterEstadiosEmOrdemAlfabetica());
            return jogo;
        }*/

        /*
                private async Task<RodadaViewModel> ObterRodadaCorrente()
                {
                    var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
                    var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaAtiva());
                    return rodada;
                }
        */
        private async Task<RodadaViewModel> ObterRodadaNaoIniciada()
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaEmConstrucao());
            return rodada;
        }

        private async Task<IEnumerable<JogoViewModel>> ObterJogosdaRodada(Guid id)
        {
            var jogoRepository = _uow.GetRepository<Jogo>() as JogoRepository;
            var jogo = _mapper.Map<IEnumerable<JogoViewModel>>(await jogoRepository.ObterJogosDaRodada(id));
            return jogo;

        }
    }
}
        /*}


        /*
        private async Task<CampeonatoViewModel> ObterCampeonato(Guid id)
        {
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            var campeonato = _mapper.Map<CampeonatoViewModel>(await campeonatoRepository.ObterCampeonato(id));
            return campeonato;
        }


        private async Task<RodadaViewModel> ObterRodadaAtiva()
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaAtiva());

            return rodada;
        }


        private async Task<JogoViewModel> AssociarRodada(JogoViewModel jogo)
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            jogo.Rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaAtiva());
            jogo.RodadaId = jogo.Rodada.Id;
            return jogo;

        }

        private async Task<JogoViewModel> PopularEstadios(JogoViewModel jogo)
        {

            var estadioRepository = _uow.GetRepository<Estadio>() as EstadioRepository;
            jogo.Estadios = _mapper.Map<IEnumerable<EstadioViewModel>>(await estadioRepository.ObterEstadiosEmOrdemAlfabetica());
            return jogo;
        }

        private async Task<JogoViewModel> PopularEquipesDoCampeonato(JogoViewModel jogo, Guid campeonatoId)
        {
            var equipeCampeonatoRepository = _uow.GetRepository<EquipeCampeonato>() as EquipeCampeonatoRepository;
            jogo.EquipesCampeonato = _mapper.Map<IEnumerable<EquipeCampeonatoViewModel>>(await equipeCampeonatoRepository.ObterEquipesDoCampeonato(campeonatoId));
            return jogo;
        }

    }
}
*/