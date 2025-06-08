using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Campeonatos; // Adicionar este using
using ApostasApp.Core.Domain.Models.Rodadas;     // Adicionar este using

namespace ApostasApp.Core.Domain.Models.Financeiro
{
    public class TransacaoFinanceira : Entity
    {
        protected TransacaoFinanceira() { }

        // Construtor privado para EF Core       
        public TransacaoFinanceira(Guid saldoId, TipoTransacao tipo, decimal valor, string descricao)
        {
            Id = Guid.NewGuid();
            SaldoId = saldoId;
            Tipo = tipo;
            Valor = valor;
            DataTransacao = DateTime.Now;
            Descricao = descricao;
        }

        //public Guid ApostadorId { get; set; }
        public decimal Valor { get; set; }
        public TipoTransacao Tipo { get; set; }
        public DateTime DataTransacao { get; set; }
        public string? Descricao { get; set; }

        // NOVAS PROPRIEDADES OPCIONAIS
        public Guid? CampeonatoId { get; set; } // Pode ser nulo
        public Guid? RodadaId { get; set; }     // Pode ser nulo
        public Guid SaldoId { get; set; } // Chave estrangeira para o Saldo relacionado

        /* EF Relations */
       
        public Campeonato? Campeonato { get; set; } // Pode ser nulo
        public Rodada? Rodada { get; set; }         // Pode ser nulo

        // Propriedade de navegação para o Saldo ao qual esta transação pertence
        public Saldo Saldo { get; set; }
        
    }
}


