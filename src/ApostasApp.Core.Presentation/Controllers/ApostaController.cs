using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Models;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Domain.Services.RankingRodadas;
using ApostasApp.Core.Domain.Services.Rodadas;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.InfraStructure.Data.Repository.Campeonatos;
using ApostasApp.Core.Presentation.Controllers;
using ApostasApp.Core.Presentation.ViewModels;
using AutoMapper;
using DApostasApp.Core.Domain.Models.RankingRodadas;
using Microsoft.AspNetCore.Mvc;


namespace ApostasApp.Presentation.Controllers
{
    //[Authorize]
    public class ApostaController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IRankingRodadaService _rankingRodadaService;
        private readonly IRodadaService _rodadaService;
        private readonly IApostadorCampeonatoService _apostadorCampeonatoService;
        private readonly IUnitOfWork _uow;

        public ApostaController(IMapper mapper,
                                  IRankingRodadaService rankingRodadaService,
                                  IRodadaService rodadaService,
                                  IUnitOfWork uow,
                                  INotificador notificador) : base(notificador)

        {
            _mapper = mapper;
            _rankingRodadaService = rankingRodadaService;
            _rodadaService = rodadaService;
            _uow = uow;

        }

        [Route("gerar-apostas-da-rodada/{id:guid}")]
        public async Task<IActionResult> GerarApostasERanking(Guid Id)
        {
            //Ao GERAR APOSTAS a rodada deve ter STATUS NÃO INICIADA e todos os APOSTADORES
            //cadastrados (Se o Sistema permitir a entrada posterior de um apostador, terá que haver
            //um procedimento de geração de APOSTAS/RANKING para o APOSTADOR)
            //Após a GERAÇÃO DE APOSTAS e seu respectivo RANKING, o status da APOSTA é alterado para
            //"EM APOSTAS" o que acarreta com a abertura das APOSTAS no SITE DO USUÁRIO

            //rodada = await ObterRodadaCorrente();  //apenas uma RODADA no CAMPEONATO ATIVO

            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodada(Id);  //apenas uma RODADA no CAMPEONATO ATIVO

            if (rodada == null)
            {
                //return View(new { success = false, message = "Rodada não encontada." });
                return NotFound();
            }
      
            var campeonatoId = rodada.CampeonatoId;

            TempData["Campeonato"] = rodada.Campeonato.Nome;
            TempData["Rodada"] = rodada.NumeroRodada;

            //aqui tem que pegar TODOS os jogos da RODADA ATIVA
            //e para cada 1, pegar todos os APOSTADORES DO CAMPEONATO REF A RODADA
            //e para cada APOSTADOR/JOGO inserir um REGISTRO DE APOSTA com placar vazio ..

            //impo : só pode deixar apostar se TODOS OS JOGOS DA RODADA, ESTIVEREM LANÇADOS

            var jogoRepository = _uow.GetRepository<Jogo>() as JogoRepository;
            var listaJogos = await jogoRepository.ObterJogosDaRodada(rodada.Id);

            var apostadorCampeonatoRepository = _uow.GetRepository<ApostadorCampeonato>() as ApostadorCampeonatoRepository;
            var listaApostadores = await apostadorCampeonatoRepository.ObterApostadoresDoCampeonato(campeonatoId);


            //gerar RANKING da RODADA

            foreach (var apostadorRank in listaApostadores)
            {

                //aqui tem que gerar o registro do RANKING onde será alocada a PONTUAÇÃO
                //e POSIÇÃO de cada APOSTADOR

                var ranking = new RankingRodada();

                //aposta.DataHoraAposta = inicialmente nulo
                ranking.ApostadorCampeonatoId = apostadorRank.Id;
                ranking.RodadaId = rodada.Id;
                ranking.DataAtualizacao = DateTime.Now;
                ranking.Posicao = 0;  //inicialmente 0
                ranking.Pontuacao = 0;  //inicialmente 0

                await _rankingRodadaService.Adicionar(ranking);
            }

            //após GERAR as APOSTAS e o RANKING da RODADA, o status da RODADA deverá
            //ser alterado para "EM APOSTAS" o que libera as apostas no SITE DOS APOSTADORES

            rodada.Status = StatusRodada.EmApostas;

            await _rodadaService.Atualizar(_mapper.Map<Rodada>(rodada));

            //_uow.Commit();
            await _uow.SaveChanges();
            


            //var apostaViewModel = _mapper.Map<IEnumerable<ApostaViewModel>>(await _apostaRepository.ObterApostasDaRodada(rodada.Id));

            // aqui pode ter uma view com todas as apostas geradas PARA CONFERÊNCIA
            return View();


        }

        [Route("consultar-apostas-da-rodada")]
        public async Task<IActionResult> ConsultarApostas()
        {
            var rodada = await ObterRodadaCorrente();

            var campeonatoId = rodada.Campeonato.Id;

            if (rodada == null)
            {
                return NotFound();
            }

            TempData["Campeonato"] = rodada.Campeonato.Nome;
            TempData["Rodada"] = rodada.NumeroRodada;

            var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;          

            var apostaViewModel = _mapper.Map<IEnumerable<ApostaViewModel>>(await apostaRepository.ObterApostasDaRodada(rodada.Id));

            // aqui pode ter uma view com todas as apostas geradas PARA CONFERÊNCIA
            return View(apostaViewModel);

        }


        [Route("deletar-apostas-da-rodada")]
        public async Task<IActionResult> DeletarApostas()
        {
            //Aqui é EM TESTES .. depois tem que colocar pergunta se deseja realmente deletar
            //e não pode DELETAR se a RODADA já estiver com APOSTAS DE USUÁRIO (CORRENTE)

            var rodada = await ObterRodadaCorrente();

            var campeonatoId = rodada.Campeonato.Id;

            if (rodada == null)
            {
                return NotFound();
            }

            var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
            var apostas = await apostaRepository.ObterApostasDaRodada(rodada.Id);

            foreach (var a in apostas)
            {
                await _rodadaService.Remover(a.Id);
            }

            //salva a operação
            //_uow.Commit(); await
            _uow.SaveChanges(); 

            TempData["Campeonato"] = rodada.Campeonato.Nome;
            TempData["Rodada"] = rodada.NumeroRodada;            

            var apostaViewModel = _mapper.Map<IEnumerable<ApostaViewModel>>(await apostaRepository.ObterApostasDaRodada(rodada.Id));

            // aqui pode ter uma view sem as apostas deletadas PARA CONFERÊNCIA
            return View(apostaViewModel);

        }

        private async Task<RodadaViewModel> ObterRodadaCorrente()
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaCorrente());
            return rodada;
        }

        private bool ExisteApostasNaRodada(Guid idRodada)
        {
            //Verifica se as APOSTAS já foram geradas
            var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
            var apostas = apostaRepository.ObterApostasDaRodada(idRodada);

            if (apostas.Result.Count() != 0)
            {
                return true;
            }

            return false;

        }

    }

}