// Localização: Controllers/UploadController.cs
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _hostingEnvironment;

    public UploadController(IWebHostEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    [HttpPost("Image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { success = false, message = "Nenhum arquivo enviado." });
        }

        try
        {
            // Gera um nome único para o arquivo para evitar colisões
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // Define o caminho completo para o diretório de destino (ex: wwwroot/assets/images)
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "assets", "images");
            var filePath = Path.Combine(uploads, uniqueFileName);

            // Garante que o diretório exista
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }

            // Salva o arquivo no servidor
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Constrói a URL para a imagem que o frontend vai usar
            var imageUrl = $"/assets/images/{uniqueFileName}";

            // Retorna o sucesso e a URL da imagem salva
            return Ok(new { success = true, url = imageUrl, message = "Upload realizado com sucesso." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Erro interno do servidor: {ex.Message}" });
        }
    }
}