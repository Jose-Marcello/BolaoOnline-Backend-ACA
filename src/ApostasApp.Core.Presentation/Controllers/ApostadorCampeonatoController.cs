using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Models;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Models.Interfaces.Usuarios;
using ApostasApp.Core.Domain.Models.Notificacoes;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Domain.Services.Apostas;
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
using Microsoft.SqlServer.Server;
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
        private readonly IApostaService _apostaService;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ApostadorCampeonatoController> _logger;
        private readonly INotificador _notificador;

        public ApostadorCampeonatoController(IMapper mapper,
                                   IApostadorCampeonatoService apostadorCampeonatoService,
                                   IUsuarioService usuarioService,
                                   IApostaService apostaService,
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
                _apostaService = apostaService;
                _uow = uow;
                _logger = logger;
                 _notificador = notificador;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no CONSTRUTOR DO APOSTADORCAMPEONATO.");
            }

        }


        // =========================================================================================================
        // CENÁRIO 1: RODADA EM APOSTAS (Onde o usuário pode fazer/editar as apostas)
        // =========================================================================================================

        // Action GET para exibir a tela de apostas para a rodada "Em Apostas"
        // Não recebe rodadaId, pois a rodada é determinada pelo status
        //[HttpGet("ApostadorCampeonato/ListarApostasDoApostadorNaRodadaEmApostas/{apostadorCampeonatoId}")]
        [HttpGet("RodadaEmApostas/{id}")]       
        public async Task<IActionResult> ListarApostasDoApostadorNaRodadaEmApostas(Guid Id)
        {
            
            var apostadorCampeonato = await ObterApostadorCampeonato(Id);

            if (apostadorCampeonato == null)
            {
                Notificar("Apostador Campeonato não encontrado.");
                return RedirectToAction("PainelUsuario", "Account");
            }

            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodadaEmApostas(); // Busca a rodada com status "EmApostas"

            if (rodada == null)
            {
                Notificar("Nenhuma rodada 'Em Apostas' encontrada.");
                return RedirectToAction("PainelUsuario", "Account");
            }

            var usuario = await _usuarioService.GetLoggedInUser();
            if (usuario == null)
            {
                Notificar("Usuário não logado.");
                return RedirectToAction("Login", "Account");
            }

            // Buscar informações de status e data/hora da aposta para a rodada
            var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
            var verifApostasNaRodada = await apostaRepository.ObterApostaSalvaDoApostadorNaRodada(rodada.Id, apostadorCampeonato.Id);

            var statusEnvio = "NÃO ENVIADA";
            string dataApostaFormatada = "";
            string horaApostaFormatada = "";

            if (verifApostasNaRodada != null && verifApostasNaRodada.Enviada && verifApostasNaRodada.DataHoraAposta.HasValue)
            {
                statusEnvio = "ENVIADA";
                dataApostaFormatada = verifApostasNaRodada.DataHoraAposta.Value.ToShortDateString();
                horaApostaFormatada = verifApostasNaRodada.DataHoraAposta.Value.ToShortTimeString();
            }

            var model = new ApostasPorRodadaEApostadorViewModel
            {
                ApostadorCampeonatoId = apostadorCampeonato.Id,
                RodadaId = rodada.Id, // Passa o ID da rodada encontrada para o ViewModel
                ApostadorApelido = usuario.Apelido,
                CampeonatoNome = rodada.Campeonato?.Nome ?? "N/A",
                NumeroRodada = rodada.NumeroRodada,
                StatusEnvioAposta = statusEnvio,
                DataAposta = dataApostaFormatada,
                HoraAposta = horaApostaFormatada
            };

            return View("ListarApostasDoApostadorNaRodadaEmApostas", model); // Aponta para a View específica
        }

        // Action POST para alimentar o DataTable da rodada "Em Apostas" (via AJAX)
        // AGORA RECEBE rodadaId como parâmetro, pois já foi determinado na Action GET       
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
                   

        // =========================================================================================================
        // CENÁRIO 2: RODADA CORRENTE (Onde o usuário vê os placares reais e suas apostas, mas não edita)
        // =========================================================================================================

        // Action GET para exibir a tela de apostas para a rodada "Corrente"
        // Não recebe rodadaId, pois a rodada é determinada pelo status
        //[HttpGet("ApostadorCampeonato/ListarApostasDoApostadorNaRodadaCorrente/{apostadorCampeonatoId}")]
        [HttpGet("/ListarApostasDoApostadorNaRodadaCorrente/{id}")]
        public async Task<IActionResult> ListarApostasDoApostadorNaRodadaCorrente(Guid id)
        {
            var apostadorCampeonato = await ObterApostadorCampeonato(id);

            if (apostadorCampeonato == null)
            {
                Notificar("Apostador Campeonato não encontrado.");
                return RedirectToAction("PainelUsuario", "Account");
            }

            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodadaCorrente(); // Busca a rodada com status "Corrente"

            if (rodada == null)
            {
                Notificar("Nenhuma rodada 'Corrente' encontrada.");
                return RedirectToAction("PainelUsuario", "Account");
            }

            var usuario = await _usuarioService.GetLoggedInUser();

            if (usuario == null)
            {
                Notificar("Usuário não logado.");
                return RedirectToAction("Login", "Account");
            }

            // Buscar informações de status e data/hora da aposta para a rodada
            var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
            var verifApostasNaRodada = await apostaRepository.ObterApostaSalvaDoApostadorNaRodada(rodada.Id, apostadorCampeonato.Id);

            var statusEnvio = "NÃO ENVIADA";
            string dataApostaFormatada = "";
            string horaApostaFormatada = "";

            if (verifApostasNaRodada != null && verifApostasNaRodada.Enviada && verifApostasNaRodada.DataHoraAposta.HasValue)
            {
                statusEnvio = "ENVIADA";
                dataApostaFormatada = verifApostasNaRodada.DataHoraAposta.Value.ToShortDateString();
                horaApostaFormatada = verifApostasNaRodada.DataHoraAposta.Value.ToShortTimeString();
            }

            var model = new ApostasPorRodadaEApostadorViewModel
            {
                ApostadorCampeonatoId = apostadorCampeonato.Id,
                RodadaId = rodada.Id, // Passa o ID da rodada encontrada
                ApostadorApelido = usuario.Apelido,
                CampeonatoNome = rodada.Campeonato?.Nome ?? "N/A",
                NumeroRodada = rodada.NumeroRodada,
                StatusEnvioAposta = statusEnvio,
                DataAposta = dataApostaFormatada,
                HoraAposta = horaApostaFormatada
            };

            return View("ListarApostasDoApostadorNaRodadaCorrente", model); // Aponta para a View específica
        }

        // Action POST para alimentar o DataTable da rodada "Corrente" (via AJAX)
        // Não recebe rodadaId, pois a rodada é determinada pelo status        
        [HttpPost("BuscarApostasDoApostadorNaRodadaCorrente/{id}")]
        public async Task<JsonResult> BuscarApostasDoApostadorNaRodadaCorrente(Guid id)
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodadaCorrente(); // Busca a rodada com status "Corrente"

            if (rodada == null)
            {
                return Json(new { data = new List<object>() }); // Retorna lista vazia se não houver rodada
            }

            var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
            var listaApostas = await apostaRepository.ObterApostasDoApostadorNaRodada(rodada.Id, id);

            // Estrutura de dados para o DataTable "Corrente" (apenas exibição) - IGUAL À SELECIONADA
            var data = listaApostas.Select(aposta => new
            {
                Id = aposta.Id,
                IdJogo = aposta.Jogo.Id,
                EquipeMandante = aposta.Jogo.EquipeCasa.Equipe.Nome,
                SiglaMandante = aposta.Jogo.EquipeCasa.Equipe.Sigla,
                EscudoMandante = aposta.Jogo.EquipeCasa.Equipe.Escudo,
                PlacarRealCasa = aposta.Jogo.PlacarCasa, // Placar real
                PlacarApostaCasa = aposta.PlacarApostaCasa, // Placar da aposta
                EquipeVisitante = aposta.Jogo.EquipeVisitante.Equipe.Nome,
                SiglaVisitante = aposta.Jogo.EquipeVisitante.Equipe.Sigla,
                EscudoVisitante = aposta.Jogo.EquipeVisitante.Equipe.Escudo,
                PlacarRealVisita = aposta.Jogo.PlacarVisita, // Placar real
                PlacarApostaVisita = aposta.PlacarApostaVisita, // Placar da aposta
                DataJogo = aposta.Jogo.DataJogo.ToString("yyyy-MM-dd"),
                HoraJogo = aposta.Jogo.HoraJogo.ToString(@"hh\:mm"),
                StatusJogo = aposta.Jogo.Status,
                Enviada = aposta.Enviada,
                Pontuacao = aposta.Pontos
            }).ToList();

            return Json(new { data });
        }
                

        // =========================================================================================================
        // CENÁRIO 3: RODADA SELECIONADA (Onde o usuário vê os placares reais e suas apostas, mas não edita)
        // =========================================================================================================

        // Action GET para exibir a tela de apostas para uma rodada selecionada
        // Recebe rodadaId como parâmetro       
        [HttpGet("ListarApostasDoApostadorNaRodada/{apostadorCampeonatoId}/{rodadaId}")]
        public async Task<IActionResult> ListarApostasDoApostadorNaRodadaSelecionada(Guid apostadorCampeonatoId, Guid rodadaId)
        {
            var apostadorCampeonato = await ObterApostadorCampeonato(apostadorCampeonatoId); 

            if (apostadorCampeonato == null)
            {
                Notificar("Apostador Campeonato não encontrado.");
                return RedirectToAction("PainelUsuario", "Account");
            }

            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodada(rodadaId); // Busca a rodada pelo ID

            if (rodada == null)
            {
                Notificar("Rodada selecionada não encontrada.");
                return RedirectToAction("PainelUsuario", "Account");
            }

            var usuario = await _usuarioService.GetLoggedInUser();
            if (usuario == null)
            {
                Notificar("Usuário não logado.");
                return RedirectToAction("Login", "Account");
            }

            // Buscar informações de status e data/hora da aposta para a rodada
            var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
            var verifApostasNaRodada = await apostaRepository.ObterApostaSalvaDoApostadorNaRodada(rodada.Id, apostadorCampeonato.Id);

            var statusEnvio = "NÃO ENVIADA";
            string dataApostaFormatada = "";
            string horaApostaFormatada = "";

            if (verifApostasNaRodada != null && verifApostasNaRodada.Enviada && verifApostasNaRodada.DataHoraAposta.HasValue)
            {
                statusEnvio = "ENVIADA";
                dataApostaFormatada = verifApostasNaRodada.DataHoraAposta.Value.ToShortDateString();
                horaApostaFormatada = verifApostasNaRodada.DataHoraAposta.Value.ToShortTimeString();
            }

            var model = new ApostasPorRodadaEApostadorViewModel
            {
                ApostadorCampeonatoId = apostadorCampeonatoId,
                RodadaId = rodada.Id, // Passa o ID da rodada encontrada
                ApostadorApelido = usuario.Apelido,
                CampeonatoNome = rodada.Campeonato.Nome,
                NumeroRodada = rodada.NumeroRodada,
                StatusEnvioAposta = statusEnvio,
                DataAposta = dataApostaFormatada,
                HoraAposta = horaApostaFormatada
            };

            return View("ListarApostasDoApostadorNaRodadaSelecionada", model); // Aponta para a View específica
        }

        // Action POST para alimentar o DataTable da rodada "Selecionada" (via AJAX)
        // Recebe rodadaId como parâmetro
        [HttpPost("BuscarApostasDoApostadorNaRodadaSelecionada/{apostadorCampeonatoId}/{rodadaId}")]        
        public async Task<JsonResult> BuscarApostasDoApostadorNaRodadaSelecionada(Guid apostadorCampeonatoId, Guid rodadaId)
        {
            var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
            var listaApostas = await apostaRepository.ObterApostasDoApostadorNaRodada(rodadaId, apostadorCampeonatoId);

            // Estrutura de dados para o DataTable "Selecionada" (apenas exibição) - IGUAL À CORRENTE
            var data = listaApostas.Select(aposta => new
            {
                Id = aposta.Id,
                IdJogo = aposta.Jogo.Id,
                EquipeMandante = aposta.Jogo.EquipeCasa.Equipe.Nome,
                SiglaMandante = aposta.Jogo.EquipeCasa.Equipe.Sigla,
                EscudoMandante = aposta.Jogo.EquipeCasa.Equipe.Escudo,
                PlacarRealCasa = aposta.Jogo.PlacarCasa, // Placar real
                PlacarApostaCasa = aposta.PlacarApostaCasa, // Placar da aposta
                EquipeVisitante = aposta.Jogo.EquipeVisitante.Equipe.Nome,
                SiglaVisitante = aposta.Jogo.EquipeVisitante.Equipe.Sigla,
                EscudoVisitante = aposta.Jogo.EquipeVisitante.Equipe.Escudo,
                PlacarRealVisita = aposta.Jogo.PlacarVisita, // Placar real
                PlacarApostaVisita = aposta.PlacarApostaVisita, // Placar da aposta
                DataJogo = aposta.Jogo.DataJogo.ToString("yyyy-MM-dd"),
                HoraJogo = aposta.Jogo.HoraJogo.ToString(@"hh\:mm"),
                StatusJogo = aposta.Jogo.Status,
                Enviada = aposta.Enviada,
                Pontuacao = aposta.Pontos
            }).ToList();

            return Json(new { data });
        }
              

        [HttpPost("ApostadorCampeonato/BuscarStatusEDataHoraApostaDaRodada")]
        public async Task<JsonResult> BuscarStatusEDataHoraApostaDaRodada([FromForm] Guid apostadorCampeonatoId, [FromForm] Guid rodadaId)
        {
            try
            {
                var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
                var aposta = await apostaRepository.ObterApostaSalvaDoApostadorNaRodada(rodadaId, apostadorCampeonatoId);

                if (aposta != null)
                {
                    return Json(new
                    {
                        aposta = new
                        {
                            enviada = aposta.Enviada,
                            dataHoraAposta = aposta.DataHoraAposta?.ToString("o") // Formato ISO 8601 para facilitar a conversão no JS
                        }
                    });
                }
                else
                {
                    return Json(new { aposta = new { enviada = false, dataHoraAposta = (string)null } });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao buscar status e data/hora da aposta: {ex.Message}");
                return Json(new { aposta = new { enviada = false, dataHoraAposta = (string)null }, error = ex.Message });
            }
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
        public async Task<JsonResult> SalvarApostas([FromBody] List<SalvarApostaViewModel> apostasViewModel)        
        {

            // Obter a rodada associada às apostas 
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodadaEmApostas();

            // Verificar o status da rodada IMEDIATAMENTE antes de salvar
            if (rodada.Status != StatusRodada.EmApostas) // Substitua "Aberto" pelo status correto
            {
                // Rodada foi fechada PARA APOSTAS, impedir a alteração dos resultados
                Notificar("As apostas para esta RODADA foram encerradas.");               
                return Json(new { success = false }); // , message = "As apostas para esta RODADA foram encerradas." });

            }

            _logger.LogInformation("Iniciando a ação SalvarApostas.");

            if (apostasViewModel.IsNullOrEmpty())
            {
                Notificar("Apostas não enviadas !!");
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
                        Notificar("Apostas não encontradas !!");
                        return Json(new { success = false }); //, message = "Aposta não encontrada." });
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
                    Notificar("Aposta INVÁLIDA - Uma APOSTA tem que ter pelo menos 3 resultados EMPATE ou VISITANTE VENCENDO !!");
                    return Json(new { success = false }); //, message = "Aposta INVÁLIDA - Uma APOSTA tem que ter pelo menos 3 resultados EMPATE ou VISITANTE VENCENDO" });
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

            // Verifica se DataHoraAposta tem um valor antes de tentar formatar
            if (aposta.Enviada)
            {
                TempData["DATA_APOSTA"] = aposta.DataHoraAposta.Value.ToShortDateString();
                TempData["HORA_APOSTA"] = aposta.DataHoraAposta.Value.ToShortTimeString();
                TempData["ENVIADA"] = "ENVIADA";
            }
            else
            {
                // Caso seja enviada mas a data seja nula (situação que deve ser evitada na lógica de negócio)
                TempData["DATA_APOSTA"] = "Data Indisponível";
                TempData["HORA_APOSTA"] = "Hora Indisponível";
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