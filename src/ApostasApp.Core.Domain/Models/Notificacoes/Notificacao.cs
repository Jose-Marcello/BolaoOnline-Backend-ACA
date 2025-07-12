// C:\Desen\ApostasApp.Core\src\ApostasApp.Core.Domain.Models.Notificacoes\Notificacao.cs

// Assumindo que sua entidade Notificacao herda de uma classe base como Entity
// Se não herdar, remova ": Entity"
using ApostasApp.Core.Domain.Models.Base; // Se Entity estiver aqui

namespace ApostasApp.Core.Domain.Models.Notificacoes
{
    // <<-- VERIFIQUE SE O NOME DA SUA CLASSE É 'Notificacao' ou 'Notification' -->>
    // Se for 'Notification', ajuste o nome da classe aqui e em todos os outros arquivos.
    public class Notificacao : Entity // Ou apenas 'public class Notificacao' se não herdar
    {
        public string Codigo { get; private set; } // Ajuste o 'set' para 'public' se desejar atribuição externa direta
        public string Tipo { get; private set; }
        public string Mensagem { get; private set; }
        public string NomeCampo { get; private set; } // Ajuste o 'set' para 'public' se desejar atribuição externa direta

        // Construtor padrão (pode ser necessário para ORMs ou deserialização)
        public Notificacao() { }

        // Construtor que você já tinha (provavelmente)
        public Notificacao(string tipo, string mensagem)
        {
            Tipo = tipo;
            Mensagem = mensagem;
        }

        // <<-- NOVO CONSTRUTOR: Para definir Codigo e NomeCampo na criação -->>
        public Notificacao(string codigo, string tipo, string mensagem, string nomeCampo = null)
        {
            Codigo = codigo;
            Tipo = tipo;
            Mensagem = mensagem;
            NomeCampo = nomeCampo;
        }
    }
}
