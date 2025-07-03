// Exemplo: ApostasApp.Core.Domain.Models.Notificacoes\Notificacao.cs
namespace ApostasApp.Core.Domain.Models.Notificacoes
{
    public class Notificacao
    {
        public string Codigo { get; private set; } // <<-- ADICIONADO: Propriedade Codigo
        public string Tipo { get; private set; }
        public string Mensagem { get; private set; }
        public string NomeCampo { get; private set; }

        // Construtor padrão (necessário para alguns ORMs ou deserialização)
        protected Notificacao() { }

        // Construtor para criar notificações
        public Notificacao(string tipo, string mensagem, string codigo = null, string nomeCampo = null)
        {
            Codigo = codigo; // Atribui o código
            Tipo = tipo;
            Mensagem = mensagem;
            NomeCampo = nomeCampo;
        }
    }
}
