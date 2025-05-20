using DApostasApp.Core.Domain.Models.RankingRodadas;
using FluentValidation;


namespace ApostasApp.Core.Domain.Validations
{
    public class RankingRodadaValidation : AbstractValidator<RankingRodada>
    {
        public RankingRodadaValidation()
        {

            RuleFor(r => r.RodadaId)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");                      

            RuleFor(r => r.ApostadorCampeonatoId)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");


            RuleFor(r => r.Pontuacao)
            .GreaterThanOrEqualTo(0).WithMessage("A pontuação não pode ser negativa");

            RuleFor(r => r.Posicao)
                .GreaterThanOrEqualTo(0).WithMessage("A posição deve ser maior que zero");

            RuleFor(r => r.DataAtualizacao)
                .NotEmpty().WithMessage("A data de atualização precisa ser fornecida");
        }

    }
}