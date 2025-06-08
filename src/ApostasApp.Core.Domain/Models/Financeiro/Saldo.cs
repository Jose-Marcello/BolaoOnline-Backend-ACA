// Em ApostasApp.Core.Domain.Models.Financeiro/Saldo.cs

using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Base; // Assumindo uma classe base para entidades (ex: EntityBase)
using System;

namespace ApostasApp.Core.Domain.Models.Financeiro
{
    public class Saldo : Entity // Ou apenas 'public class Saldo' se não tiver EntityBase
    {

        // Construtor privado para EF Core e construtor público para criação
        protected Saldo() { }

        public Saldo(Guid apostadorId, decimal valorInicial = 0)
        {
            Id = Guid.NewGuid();
            ApostadorId = apostadorId;
            Valor = valorInicial;
            DataUltimaAtualizacao = DateTime.Now;
            Transacoes = new List<TransacaoFinanceira>();
        }

        public Guid ApostadorId { get; set; } // Chave estrangeira para o usuário/apostador
        public decimal Valor { get; private set; } // O valor do saldo (private set para controle por métodos)
        public DateTime DataUltimaAtualizacao { get; private set; } // Data da última alteração

        // Coleção de transações associadas a este saldo
        public ICollection<TransacaoFinanceira> Transacoes { get; set; }

        // Método para adicionar valor ao saldo (depósito)
        public void Adicionar(decimal valor)
        {
            if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor), "O valor a ser adicionado deve ser maior que zero.");
            Valor += valor;
            DataUltimaAtualizacao = DateTime.Now;
        }

        // Método para debitar valor do saldo (saque, adesão)
        public bool Debitar(decimal valor)
        {
            if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor), "O valor a ser debitado deve ser maior que zero.");
            if (Valor < valor) return false; // Saldo insuficiente

            Valor -= valor;
            DataUltimaAtualizacao = DateTime.Now;
            return true;
        }

        public Apostador Apostador { get; set; } // Propriedade de navegação

    }
}