// Localização: ApostasApp.Core.Domain.Models.Notificacoes/NotificationDto.cs

// Este DTO é para comunicação com o frontend.
// Ele deve espelhar a interface NotificationDto no seu frontend Angular.
namespace ApostasApp.Core.Domain.Models.Notificacoes
{
    public class NotificationDto
    {
        public string Codigo { get; set; } // <<-- ADICIONADO: Propriedade Codigo
        public string Tipo { get; set; }
        public string Mensagem { get; set; }
        public string NomeCampo { get; set; } // Opcional, se você usar para validação de campos específicos

        // Construtor padrão (necessário para deserialização JSON)
        public NotificationDto() { }

        // Construtor para facilitar a criação (opcional, mas bom para testes)
        public NotificationDto(string codigo, string tipo, string mensagem, string nomeCampo = null) // <<-- AJUSTADO: Inclui 'codigo'
        {
            Codigo = codigo; // Atribui o código
            Tipo = tipo;
            Mensagem = mensagem;
            NomeCampo = nomeCampo;
        }
    }
}
