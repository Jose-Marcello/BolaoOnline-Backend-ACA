// Localização: ApostasApp.Core.Application.Models/ApiResponse.cs

using System.Collections.Generic;
using ApostasApp.Core.Domain.Models.Notificacoes; // Ajuste este using se NotificationDto estiver em outro lugar

namespace ApostasApp.Core.Application.Models
{
    /// <summary>
    /// Classe genérica para padronizar as respostas da API para o frontend.
    /// Inclui status de sucesso, dados e uma lista de notificações.
    /// </summary>
    /// <typeparam name="T">O tipo de dado que a resposta contém.</typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public List<NotificationDto> Notifications { get; set; }

        public ApiResponse()
        {
            Notifications = new List<NotificationDto>();
        }

        // Construtores para facilitar a criação de respostas
        public ApiResponse(bool success, T data, List<NotificationDto> notifications = null)
        {
            Success = success;
            Data = data;
            Notifications = notifications ?? new List<NotificationDto>();
        }
    }

    /// <summary>
    /// Versão não genérica para respostas que não retornam dados específicos.
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        public ApiResponse() : base() { }

        public ApiResponse(bool success, string message = null, List<NotificationDto> notifications = null)
            : base(success, null, notifications)
        {
            // Você pode adicionar uma notificação de mensagem principal aqui se desejar
            if (!string.IsNullOrEmpty(message))
            {
                Notifications.Add(new NotificationDto { Tipo = success ? "Sucesso" : "Erro", Mensagem = message });
            }
        }
    }
}
