// Em ApostasApp.Core.InfraStructure.Data.Repository.Financeiro/SaldoRepository.cs
using ApostasApp.Core.Domain.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.InfraStructure.Data.Context; // Assumindo o caminho do seu DbContext
using ApostasApp.Core.InfraStructure.Data.Repository; // Assumindo o caminho do seu GenericRepository

using Microsoft.EntityFrameworkCore; // Para usar .Include() se necessário
using System;
using System.Threading.Tasks;

namespace ApostasApp.Core.InfraStructure.Data.Repository.Financeiro // Ajuste o namespace
{
    public class SaldoRepository : Repository<Saldo>, ISaldoRepository
    {
        private readonly MeuDbContext _context; // Referência ao seu DbContext

        public SaldoRepository(MeuDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Saldo> ObterSaldoPorApostadorId(Guid apostadorId)
        {
            // Usar FirstOrDefaultAsync para retornar null se não encontrar
            // Usar .Include(s => s.Transacoes) se você quiser carregar as transações junto com o saldo
            return await _context.Saldos
                                 .Include(s => s.Transacoes) // Inclui as transações relacionadas
                                 .FirstOrDefaultAsync(s => s.ApostadorId == apostadorId);
        }
        // Se precisar de outros métodos específicos para Saldo, adicione aqui
    }
}