// MercadoPagoSettings.cs
namespace ApostasApp.Core.Domain.Models.Configuracoes
{
    public class MercadoPagoSettings
    {        
            public string AccessToken { get; set; }
            // NOVO: Adicione esta linha
            public string WebhookSecret { get; set; }        

    }
}