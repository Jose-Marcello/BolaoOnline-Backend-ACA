using Microsoft.AspNetCore.Mvc;

namespace ApostasApp.Core.Web.Controllers
{  

  
    // A rota base é apenas "/"
    [Route("")]
    [ApiController]
    public class HealthController : ControllerBase
    {
      // Responde a GET /
      [HttpGet]
      public IActionResult Get()
      {
        // Retorna uma resposta simples e rápida para o Azure saber que o app está ativo.
        return Ok(new { Status = "Healthy", Service = "BolaoOnline API" });
      }

      // Responde a GET /health
      [HttpGet("health")]
      public IActionResult GetHealth()
      {
        // Opcional, mas útil para o Azure Health Check
        return Ok();
      }

    }
 
}
