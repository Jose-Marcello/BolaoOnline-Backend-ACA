// ApostasApp.Core.Application.Models/ApiResponse.cs
// Esta classe define o formato das respostas da sua API, incluindo métodos estáticos para criação.

// Usar NotificationDto para a comunicação da API
using ApostasApp.Core.Domain.Models.Notificacoes; // Para Notificacao, se necessário mapear de/para

using System.Collections.Generic;
using System.Linq;

namespace ApostasApp.Core.Application.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<NotificationDto> Notifications { get; set; } // Propriedade para DTOs de notificação

        public ApiResponse()
        {
            Notifications = new List<NotificationDto>();
        }

        public ApiResponse(bool success, List<NotificationDto> notifications = null)
        {
            Success = success;
            Message = success ? "Operação realizada com sucesso." : "Ocorreu um erro.";
            Notifications = notifications ?? new List<NotificationDto>();
        }

        public ApiResponse(bool success, string message, List<NotificationDto> notifications = null)
        {
            Success = success;
            Message = message;
            Notifications = notifications ?? new List<NotificationDto>();
        }

        // <<-- MÉTODOS ESTÁTICOS CreateSuccess e CreateError (NÃO GENÉRICOS) -->>
        public static ApiResponse CreateSuccess(string message = "Operação realizada com sucesso.", List<NotificationDto> notifications = null)
        {
            return new ApiResponse(true, message, notifications);
        }

        public static ApiResponse CreateError(string message = "Ocorreu um erro na operação.", List<NotificationDto> notifications = null)
        {
            return new ApiResponse(false, message, notifications);
        }

        // <<-- MÉTODOS ESTÁTICOS CreateSuccess e CreateError (GENÉRICOS) -->>
        public static ApiResponse<T> CreateSuccess<T>(T data, string message = "Operação realizada com sucesso.", List<NotificationDto> notifications = null)
        {
            return new ApiResponse<T>(true, message, data, notifications);
        }

        public static ApiResponse<T> CreateError<T>(string message = "Ocorreu um erro na operação.", List<NotificationDto> notifications = null)
        {
            return new ApiResponse<T>(false, message, default(T), notifications);
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }

        public ApiResponse() : base() { }

        public ApiResponse(bool success, string message, T data, List<NotificationDto> notifications = null)
            : base(success, message, notifications)
        {
            Data = data;
        }

        public ApiResponse(bool success, T data, List<NotificationDto> notifications = null)
            : base(success, notifications)
        {
            Data = data;
        }
    }
}
