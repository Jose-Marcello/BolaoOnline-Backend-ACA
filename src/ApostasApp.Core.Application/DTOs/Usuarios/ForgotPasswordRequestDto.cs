using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; // Para System.Text.Json
using Newtonsoft.Json; // IMPORTANTE: Adicione este using
using System.Collections.Generic; // Para usar listas em DTOs, se necessário

namespace ApostasApp.Core.Application.DTOs.Usuarios
{
  public class ForgotPasswordRequestDto
  {
    //[Required(ErrorMessage = "O Email é obrigatório.")]
    //[EmailAddress(ErrorMessage = "O Email está em formato inválido.")]

    // 🎯 SOLUÇÃO DEFINITIVA: 
    // 1. Usa PascalCase (Padrão C#)
    // 2. Adiciona as anotações de ambos os serializers para forçar o nome JSON 'email'
    // Isso elimina a dependência da política de nomes global do Program.cs, que está falhando no ACA.
    [JsonPropertyName("email")] // Anotação System.Text.Json
    [JsonProperty("email")] // Anotação Newtonsoft.Json (Para máxima compatibilidade)
    public string Email { get; set; } // Propriedade em PascalCase (correto)
  }
}
