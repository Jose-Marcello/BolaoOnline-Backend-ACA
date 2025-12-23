// Localização: ApostasApp.Core.Web/Controllers/ApostadorController.cs
// NOVO controlador para lidar com dados gerais do apostador.
// Ele fornecerá os endpoints que o frontend Angular está a procurar.

using ApostasApp.Core.Application.DTOs.Apostadores;
using ApostasApp.Core.Application.DTOs.Campeonatos;
using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Application.DTOs.Rodadas;
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.Services.Interfaces.Apostadores;
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos;
using ApostasApp.Core.Application.Services.Interfaces.Rodadas;
using ApostasApp.Core.Application.Services.Interfaces.Usuarios; // Para IUsuarioService
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork (se ainda for necessário para DI, mas não para BaseController)
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Infrastructure.Data.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq; // Necessário para o método .Any()
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApostasApp.Core.Web.Controllers // Namespace CORRIGIDO para ApostasApp.Core.Web.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize] // Todos os métodos neste controlador exigirão autenticação por padrão, EXCETO os com [AllowAnonymous]
  public class ApostadorController : BaseController
  {
    private readonly IUsuarioService _usuarioService;
    private readonly IApostadorService _apostadorService;
    private readonly IApostadorCampeonatoService _apostadorCampeonatoService;
    private readonly IRodadaService _rodadaService;
    private readonly ICampeonatoService _campeonatoService;
    private readonly IMapper _mapper;

    public ApostadorController(
        INotificador notificador,
        // REMOVIDO: IUnitOfWork uow, pois BaseController não o recebe mais no construtor
        IUsuarioService usuarioService,
        IApostadorService apostadorService,
        IApostadorCampeonatoService apostadorCampeonatoService,
        IRodadaService rodadaService,
        ICampeonatoService campeonatoService,
        IMapper mapper)
        : base(notificador) // Passa apenas o notificador para a BaseController
    {
      _usuarioService = usuarioService;
      _apostadorService = apostadorService;
      _apostadorCampeonatoService = apostadorCampeonatoService;
      _rodadaService = rodadaService;
      _campeonatoService = campeonatoService;
      _mapper = mapper;
    }

    /// <summary>
            /// Obtém os dados detalhados do apostador logado, incluindo apelido e saldo.
            /// Corresponde a GET /api/Apostador/Dados
            /// </summary>
    [HttpGet("Dados")]
    [AllowAnonymous] // Permite acesso para usuários não logados (anonimamente)
    public async Task<IActionResult> GetDadosApostador()
    {
      var userId = ObterUsuarioIdLogado();

      if (string.IsNullOrEmpty(userId))
      {
        // 🛑 CORREÇÃO DEFINITIVA (usando ApostadorDto): CENÁRIO CONVIDADO 🛑
        // Retorna um DTO MOCKADO (Vazio/Padrão) para o Frontend, evitando o erro 401/400.
        var apostadorVazio = new ApostadorDto
        {
          // Inicializa campos necessários para não dar NullReferenceException no Frontend
          Apelido = "Convidado",
          Saldo = new SaldoDto { Valor = 0 } // Presumindo que SaldoDto existe e é necessário
                                             // Você pode precisar inicializar outros campos que o Dashboard espera (ex: Lista de Apostas)
        };

        return CustomResponse(apostadorVazio); // Retorna 200 OK
      }

      // CENÁRIO DE USUÁRIO LOGADO: Lógica de busca real
      var apostadorEntity = await _apostadorService.GetApostadorByUserIdAsync(userId);

      if (apostadorEntity == null)
      {
        NotificarAlerta("Dados do apostador não encontrados.", "DADOS_APOSTADOR_NAO_ENCONTRADOS");
        // Retorna o tipo esperado (ApostadorDto)
        return CustomResponse<ApostadorDto>();
      }

      // Mapeamento para o DTO de retorno correto
      var apostadorDto = _mapper.Map<ApostadorDto>(apostadorEntity);

      // Lógica de preenchimento (se necessário)
      if (apostadorDto.Saldo != null && string.IsNullOrEmpty(apostadorDto.Saldo.ApostadorId))
      {
        apostadorDto.Saldo.ApostadorId = apostadorEntity.Id.ToString();
      }

      return CustomResponse(apostadorDto); // Retorna o DTO real
    }



    /// <summary>
    /// Obtém todos os campeonatos ativos disponíveis para o usuário visualizar e potencialmente aderir.
    /// Corresponde a GET /api/Apostador/TodosCampeonatosDisponiveis
    /// </summary>
    [AllowAnonymous] // <--- AGORA ELE SERÁ REALMENTE ANÔNIMO
    [HttpGet("TodosCampeonatosDisponiveis")]
    public async Task<IActionResult> GetTodosCampeonatosDisponiveis()
    {
      string? userId = null; // Inicialize userId como null por padrão

      if (UsuarioEstaAutenticado()) // Método do BaseController
      {
        userId = ObterUsuarioIdLogado(); // Método do BaseController
      }

      var response = await _campeonatoService.GetAvailableCampeonatos(userId);

      // O serviço já deve ter adicionado notificações se algo deu errado
      if (!response.Success) // Se o serviço indicou falha
      {
        // As notificações já estarão em response.Notifications (NotificationDto)
        return CustomResponse(response);
      }

      // <<-- CORRIGIDO: Acessando response.Data antes de chamar Any() -->>
      if (response.Data == null || !response.Data.Any())
      {
        NotificarAlerta("Nenhum campeonato ativo disponível no momento.", "NENHUM_CAMPEONATO_DISPONIVEL");
        return CustomResponse<IEnumerable<CampeonatoDto>>(new List<CampeonatoDto>()); // Retorna lista vazia com alerta
      }

      return CustomResponse(response); // Retorna a resposta de sucesso do serviço
    }

    /// <summary>
    /// Obtém a lista de rodadas em destaque.
    /// Corresponde a GET /api/Apostador/RodadasEmDestaque
    /// </summary>
    [HttpGet("RodadasEmDestaque")]
    [AllowAnonymous] // <--- AGORA ELE SERÁ REALMENTE ANÔNIMO
    public async Task<IActionResult> GetRodadasEmDestaque()
    {
      var rodadasApiResponse = await _rodadaService.ObterRodadasEmDestaque(); // Retorna ApiResponse<IEnumerable<RodadaDto>>

      // O serviço já deve ter adicionado notificações se algo deu errado
      if (!rodadasApiResponse.Success)
      {
        return CustomResponse(rodadasApiResponse);
      }

      var rodadasDto = rodadasApiResponse.Data?.ToList(); // Mapeia o Data da ApiResponse para List<RodadaDto>

      if (rodadasDto == null || !rodadasDto.Any())
      {
        NotificarAlerta("Nenhuma rodada em destaque encontrada.", "NENHUMA_RODADA_DESTAQUE");
        return CustomResponse<IEnumerable<RodadaDto>>(new List<RodadaDto>()); // Retorna lista vazia com alerta
      }

      return CustomResponse(rodadasApiResponse); // Retorna a resposta de sucesso do serviço
    }

    [HttpPost("AderirCampeonato")]
    [Authorize] // Este método CONTINUA protegido por [Authorize]
    public async Task<IActionResult> AderirCampeonato([FromBody] VincularApostadorCampeonatoDto request)
    {
      var userIdString = ObterUsuarioIdLogado(); // Método do BaseController

      if (string.IsNullOrEmpty(userIdString))
      {
        // CORRIGIDO: Usando NotificarErro do BaseController
        NotificarErro("Usuário não autenticado ou token inválido.", "USUARIO_NAO_AUTENTICADO");
        return CustomResponse<bool>(); // Usa CustomResponse do BaseController
      }

      var apostadorEntity = await _apostadorService.GetApostadorByUserIdAsync(userIdString);
      if (apostadorEntity == null)
      {
        // CORRIGIDO: Usando NotificarAlerta do BaseController
        NotificarAlerta("Dados do apostador não encontrados.", "APOSTADOR_NAO_ENCONTRADO");
        return CustomResponse<bool>(); // Usa CustomResponse do BaseController
      }

      if (!string.IsNullOrEmpty(request.ApostadorId) && request.ApostadorId.ToUpper() != apostadorEntity.Id.ToString().ToUpper())
      {
        // CORRIGIDO: Usando NotificarErro do BaseController
        NotificarErro("ID do apostador na requisição inválido ou não corresponde ao usuário logado.", "ID_APOSTADOR_INVALIDO");
        return CustomResponse<bool>(); // Usa CustomResponse do BaseController
      }

      Guid campeonatoIdGuid;
      if (!Guid.TryParse(request.CampeonatoId, out campeonatoIdGuid))
      {
        // CORRIGIDO: Usando NotificarErro do BaseController
        NotificarErro("ID do Campeonato fornecido é inválido.", "ID_CAMPEONATO_INVALIDO");
        return CustomResponse<bool>(); // Usa CustomResponse do BaseController
      }

      var resultadoAdesao = await _campeonatoService.AderirCampeonatoAsync(apostadorEntity.Id, campeonatoIdGuid);

      return CustomResponse(resultadoAdesao); // Retorna a ApiResponse do serviço de forma consistente
    }


    // <<-- NOVO ENDPOINT -->>
    [HttpGet("apostador-campeonato-id/{campeonatoId}")]
    public async Task<ActionResult<ApiResponse<string>>> ObterApostadorCampeonatoId(string campeonatoId)
    {
      // O UserId é obtido do token de autenticação do usuário logado
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
      if (userIdClaim == null)
      {
        NotificarErro("Usuário não autenticado.");
        return CustomResponse();
      }

      if (!Guid.TryParse(campeonatoId, out Guid campeonatoIdGuid))
      {
        NotificarErro("ID do campeonato inválido.");
        return CustomResponse();
      }

      var result = await _apostadorService.ObterApostadorCampeonatoIdParaUsuarioECampeonato(userIdClaim.Value, campeonatoIdGuid);
      return CustomResponse(result);
    }



    [HttpPut("AtualizarPerfil")]
    public async Task<IActionResult> AtualizarPerfil([FromBody] UpdatePerfilRequestDto request)
    {
      var userId = ObterUsuarioIdLogado();
      if (string.IsNullOrEmpty(userId))
      {
        NotificarErro("Usuário não identificado ou token inválido.", "USUARIO_NAO_IDENTIFICADO");
        return CustomResponse();
      }

      // A lógica de negócio e persistência está agora no serviço
      var success = await _apostadorService.AtualizarPerfilAsync(userId, request);

      if (success)
      {
        NotificarSucesso("Perfil atualizado com sucesso.");
        return CustomResponse(true);
      }
      else
      {
        NotificarErro("Erro ao atualizar o perfil. Tente novamente.", "ERRO_ATUALIZACAO_PERFIL");
        return CustomResponse(false);
      }
    }
    // ... (todo o código da controller abaixo)




    [Authorize]
    [HttpPost("upload-foto")]
    public async Task<IActionResult> UploadFoto(IFormFile file)
    {
      if (file == null || file.Length == 0) return BadRequest("Nenhum arquivo enviado.");

      // 1. Corrigir Extensões (Adicionar .jfif)
      var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".jfif" };
      var extensao = Path.GetExtension(file.FileName).ToLower();
      if (!extensoesPermitidas.Contains(extensao))
        return BadRequest("Apenas imagens JPG, PNG ou JFIF são permitidas.");

      var fileName = $"{Guid.NewGuid()}{extensao}";

      // 2. Corrigir Caminho Físico (wwwroot/uploads/perfis)
      var folderName = Path.Combine("wwwroot", "uploads", "perfis");
      var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

      if (!Directory.Exists(pathToSave)) Directory.CreateDirectory(pathToSave);

      var fullPath = Path.Combine(pathToSave, fileName);

      // 3. Salvar o arquivo
      using (var stream = new FileStream(fullPath, FileMode.Create))
      {
        await file.CopyToAsync(stream);
      }

      // 4. Caminho para o Banco (URL que o Angular vai usar)
      var dbPath = $"/uploads/perfis/{fileName}";

      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var sucesso = await _apostadorService.AtualizarFotoPerfilAsync(userId, dbPath);

      if (sucesso) return Ok(new { success = true, fotoUrl = dbPath });

      return BadRequest("Erro ao atualizar o banco de dados.");
    }



    /*
    public async Task<bool> AtualizarFotoPerfil(string userId, string dbPath)
    {
      var apostador = await _apostadorService.ObterPorId(Guid.Parse(userId));
      if (apostador == null) return false;

      // GRAVA APENAS O CAMINHO (IGUAL AOS ESCUDOS)
      apostador.Usuario.FotoPerfil = dbPath;

      //_apostadorService.Atualizar(apostador);

      //return await _apostadorService.Atualizar(apostador);

      // 2.Executa a atualização(que é void, então não retorna nada)
      await _apostadorService.Atualizar(apostador);

      // 3. Se chegou aqui sem dar exceção, retornamos true
      return true;
     

    }
    */
  }

}
