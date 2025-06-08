using ApostasApp.Core.Domain.Models.Financeiro; // Para a entidade Saldo
using ApostasApp.Core.Domain.Interfaces;


namespace ApostasApp.Core.Domain.Interfaces.Financeiro // Ajuste o namespace conforme sua estrutura
{
    // ISaldoRepository pode herdar de um IRepository genérico se você tiver um
    // Ou pode ser uma interface específica apenas com os métodos necessários
    public interface ISaldoRepository : IRepository<Saldo> // Assumindo que IRepository é seu genérico
    {
        // Método específico para obter o saldo de um usuário
        //Task<Saldo> ObterSaldoPorUsuarioId(Guid usuarioId);
        Task<Saldo> ObterSaldoPorApostadorId(Guid apostadorId);
        // Outros métodos específicos para Saldo, se houver, como:
        // Task AtualizarSaldo(Saldo saldo); // Ou usar o método Update do IRepository genérico
    }
}