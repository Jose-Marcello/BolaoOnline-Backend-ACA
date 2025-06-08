// ApostasApp.Core.Domain.Validations.Apostas/ApostaRodadaValidation.cs
using ApostasApp.Core.Domain.Models.Apostas;
// Se seu Notificador/BaseService depende de Notificacao
using FluentValidation;
using System;

namespace ApostasApp.Core.Application.Validations
{
    // A validação para ApostaRodada. Pode herdar de Notificador se precisar.
    public class ApostaRodadaValidation : AbstractValidator<ApostaRodada> // Ou FluentValidation.AbstractValidator
    {
        public ApostaRodadaValidation()
        {
            RuleFor(a => a.Id)
                .NotEmpty().WithMessage("O Id da ApostaRodada não pode ser vazio.");

            RuleFor(a => a.ApostadorCampeonatoId)
                .NotEmpty().WithMessage("O Id do ApostadorCampeonato é obrigatório para a ApostaRodada.");

            RuleFor(a => a.RodadaId)
                .NotEmpty().WithMessage("O Id da Rodada é obrigatório para a ApostaRodada.");

            RuleFor(a => a.DataHoraSubmissao)
                .NotEmpty().WithMessage("A Data de Criação da ApostaRodada é obrigatória.");

            // Exemplo de uma regra de negócio mais complexa (opcional por enquanto, mas bom ter em mente)
            // RuleFor(a => a.PontuacaoTotal)
            //    .GreaterThanOrEqualTo(0).WithMessage("A Pontuação Total não pode ser negativa.");

            // Adicione outras regras de validação conforme necessário para sua entidade ApostaRodada.
            // Por exemplo:
            // - Verifique se a ApostaRodada já existe para aquele ApostadorCampeonato e Rodada.
            // (Esta validação geralmente é feita no serviço, consultando o repositório, não na validação da entidade)
        }
    }
}