using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Notificacoes; // Para Notificacao (domínio)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding; // Para ModelStateDictionary
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; // Necessário para acessar Claims

namespace ApostasApp.Core.Web.Controllers
{
  [ApiController]
  public abstract class BaseController : ControllerBase
  {
    private readonly INotificador _notificador;

    protected BaseController(INotificador notificador)
    {
      _notificador = notificador;
    }

    // Propriedade para verificar se há notificações de erro
    protected bool OperacaoValida()
    {
      // Uma operação é considerada válida se NÃO houver notificações do TIPO "Erro"
      return !_notificador.ObterNotificacoes().Any(n => n.Tipo == "Erro");
    }

    // Método para notificar o sistema de domínio a partir do controller
    protected void NotificarErro(string mensagem, string? codigo = null, string? nomeCampo = null)
    {
      _notificador.Handle(new Notificacao(codigo, "Erro", mensagem, nomeCampo));
    }

    protected void NotificarSucesso(string mensagem, string? codigo = null, string? nomeCampo = null)
    {
      _notificador.Handle(new Notificacao(codigo, "Sucesso", mensagem, nomeCampo));
    }

    protected void NotificarAlerta(string mensagem, string? codigo = null, string? nomeCampo = null)
    {
      _notificador.Handle(new Notificacao(codigo, "Alerta", mensagem, nomeCampo));
    }

    // SOBRECARGA 1: Processa um ApiResponse<T> já existente (vindo de um serviço)
    protected ActionResult CustomResponse<T>(ApiResponse<T> serviceResponse)
    {
      var allNotifications = new List<NotificationDto>();

      if (serviceResponse.Notifications != null)
      {
        allNotifications.AddRange(serviceResponse.Notifications);
      }

      var controllerNotifications = _notificador.ObterNotificacoes()
                           .Select(n => new NotificationDto
                           {
                             Codigo = n.Codigo,
                             Tipo = n.Tipo,
                             Mensagem = n.Mensagem,
                             NomeCampo = n.NomeCampo
                           })
                           .ToList();
      if (controllerNotifications.Any())
      {
        allNotifications.AddRange(controllerNotifications);
      }

      _notificador.LimparNotificacoes();

      bool hasErrors = allNotifications.Any(n => n.Tipo == "Erro");

      if (hasErrors)
      {
        return BadRequest(ApiResponse<T>.CreateError(
          serviceResponse.Message ?? "Ocorreram erros na sua requisição.",
          allNotifications
        ));
      }
      else
      {
        // CORREÇÃO CRÍTICA FINAL: Forçamos o retorno 200 OK com o corpo JSON.
        var finalResponse = ApiResponse<T>.CreateSuccess(
            serviceResponse.Data,
            serviceResponse.Message ?? "Operação realizada com sucesso.",
            allNotifications
        );

        // Retorna Status 200 explicitamente, contornando a otimização 204.
        return StatusCode(200, finalResponse);
      }
    }

    // ... (O restante das sobrecargas do CustomResponse permanece como no arquivo fornecido) ...

    // SOBRECARGA 2: Constrói um ApiResponse<T> a partir de um objeto de dados T (para sucesso)
    protected ActionResult CustomResponse<T>(T data)
    {
      var allNotifications = _notificador.ObterNotificacoes()
                       .Select(n => new NotificationDto
                       {
                         Codigo = n.Codigo,
                         Tipo = n.Tipo,
                         Mensagem = n.Mensagem,
                         NomeCampo = n.NomeCampo
                       })
                       .ToList();
      _notificador.LimparNotificacoes();

      bool hasErrors = allNotifications.Any(n => n.Tipo == "Erro");

      if (hasErrors)
      {
        return BadRequest(ApiResponse<T>.CreateError(
          "Ocorreram erros na sua requisição.",
          allNotifications
        ));
      }
      else
      {
        return Ok(ApiResponse<T>.CreateSuccess(
          data,
          "Operação realizada com sucesso.",
          allNotifications
        ));
      }
    }

    // SOBRECARGA 3: Constrói um ApiResponse<T> sem dados específicos
    protected ActionResult CustomResponse<T>()
    {
      var allNotifications = _notificador.ObterNotificacoes()
                       .Select(n => new NotificationDto
                       {
                         Codigo = n.Codigo,
                         Tipo = n.Tipo,
                         Mensagem = n.Mensagem,
                         NomeCampo = n.NomeCampo
                       })
                       .ToList();
      _notificador.LimparNotificacoes();

      bool hasErrors = allNotifications.Any(n => n.Tipo == "Erro");

      if (hasErrors)
      {
        return BadRequest(ApiResponse<T>.CreateError(
          "Ocorreram erros na sua requisição.",
          allNotifications
        ));
      }
      else
      {
        return Ok(ApiResponse<T>.CreateSuccess(
          default(T), // Data é o valor padrão para T
                "Operação realizada com sucesso.",
          allNotifications
        ));
      }
    }

    // SOBRECARGA 4: Constrói um ApiResponse simples (sem tipo genérico para Data),
    protected ActionResult CustomResponse()
    {
      var allNotifications = _notificador.ObterNotificacoes()
                       .Select(n => new NotificationDto
                       {
                         Codigo = n.Codigo,
                         Tipo = n.Tipo,
                         Mensagem = n.Mensagem,
                         NomeCampo = n.NomeCampo
                       })
                       .ToList();
      _notificador.LimparNotificacoes();

      bool hasErrors = allNotifications.Any(n => n.Tipo == "Erro");

      if (hasErrors)
      {
        return BadRequest(ApiResponse.CreateError(
          "Ocorreram erros na sua requisição.",
          allNotifications
        ));
      }
      else
      {
        return Ok(ApiResponse.CreateSuccess(
          "Operação realizada com sucesso.",
          allNotifications
        ));
      }
    }

    // Método para lidar com erros do ModelState e notificar
    protected ActionResult CustomValidationProblem(ModelStateDictionary modelState)
    {
      var errors = modelState.Values.SelectMany(v => v.Errors);
      foreach (var error in errors)
      {
        NotificarErro(error.ErrorMessage, null, error.Exception?.Message);
      }
      return CustomResponse(); // Chama CustomResponse() sem dados, que irá coletar as notificações de erro do ModelState
    }

    // Método auxiliar para obter o ID do usuário logado
    protected string ObterUsuarioIdLogado()
    {
      return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    // Método auxiliar para verificar se o usuário está autenticado
    protected bool UsuarioEstaAutenticado()
    {
      return User.Identity.IsAuthenticated;
    }
  }
}
