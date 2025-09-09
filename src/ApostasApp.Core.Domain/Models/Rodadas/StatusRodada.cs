namespace ApostasApp.Core.Domain.Models.Rodadas

{
    public enum StatusRodada
    {
        EmConstrucao = 0,
        ProntaNaoIniciada = 1,//Prontas. mas os jogos não começaram 
        EmApostas = 2, //Só pode haver 1 rodada EmApostas
        Corrente = 3, //Só pode haver 1 Rodada Corrente -
        Finalizada = 4
    
       
    
    }
}