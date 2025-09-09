namespace ApostasApp.Core.Domain.Interfaces
{
    public interface IRankingResult
    {
        Guid ApostadorId { get; set; }
        int Pontuacao { get; set; }
        string NomeApostador { get; set; }
        string UsuarioId { get; }    
        
        //CORREÇÃO: ADICIONE AS DUAS PROPRIEDADES ABAIXO!
        string Apelido { get; }
        string FotoPerfil { get; }
        
    }
}
    