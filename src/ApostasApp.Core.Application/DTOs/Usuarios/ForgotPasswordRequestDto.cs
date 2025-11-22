using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; // NOVO: Adicionado para forçar o nome da propriedade JSON

namespace ApostasApp.Core.Application.DTOs.Usuarios
{
  public class ForgotPasswordRequestDto
  {
    [Required(ErrorMessage = "O Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "O Email está em formato inválido.")] 
    // CORREÇÃO CRÍTICA: Força o nome da propriedade JSON para 'email' (camelCase)
    // Isso resolve a falha de desserialização no ambiente de produção/ACA.
    //[JsonPropertyName("email")]
    public string Email { get; set; }
  }
}
