using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Models;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Models.Interfaces.Usuarios;
using ApostasApp.Core.Domain.Models.Notificacoes;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.InfraStructure;
using ApostasApp.Core.InfraStructure.Data.Repository.Apostadores;
using ApostasApp.Core.InfraStructure.Data.Repository.Campeonatos;
using ApostasApp.Core.Presentation.Controllers;
using ApostasApp.Core.Presentation.ViewModels;
using ApostasApp.Core.PresentationViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;


namespace ApostasApp.Core.Presentation.Controllers
{

    [Route("ApostadorCampeonato")]
    public class ApostadorCampeonatoController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IApostadorCampeonatoService _apostadorCampeonatoService;
        private readonly IUsuarioService _usuarioService;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ApostadorCampeonatoController> _logger;
        private readonly INotificador _notificador;

        public ApostadorCampeonatoController(IMapper mapper,
                                   IApostadorCampeonatoService apostadorCampeonatoService,
                                   IUsuarioService usuarioService,
                                   IUnitOfWork uow,
                                   ILogger<ApostadorCampeonatoController> logger,
                                   INotificador notificador) : base(notificador)
        //IAppIdentityUser user) : base(notificador, user) 
        {

            try
            {
                _mapper = mapper;
                _apostadorCampeonatoService = apostadorCampeonatoService;
                _usuarioService = usuarioService;
                _uow = uow;
                _logger = logger;
                 _notificador = notificador;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no CONSTRUTOR DO APOSTADORCAMPEONATO.");
            }

        }


        // [Route("apostar-na-rodada/{id:guid}")]
        // [HttpPost]
        /*[Route("ApostadorCampeonato/Apostar")]
        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Apostar([FromQuery] Guid rodadaId)
        //public async Task<IActionResult> Apostar(Guid id)
        {
            try
            {
                CarregarDadosRodadaApostador(rodadaId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao clicar em APOSTAR.");
            }

            return View();
        }
*/
        /*[Route("consultar-apostas-do-apostador-na-rodada/{id:guid}")]
        [HttpPost]
        public async Task<IActionResult> ConsultarApostas(Guid id)
        {
            CarregarDadosRodadaApostador(id);

            return View();

        }*/

        //[Route("buscar-apostas-do-apostador-na-rodada/{id:guid}")]
        [HttpGet("RodadaEmApostas/{id}")]
        public async Task<IActionResult> ListarApostasDoApostadorNaRodadaEmApostas(Guid id)
        {
            var apostadorCampeonato = await ObterApostadorCampeonato(id);

            var usuario = await _usuarioService.GetLoggedInUser();

            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodadaEmApostas();

            if (apostadorCampeonato != null)
            {
                TempData["Campeonato"] = rodada.Campeonato.Nome;
                TempData["Rodada"] = rodada.NumeroRodada;
                TempData["Apostador"] = usuario.Apelido;
                TempData["IdApostadorCampeonato"] = apostadorCampeonato.Id;

                //Isso aqui deverá ser substituído por uma consulta em ApostasDaRodada, uma tabela
                //que deverá conter apenas DATAHORAAPOSTA, ID da RODADA, ID do APOSTADOR, ENVIADA, PONTOS
                var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
                var verifApostasNaRodada = await apostaRepository.ObterApostaSalvaDoApostadorNaRodada(rodada.Id, apostadorCampeonato.Id);

                if (verifApostasNaRodada.Enviada)
                {
                    TempData["DATA_APOSTA"] = verifApostasNaRodada.DataHoraAposta.ToShortDateString();
                    TempData["HORA_APOSTA"] = verifApostasNaRodada.DataHoraAposta.ToShortTimeString();
                    TempData["ENVIADA"] = "ENVIADA";
                }
                else
                {
                    TempData["DATAHORA_APOSTA"] = "";
                    TempData["ENVIADA"] = "AINDA NÃO ENVIADA";
                }

            }

            return View(apostadorCampeonato);

        }


        //[Route("buscar-apostas-do-apostador-na-rodada/{id:guid}")]
        //[HttpGet("/ListarApostasDoApostadorNaRodadaCorrente/{id:guid}")]
        [HttpGet("/ListarApostasDoApostadorNaRodadaCorrente/{id}")]
        //[Route("/ListarApostasDoApostadorNaRodadaCorrente/{id}")]
        //[HttpGet("ListarApostasDoApostadorNaRodadaCorrente/{id}")]        
        public async Task<IActionResult> ListarApostasDoApostadorNaRodadaCorrente(Guid id)
        {
            var apostadorCampeonato = await ObterApostadorCampeonato(id);

            var usuario = await _usuarioService.GetLoggedInUser();

            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodadaCorrente();

            if (apostadorCampeonato != null)
            {
                TempData["Campeonato"] = rodada.Campeonato.Nome;
                TempData["Rodada"] = rodada.NumeroRodada;
                TempData["idRodada"] = rodada.Id;
                TempData["Apostador"] = usuario.Apelido;
                TempData["IdApostadorCampeonato"] = apostadorCampeonato.Id;

                //Isso aqui deverá ser substituído por uma consulta em ApostasDaRodada, uma tabela
                //que deverá conter apenas DATAHORAAPOSTA, ID da RODADA, ID do APOSTADOR, ENVIADA, PONTOS
                var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
                var verifApostasNaRodada = await apostaRepository.ObterApostaSalvaDoApostadorNaRodada(rodada.Id, apostadorCampeonato.Id);

                if (verifApostasNaRodada.Enviada)
                {
                    TempData["DATA_APOSTA"] = verifApostasNaRodada.DataHoraAposta.ToShortDateString();
                    TempData["HORA_APOSTA"] = verifApostasNaRodada.DataHoraAposta.ToShortTimeString();
                    TempData["ENVIADA"] = "ENVIADA";
                }
                else
                {
                    TempData["DATAHORA_APOSTA"] = "";
                    TempData["ENVIADA"] = "AINDA NÃO ENVIADA";
                }

            }

            return View(apostadorCampeonato);
        }


        [HttpGet("ListarApostasDoApostadorNaRodada/{apostadorCampeonatoId}/{rodadaId}")]
        public async Task<IActionResult> ListarApostasDoApostadorNaRodada(Guid apostadorCampeonatoId, Guid rodadaId)
        {
            var apostadorCampeonato = await ObterApostadorCampeonato(apostadorCampeonatoId); // Assumindo que ObterApostadorCampeonato está definido
            var usuario = await _usuarioService.GetLoggedInUser(); // Assumindo que _usuarioService está injetado

            // Verificações de nulidade para apostadorCampeonato e usuario
            if (apostadorCampeonato == null || usuario == null)
            {
                TempData["Notificacao"] = "Dados do apostador ou usuário não encontrados.";
                return RedirectToAction("PainelUsuario", "Account");
            }

            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodadaCampeonato(rodadaId); // Assumindo que ObterRodadaCampeonato existe e obtém a rodada completa com campeonato

            if (rodada == null)
            {
                TempData["Notificacao"] = "Rodada não encontrada.";
                return RedirectToAction("PainelUsuario", "Account");
            }

            // Consulta a aposta salva para verificar o status
            var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
            var verifApostasNaRodada = await apostaRepository.ObterApostaSalvaDoApostadorNaRodada(rodada.Id, apostadorCampeonato.Id);
                        

            
            // Cria e popula a nova ViewModel
            var viewModel = new ApostasPorRodadaViewModel
            {
                ApostadorCampeonatoId = apostadorCampeonato.Id, // Já vem preenchido da URL
                RodadaId = rodadaId, // Já vem preenchido da URL
                ApostadorApelido = usuario.Apelido,
                CampeonatoNome = rodada.Campeonato?.Nome ?? "N/A", // Verifique se Campeonato não é null
                NumeroRodada = rodada.NumeroRodada,
                DataAposta = verifApostasNaRodada?.DataHoraAposta.ToShortDateString() ?? "",
                HoraAposta = verifApostasNaRodada?.DataHoraAposta.ToShortTimeString() ?? "",
                StatusEnvioAposta = verifApostasNaRodada?.Enviada == true ? "ENVIADA" : "AINDA NÃO ENVIADA"
            };

            // OPCIONAL: Mantenha TempData para notificações pontuais ou mensagens que desaparecem após uma requisição
            // TempData["Notificacao"] = "Alguma mensagem aqui";

            // Agora, retorne a View com a nova ViewModel
            return View(viewModel);

            //return View(apostadorCampeonato);
        }

        [HttpPost("BuscarApostasDoApostadorNaRodada/{apostadorCampeonatoId}/{rodadaId}")]
        public async Task<IActionResult> BuscarApostasDoApostadorNaRodada(Guid apostadorCampeonatoId, Guid rodadaId)
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodada(rodadaId);

            var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
            var listaApostas = await apostaRepository.ObterApostasDoApostadorNaRodada(rodadaId, apostadorCampeonatoId);

            var data = listaApostas.Select(aposta => new
            {

                Id = aposta.Id,
                IdJogo = aposta.Jogo.Id,
                SiglaMandante = aposta.Jogo.EquipeCasa.Equipe.Sigla,
                EscudoMandante = aposta.Jogo.EquipeCasa.Equipe.Escudo,
                SiglaVisitante = aposta.Jogo.EquipeVisitante.Equipe.Sigla,
                EscudoVisitante = aposta.Jogo.EquipeVisitante.Equipe.Escudo,
                PlacarRealCasa = aposta.Jogo.PlacarCasa,
                PlacarRealVisitante = aposta.Jogo.PlacarVisita,
                PlacarApostaCasa = aposta.PlacarApostaCasa,
                PlacarApostaVisitante = aposta.PlacarApostaVisita,
                StatusJogo = aposta.Jogo.Status,

                Pontuacao = aposta.Pontos

            }).ToList();

            return Json(new { data });
        }
        
        [HttpPost("BuscarStatusEDataHoraApostaDaRodada/{id}")]
        public async Task<JsonResult> BuscarStatusEDataHoraApostaDaRodada(Guid Id)
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodadaEmApostas();


            var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
            var aposta = await apostaRepository.ObterStatusApostasDoApostadorNaRodada(rodada.Id, Id);
            
            return Json(new { aposta });
        }

        [HttpPost("BuscarApostasDoApostadorNaRodadaEmApostas/{id}")]
        public async Task<JsonResult> BuscarApostasDoApostadorNaRodadaEmApostas(Guid Id)
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodadaEmApostas();


            var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
            var listaApostas = await apostaRepository.ObterApostasDoApostadorNaRodada(rodada.Id, Id);

            var data = listaApostas.Select(aposta => new
            {

                Id = aposta.Id,
                IdJogo = aposta.Jogo.Id,
                EquipeMandante = aposta.Jogo.EquipeCasa.Equipe.Nome,
                SiglaMandante = aposta.Jogo.EquipeCasa.Equipe.Sigla,
                EscudoMandante = aposta.Jogo.EquipeCasa.Equipe.Escudo,
                PlacarMandante = aposta.PlacarApostaCasa == 0 && !aposta.Enviada ? "" : aposta.PlacarApostaCasa.ToString(), // Verifica se é zero
                EquipeVisitante = aposta.Jogo.EquipeVisitante.Equipe.Nome,
                SiglaVisitante = aposta.Jogo.EquipeVisitante.Equipe.Sigla,
                EscudoVisitante = aposta.Jogo.EquipeVisitante.Equipe.Escudo,
                PlacarVisitante = aposta.PlacarApostaVisita == 0 && !aposta.Enviada ? "" : aposta.PlacarApostaVisita.ToString(), // Verifica se é zero
                DataJogo = aposta.Jogo.DataJogo.ToString("yyyy-MM-dd"), // Formato ISO 8601 para facilitar a ordenação
                HoraJogo = aposta.Jogo.HoraJogo.ToString(@"hh\:mm") // Formato HH:mm


            }).ToList();

            return Json(new { data });
        }


        [HttpPost("BuscarApostasDoApostadorNaRodadaCorrente/{id}")]
        public async Task<JsonResult> BuscarApostasDoApostadorNaRodadaCorrente(Guid Id)
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodadaCorrente();

            var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
            var listaApostas = await apostaRepository.ObterApostasDoApostadorNaRodada(rodada.Id, Id);

            var data = listaApostas.Select(aposta => new
            {

                Id = aposta.Id,
                IdJogo = aposta.Jogo.Id,
                SiglaMandante = aposta.Jogo.EquipeCasa.Equipe.Sigla,
                EscudoMandante = aposta.Jogo.EquipeCasa.Equipe.Escudo,
                SiglaVisitante = aposta.Jogo.EquipeVisitante.Equipe.Sigla,
                EscudoVisitante = aposta.Jogo.EquipeVisitante.Equipe.Escudo,
                PlacarRealCasa = aposta.Jogo.PlacarCasa,
                PlacarRealVisitante = aposta.Jogo.PlacarVisita,
                PlacarApostaCasa = aposta.PlacarApostaCasa,
                PlacarApostaVisitante = aposta.PlacarApostaVisita,
                StatusJogo = aposta.Jogo.Status,
                Enviada = aposta.Enviada,
                DataHoraAposta = aposta.DataHoraAposta,
                DataJogo = aposta.Jogo.DataJogo.ToString("yyyy-MM-dd"), // Formato ISO 8601 para facilitar a ordenação
                HoraJogo = aposta.Jogo.HoraJogo.ToString(@"hh\:mm"), // Formato HH:mm

                Pontuacao = aposta.Pontos

            }).ToList();

            return Json(new { data });
        }


        [HttpGet("ExibirInterfaceDaRodadaEmApostas/{apostadorCampeonatoId}")]
        public async Task<IActionResult> ExibirInterfaceDaRodadaEmApostas(Guid apostadorCampeonatoId)
        {
            var apostadorCampeonatoRepository = _uow.GetRepository<ApostadorCampeonato>() as ApostadorCampeonatoRepository;
            var apostadorCampeonato = await apostadorCampeonatoRepository.ObterPorId(apostadorCampeonatoId);

            // Obter a rodada ativa (corrente/em apostas)
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodadaEmApostas();
                      
            if (rodada == null)
            {
                TempData["Notificacao"] = "No momento NÃO HÁ uma RODADA em APOSTAS, fique atento ao final da RODADA CORRENTE e ao aviso da próxima abertura de APOSTAS !! .";
                return RedirectToAction("PainelUsuario", "Account");

            }

            var campeonato = rodada?.Campeonato; // Adicionado tratamento para rodada nula

            if (rodada != null)
            {
                return RedirectToAction("ListarApostasDoApostadorNaRodadaEmApostas", new { id = apostadorCampeonatoId });
            }
            else
            {
                // Lógica para outros estados de rodada, se necessário
                return NotFound(); // Ou outra ação apropriada
            }
        }


        [HttpGet("ExibirInterfaceDaRodadaCorrente/{apostadorCampeonatoId}")]
        public async Task<IActionResult> ExibirInterfaceDaRodadaCorrente(Guid apostadorCampeonatoId)
        {
            var apostadorCampeonatoRepository = _uow.GetRepository<ApostadorCampeonato>() as ApostadorCampeonatoRepository;
            var apostadorCampeonato = await apostadorCampeonatoRepository.ObterPorId(apostadorCampeonatoId);

            // Obter a rodada ativa (corrente/em apostas)
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodadaCorrente();

            if (rodada == null)
            {
                TempData["Notificacao"] = "Ainda não há uma RODADA CORRENTE, com Jogos em Andamento!! no momento.";
                return RedirectToAction("PainelUsuario", "Account");
              
            }

            var campeonato = rodada?.Campeonato; // Adicionado tratamento para rodada nula

            if (rodada.Status == StatusRodada.Corrente)
            {
                return RedirectToAction("ListarApostasDoApostadorNaRodadaCorrente", new { id = apostadorCampeonatoId });

            }
            else
            {
                // Lógica para outros estados de rodada, se necessário
                return NotFound(); // Ou outra ação apropriada
            }
        }


        [HttpPost("SalvarApostas")]
        //[Route("ApostadorCampeonato/SalvarApostas")]
        public async Task<JsonResult> SalvarApostas([FromBody] List<SalvarApostaViewModel> apostasViewModel)
        //public async Task<JsonResult> SalvarApostasNova([FromBody] List<SalvarApostaViewModel> apostasViewModel)
        {

            // Obter a rodada associada às apostas (que só pode ser a RODADA ATIVA
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodadaEmApostas();

            // Verificar o status da rodada IMEDIATAMENTE antes de salvar
            if (rodada.Status != StatusRodada.EmApostas) // Substitua "Aberto" pelo status correto
            {
                // Rodada foi fechada PARA APOSTAS, impedir a alteração dos resultados
                //ModelState.AddModelError(string.Empty, "As apostas para esta RODADA foram encerradas.");
                return Json(new { success = false, message = "As apostas para esta RODADA foram encerradas." });

            }

            _logger.LogInformation("Iniciando a ação SalvarApostas.");

            if (apostasViewModel.IsNullOrEmpty())
            {

                return Json(new { success = false, message = "Apostas não enviadas !!" });
            }

            try
            {
                //_uow.BeginTransaction(); // Inicia a transação

                var apostas = _mapper.Map<List<Aposta>>(apostasViewModel);

                var contaApostaComEmpateOuVisitanteVencendo = 0;

                foreach (var aposta in apostas)
                {

                    var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
                    var apostaDb = await apostaRepository.ObterPorId(aposta.Id);

                    if (apostaDb == null)
                    {
                        _logger.LogWarning("Nenhuma aposta recebida no corpo da requisição (SalvarApostasNova).");
                        return Json(new { success = false, message = "Aposta não encontrada." });
                    }

                    apostaDb.PlacarApostaCasa = aposta.PlacarApostaCasa;
                    apostaDb.PlacarApostaVisita = aposta.PlacarApostaVisita;
                    apostaDb.DataHoraAposta = DateTime.Now;
                    apostaDb.Enviada = true;

                    //se apostar EMPATE ou VISITANTE VENCEDOR
                    if (apostaDb.PlacarApostaCasa == apostaDb.PlacarApostaVisita || apostaDb.PlacarApostaCasa < apostaDb.PlacarApostaVisita)
                    {
                        contaApostaComEmpateOuVisitanteVencendo = contaApostaComEmpateOuVisitanteVencendo + 1;
                    }

                    await apostaRepository.Atualizar(apostaDb);
                }

                //aqui TEM QUE BLOQUEAR SEM APAGAR AS APOSTAS, para que o USUÁRIO, CONSERTE
                if (contaApostaComEmpateOuVisitanteVencendo < 3)
                {
                    return Json(new { success = false, message = "Aposta INVÁLIDA - Uma APOSTA tem que ter pelo menos 3 resultados EMPATE ou VISITANTE VENCENDO" });
                }

                //_uow.Commit();
                await _uow.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                //_uow.Rollback(); // Desfaz a transação em caso de erro
                return Json(new { success = false, message = ex.Message });
            }
        }


        /*private async Task<RodadaViewModel> ObterRodadaAtiva()
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaAtiva());
            return rodada;
        }*/


        private async Task<ApostadorViewModel> ObterApostador(Guid idApostadorCampeonato)
        {
            var apostadorCampeonatoRepository = _uow.GetRepository<ApostadorCampeonato>() as ApostadorCampeonatoRepository; var apostadorCampeonato = _mapper.Map<ApostadorCampeonatoViewModel>
                                      (await apostadorCampeonatoRepository.ObterApostadorCampeonato(idApostadorCampeonato));

            var apostadorId = apostadorCampeonato.ApostadorId;

            var apostadorRepository = _uow.GetRepository<Apostador>() as ApostadorRepository;
            var apostador = _mapper.Map<ApostadorViewModel>(await apostadorRepository.ObterApostador(apostadorId));

            return apostador;
        }

        private async Task<ApostadorCampeonatoViewModel> ObterApostadorCampeonato(Guid idApostadorCampeonato)
        {
            var apostadorCampeonatoRepository = _uow.GetRepository<ApostadorCampeonato>() as ApostadorCampeonatoRepository;
            var apostadorCampeonato = _mapper.Map<ApostadorCampeonatoViewModel>
                                   (await apostadorCampeonatoRepository.ObterApostadorCampeonato(idApostadorCampeonato));
            return apostadorCampeonato;
        }

        /*private async Task CarregarDadosRodadaApostador(Guid idApostadorCampeonato)
        {
            var rodada = await ObterRodadaAtiva();
            TempData["Campeonato"] = rodada.Campeonato.Nome;
            TempData["Rodada"] = rodada.NumeroRodada;

            var apostador = await ObterApostador(idApostadorCampeonato);
            TempData["Apostador"] = apostador.Usuario.UserName;
            TempData["IdApostadorCampeonato"] = idApostadorCampeonato;
        }
*/
        private void DefinirDadosApostaEnviada(Domain.Models.Apostas.Aposta aposta)
        {
            if (aposta.Enviada)
            {
                TempData["DATA_APOSTA"] = aposta.DataHoraAposta.ToShortDateString();
                TempData["HORA_APOSTA"] = aposta.DataHoraAposta.ToShortTimeString();
                TempData["ENVIADA"] = "ENVIADA";
            }
            else
            {
                TempData["DATAHORA_APOSTA"] = "";
                TempData["ENVIADA"] = "AINDA NÃO ENVIADA";
            }
        }

        private string ObterPlacarAposta(int placar, bool enviada)
        {
            return placar == 0 && !enviada ? "" : placar.ToString();
        }


        protected void Notificar(string mensagem)
        {
            // Acessa o _notificador injetado na BaseController
            // Assumindo que a BaseController torna _notificador acessível através de uma propriedade protegida
            // Se _notificador for privado, você precisará adicionar um método na BaseController para isso.
            _notificador.Handle(new Notificacao(mensagem));
        }

    }
}