namespace ApostasApp.Core.Domain.Models.Notificacoes
{
    public class Notificacao
    {
        public string Tipo { get; }
        public string Mensagem { get; }
        public string NomeCampo { get; }

        public Notificacao(string tipo, string mensagem, string nomeCampo = null)
        {
            Tipo = tipo;
            Mensagem = mensagem;
            NomeCampo = nomeCampo;
        }
    }
}
