// Localização: ApostasApp.Core.Domain.Models.Financeiro/TransacaoFinanceira.cs
using ApostasApp.Core.Domain.Models.Base;
using System;
using ApostasApp.Core.Domain.Models.Apostas; // Adicione este using para ApostaRodada

namespace ApostasApp.Core.Domain.Models.Financeiro
{
    public class TransacaoFinanceira : Entity
    {
        // Propriedades de Chave Estrangeira e Navegação para Saldo (já corretas)
        public Guid SaldoId { get; set; }
        public Saldo Saldo { get; private set; }

        // >>> NOVAS PROPRIEDADES PARA LIGAÇÃO COM APOSTARODADA <<<
        public Guid? ApostaRodadaId { get; private set; } // Chave Estrangeira (nullable)
        public ApostaRodada ApostaRodada { get; private set; } // Propriedade de Navegação

        public decimal Valor { get; private set; }
        public TipoTransacao Tipo { get; private set; }
        public DateTime DataTransacao { get; private set; }
        public string Descricao { get; private set; }

        // NOVO: Adicione estas duas propriedades
        public string ExternalReference { get; set; }
        public string Status { get; set; }

        // Construtor vazio para o Entity Framework Core
        protected TransacaoFinanceira() { }

        // Construtor principal - AGORA COM OPÇÃO PARA APOSTARODADAID
        public TransacaoFinanceira(
            Guid saldoId,
            TipoTransacao tipo,
            decimal valor,
            string descricao,
            Guid? apostaRodadaId = null) // Parâmetro opcional e nullable
            : base()
        {
            if (saldoId == Guid.Empty)
                throw new ArgumentException("SaldoId não pode ser um GUID vazio.", nameof(saldoId));
            if (valor == 0)
                throw new ArgumentException("Valor da transação não pode ser zero.", nameof(valor));
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("Descrição da transação não pode ser vazia.", nameof(descricao));

            SaldoId = saldoId;
            Tipo = tipo;
            Valor = valor;
            Descricao = descricao;
            DataTransacao = DateTime.Now;
            ApostaRodadaId = apostaRodadaId; // Atribui o ID opcional
        }

        // Você pode adicionar métodos para associar/desassociar a ApostaRodadaId se necessário
        public void AssociarApostaRodada(Guid apostaRodadaId)
        {
            if (apostaRodadaId == Guid.Empty)
                throw new ArgumentException("ApostaRodadaId não pode ser um GUID vazio.", nameof(apostaRodadaId));
            ApostaRodadaId = apostaRodadaId;
        }

        public void DesassociarApostaRodada()
        {
            ApostaRodadaId = null;
        }
    }
}
