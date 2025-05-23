using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Interfaces.Jogos;
using ApostasApp.Core.Domain.Models;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Models.Interfaces.Usuarios;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.Notificacoes;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Domain.Models.Usuarios;
using ApostasApp.Core.Domain.Services.Apostadores;
using ApostasApp.Core.Domain.Services.Apostas;
using ApostasApp.Core.Domain.Services.RankingRodadas;
using ApostasApp.Core.Infrastructure;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.InfraStructure.Data.Repository.Apostadores;
using ApostasApp.Core.InfraStructure.Data.Repository.Campeonatos;
using ApostasApp.Core.Presentation.ViewModels;
using ApostasApp.Infrastructure.Data.Repository;
using DApostasApp.Core.Domain.Models.RankingRodadas; // Verifique se o namespace está correto. Se DApostadorApp, corrija.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApostasApp.Core.Presentation.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IUnitOfWork _uow;
        private readonly IUsuarioService _usuarioService;
        private readonly IApostadorService _apostadorService;
        private readonly IApostaService _apostaService;
        private readonly IRankingRodadaService _rankingRodadaService;
        private readonly IApostadorCampeonatoService _apostadorCampeonatoService;
        private readonly INotificador _notificador;


        public AccountController(
                UserManager<Usuario> userManager,
                SignInManager<Usuario> signInManager,
                ILogger<AccountController> logger,
                IUnitOfWork uow,
                IUsuarioService usuarioService,
                IApostaService apostaService,
                IRankingRodadaService rankingRodadaService,
                IApostadorCampeonatoService apostadorCampeonatoService,
                IApostadorService apostadorService,
                INotificador notificador) : base(notificador)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _uow = uow;
            _usuarioService = usuarioService;
            _apostaService = apostaService;
            _rankingRodadaService = rankingRodadaService;
            _apostadorService = apostadorService;
            _apostadorCampeonatoService = apostadorCampeonatoService;
            _notificador = notificador;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            try
            {
                var user = new Usuario
                {
                    UserName = model.Email,
                    Email = model.Email,
                    CPF = model.CPF.Replace(".", "").Replace("-", ""),
                    Celular = model.Celular,
                    Apelido = model.Apelido,
                    RegistrationDate = DateTime.Now,
                    LastLoginDate = null
                };

                ViewData["ReturnUrl"] = returnUrl;

                if (ModelState.IsValid)
                {
                    //Usuario
                    if (await _usuarioService.ApelidoExiste(model.Apelido))
                    {
                        ModelState.AddModelError("Apelido", "Já existe um USUÁRIO registrado com este APELIDO.");
                        return View(model);
                    }

                    var result = await _usuarioService.RegisterUserAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        user = await _usuarioService.FindByEmailAsync(model.Email);

                        var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
                        var campeonato = await campeonatoRepository.ObterCampeonatoAtivo();

                        //Apostador
                        var statusApostador = StatusApostador.AguardandoAssociacao;

                        if (campeonato != null)
                        {
                            statusApostador = StatusApostador.AssociadoACampeonato;
                        }

                        var apostador = new Apostador
                        {
                            UsuarioId = user.Id,
                            Status = statusApostador
                        };

                        await _apostadorService.Adicionar(apostador);

                        //ApostadorCampeonato
                        if (campeonato != null)
                        {
                            var apostadorCampeonato = new ApostadorCampeonato
                            {
                                ApostadorId = apostador.Id,
                                CampeonatoId = campeonato.Id
                            };

                            await _apostadorCampeonatoService.Adicionar(apostadorCampeonato);

                            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
                            var rodadaEmApostas = await rodadaRepository.ObterRodadaEmApostas();

                            // Se não houver rodada em apostas, tenta obter a rodada corrente
                            Rodada rodadaCorrente = null;
                            if (rodadaEmApostas == null)
                            {
                                rodadaCorrente = await rodadaRepository.ObterRodadaCorrente();
                            }

                            if (rodadaEmApostas != null && rodadaEmApostas.Status == StatusRodada.EmApostas)
                            {
                                // Lógica de criação de apostas e ranking para a rodada EmApostas
                                var jogodorRepository = _uow.GetRepository<Jogo>() as IJogoRepository;
                                var listaJogos = await jogodorRepository.ObterJogosDaRodada(rodadaEmApostas.Id);

                                foreach (var jogo in listaJogos)
                                {
                                    try
                                    {
                                        var aposta = new Aposta
                                        {
                                            JogoId = jogo.Id,
                                            ApostadorCampeonatoId = apostadorCampeonato.Id,
                                            Enviada = false
                                        };
                                        System.Diagnostics.Debug.WriteLine($" Aposta : {aposta.Id}");
                                        System.Diagnostics.Debug.WriteLine($" JogoId: {aposta.JogoId}");
                                        System.Diagnostics.Debug.WriteLine($" ApostadorCampeonatoId: {aposta.ApostadorCampeonatoId}");
                                        System.Diagnostics.Debug.WriteLine($" Enviada : {aposta.Enviada}");

                                        await _apostaService.Adicionar(aposta);
                                    }
                                    catch (DbUpdateException ex)
                                    {
                                        Console.WriteLine($"Erro ao adicionar aposta (DbUpdateException): {ex.Message}");
                                        Console.WriteLine(ex);
                                        throw;
                                    }
                                    catch (ObjectDisposedException ex)
                                    {
                                        Console.WriteLine($"Erro ao adicionar aposta (ObjectDisposedException): {ex.Message}");
                                        Console.WriteLine(ex);
                                        throw;
                                    }
                                    catch (System.IO.IOException ex)
                                    {
                                        Console.WriteLine($"Erro ao adicionar aposta (IOException): {ex.Message}");
                                        Console.WriteLine(ex);
                                        throw;
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Erro ao adicionar aposta: {ex.Message}");
                                        Console.WriteLine(ex);
                                        throw;
                                    }
                                }

                                var ranking = new RankingRodada
                                {
                                    ApostadorCampeonatoId = apostadorCampeonato.Id,
                                    RodadaId = rodadaEmApostas.Id,
                                    DataAtualizacao = DateTime.Now,
                                    Posicao = 0,
                                    Pontuacao = 0
                                };

                                await _rankingRodadaService.Adicionar(ranking);
                            }
                        }

                        await _uow.SaveChanges();

                        var code = await _usuarioService.GenerateEmailConfirmationTokenAsync(user);
                        var codeEncoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Action("ConfirmEmail", "Account",
                            new { userId = user.Id, code = codeEncoded }, Request.Scheme);

                        await _usuarioService.SendConfirmationEmailAsync(user, callbackUrl);

                        ViewBag.Email = model.Email;

                        return View("EmailConfirmationPending"); // Continua para a tela de confirmação de e-mail
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao salvar dados no banco de dados.");
                return BadRequest(new { error = "Erro ao salvar dados." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argumento inválido.");
                return BadRequest(new { error = "Argumento inválido." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar o usuário.");
                return BadRequest(new { error = ex.Message });
            }

            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await _usuarioService.FindByIdAsync(userId);

            if (user == null)
            {
                return View("Error");
            }

            code = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(code));
            var result = await _usuarioService.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                await _uow.SaveChanges();
                // E-mail confirmado com sucesso! Agora, redirecione PARA LOGIN
                return RedirectToAction("Login", "Account");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError($"Erro na confirmação de e-mail: {error.Code} - {error.Description}");
                }
                return View("Error");
            }
        }

        public IActionResult ConfirmEmailConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _usuarioService.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    if (!user.EmailConfirmed)
                    {
                        ViewBag.EmailNaoConfirmado = "Seu e-mail ainda não foi confirmado. Por favor, verifique sua caixa de entrada (e spam) ou solicite um novo e-mail abaixo.";
                        ViewBag.EmailParaReenvio = model.Email;
                        return View(model);
                    }

                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        var apostadorRepository = _uow.GetRepository<Apostador>() as ApostadorRepository;
                        var apostador = await apostadorRepository.ObterApostadorPorUsuarioId(user.Id);

                        var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
                        var rodadaEmApostas = await rodadaRepository.ObterRodadaEmApostas(); // Tenta obter a rodada "Em Apostas"
                        Rodada rodadaCorrente = null;

                        if (rodadaEmApostas == null) // Se não encontrou rodada em apostas, tenta a rodada corrente
                        {
                            rodadaCorrente = await rodadaRepository.ObterRodadaCorrente();
                        }

                        // O campeonato é pego da rodada que foi encontrada primeiro (EmApostas tem prioridade)
                        var campeonato = (rodadaEmApostas ?? rodadaCorrente)?.Campeonato;

                        if (apostador != null && campeonato != null)
                        {
                            var apostadorCampeonatoRepository = _uow.GetRepository<ApostadorCampeonato>() as ApostadorCampeonatoRepository;
                            var apostadorCampeonato = await apostadorCampeonatoRepository.ObterApostadorCampeonatoPorApostadorECampeonato(user.Id, campeonato.Id);

                            if (apostadorCampeonato != null)
                            {
                                if (rodadaEmApostas != null && rodadaEmApostas.Status == StatusRodada.EmApostas)
                                {
                                    // Redireciona para a Action de edição de apostas para a rodada "Em Apostas"
                                    return RedirectToAction("ListarApostasDoApostadorNaRodadaEmApostas", "ApostadorCampeonato", new { id = apostadorCampeonato.Id });
                                }
                                else if (rodadaCorrente != null && rodadaCorrente.Status == StatusRodada.Corrente)
                                {
                                    // Redireciona para a Action de visualização para a rodada "Corrente"
                                    // Esta Action deve aceitar o ID do ApostadorCampeonato. A rodada será buscada internamente.
                                    return RedirectToAction("ListarApostasDoApostadorNaRodadaCorrente", "ApostadorCampeonato", new { id = apostadorCampeonato.Id });
                                }
                                else
                                {
                                    // Se não encontrou nenhuma rodada "Em Apostas" ou "Corrente", vai para o painel
                                    return RedirectToAction("PainelUsuario", "Account");
                                }
                            }
                            else
                            {
                                // Se não encontrou ApostadorCampeonato, redireciona para o PainelUsuario
                                return RedirectToAction("PainelUsuario", "Account");
                            }
                        }
                        else
                        {
                            // Se não encontrou apostador, campeonato ou nenhuma rodada ativa (EmApostas ou Corrente), 
                            // redireciona para o PainelUsuario
                            return RedirectToAction("PainelUsuario", "Account");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Senha Inválida.");
                        ViewBag.EmailNaoConfirmado ??= string.Empty;
                        ViewBag.EmailParaReenvio ??= string.Empty;
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuário Inválido.");
                    ViewBag.EmailNaoConfirmado ??= string.Empty;
                    ViewBag.EmailParaReenvio ??= string.Empty;
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Dados de login inválidos.");
                ViewBag.EmailNaoConfirmado ??= string.Empty;
                ViewBag.EmailParaReenvio ??= string.Empty;
                return View(model);
            }

            ViewBag.EmailNaoConfirmado ??= string.Empty;
            ViewBag.EmailParaReenvio ??= string.Empty;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet("Account/EsqueciSenha")]
        [Route("Account/EsqueciSenha")]
        public IActionResult EsqueciSenha()
        {
            return View(new EsqueciSenhaViewModel());
        }

        [HttpPost("Account/EsqueciSenha")]
        [Route("Account/EsqueciSenha")]
        public async Task<IActionResult> EsqueciSenha(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Message = "O e-mail é obrigatório.";
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            string callbackUrl = null;

            if (user != null)
            {
                string tokenGerado = await _userManager.GeneratePasswordResetTokenAsync(user);
                _logger.LogInformation($"Token de redefinição de senha gerado para o usuário {user.Id}: {tokenGerado}");

                callbackUrl = Url.Action("RedefinirSenha", "Account", new { userId = user.Id, token = tokenGerado }, protocol: HttpContext.Request.Scheme);
            }

            await _usuarioService.EsqueciMinhaSenhaAsync(email, callbackUrl);

            ViewBag.Message = "Um email com instruções para redefinir sua senha foi enviado para você (se o email estiver cadastrado).";
            return View();
        }

        [HttpGet("RedefinirSenha")]
        [AllowAnonymous]
        public async Task<IActionResult> RedefinirSenha(string userId = null, string token = null)
        {
            _logger.LogInformation($"Token de redefinição de senha recebido para o usuário {userId}: {token}");
            if (userId == null || token == null)
            {
                ViewBag.ErrorMessage = "Link de redefinição de senha inválido.";
                return View("RedefinirSenhaErro");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Usuário inválido.";
                return View("RedefinirSenhaErro");
            }

            var model = new RedefinirSenhaViewModel
            {
                UserId = userId,
                Token = token,
            };

            return View(model);
        }

        [HttpPost("RedefinirSenha")]
        [AllowAnonymous]
        public async Task<IActionResult> RedefinirSenha(RedefinirSenhaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user != null)
                {
                    ViewBag.UserId = user.Id;
                    ViewBag.Token = model.Token;

                    _logger.LogInformation($"Tentando redefinir senha para o usuário com ID: {user.Id}");
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    if (result.Succeeded)
                    {
                        await _uow.SaveChanges();

                        _logger.LogInformation($"Senha redefinida com sucesso para o usuário com ID: {user.Id}");
                        await _signInManager.SignOutAsync();
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        _logger.LogError($"Erro ao redefinir senha para o usuário com ID: {user.Id}. Falha em ResetPasswordAsync.");
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError($"Erro de redefinição de senha: Código = {error.Code}, Descrição = {error.Description}");
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuário não encontrado.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ResendEmailConfirmation(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "O e-mail é obrigatório.");
                return View("Login");
            }

            await _usuarioService.ResendEmailConfirmationAsync(email);

            if (_notificador.TemNotificacao())
            {
                foreach (var notificacao in _notificador.ObterNotificacoes())
                {
                    ModelState.AddModelError(string.Empty, notificacao.Mensagem);
                }
            }
            else
            {
                ViewBag.Message = "Um novo e-mail de confirmação foi enviado.";
            }

            return View("Login");
        }

        [HttpGet("PainelUsuario")]
        public async Task<IActionResult> PainelUsuario()
        {
            var campeonato = await ObterCampeonatoAtivo();

            if (campeonato != null)
            {
                var userId = await _usuarioService.GetLoggedInUserId();

                var apostadorCampeonato = await ObterApostadorCampeonato(userId, campeonato.Id);

                var painelUsuarioViewModel = new PainelUsuarioViewModel
                {
                    ApostadorCampeonatoId = apostadorCampeonato?.Id ?? Guid.Empty
                };

                var usuario = await _usuarioService.ObterUsuarioPorId(userId);

                if (usuario != null)
                {
                    ViewBag.NomeUsuario = usuario.Apelido;
                }
                else
                {
                    ViewBag.NomeUsuario = "Usuário";
                }

                return View(painelUsuarioViewModel);
            }
            else
            {
                TempData["ApostadorCampeonatoId"] = null;
                ViewBag.NomeUsuario = "Usuário";
            }

            return View();
        }

        private async Task<Campeonato> ObterCampeonatoAtivo()
        {
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            var campeonato = await campeonatoRepository.ObterCampeonatoAtivo();

            return campeonato;
        }

        private async Task<Domain.Models.Campeonatos.ApostadorCampeonato> ObterApostadorCampeonato(string usuarioId, Guid campeonatoId)
        {
            var apostadorCampeonatoRepository = _uow.GetRepository<ApostadorCampeonato>() as ApostadorCampeonatoRepository;
            var apostadorCampeonato = await apostadorCampeonatoRepository.ObterApostadorCampeonatoPorApostadorECampeonato(usuarioId, campeonatoId);

            return apostadorCampeonato;
        }
    }
}
