namespace ApostasApp.Core.Domain.Models.Results
{     
        public class OperationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }

            public static OperationResult CreateSuccess(string message = null)
            {
                return new OperationResult { Success = true, Message = message };
            }

            public static OperationResult CreateFailure(string message)
            {
                return new OperationResult { Success = false, Message = message };
            }
        }    
}