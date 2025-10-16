// HealthController.cs (Ajustado)
using Microsoft.AspNetCore.Mvc;

namespace ApostasApp.Core.Web.Controllers
{
  // Rota específica para o App Service - NÃO USE [Route("")]
  [Route("app-status")]
  [ApiController]
  public class HealthController : ControllerBase
  {
    // Responde a GET /app-status
    [HttpGet]
    public IActionResult Get()
    {
      return Ok(new { Status = "Healthy", Service = "BolaoOnline API" });
    }

    // Remova o GetHealth para simplificar
  }
}
