// Exemplo de DTO para a requisição de atualização (UpdatePerfilRequestDto.cs)

namespace ApostasApp.Core.Application.DTOs.Apostadores
{ 

  public class UpdatePerfilRequestDto
  {
    public string Apelido { get; set; }
    public string Celular { get; set; }
    public string FotoPerfil { get; set; } // O campo agora é a URL, não o arquivo
  }

}