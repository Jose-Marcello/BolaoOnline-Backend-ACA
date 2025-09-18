// Localização: ApostasApp.Core.Infrastructure.Data.Models/RankingDataModel.cs
// ou em um novo DTO no seu projeto de Application.

using ApostasApp.Core.Domain.Interfaces;

namespace ApostasApp.Core.Infrastructure.Data.Models
{
    public class RankingDataModel : IRankingResult
    {
        public Guid ApostadorId { get; set; }
        public string UsuarioId { get; set; } 
        public int Pontuacao { get; set; }
        public string NomeApostador { get; set; }
        public string Apelido { get; set; }
        public string FotoPerfil { get; set; }

    }
}
