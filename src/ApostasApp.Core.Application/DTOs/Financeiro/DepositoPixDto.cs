namespace ApostasApp.Core.Application.DTOs.Financeiro // Mantenha seu namespace existente
{
    public class DepositoPixDto
    {
        public Guid ApostadorId { get; set; }
        public decimal Valor { get; set; }
    }
}