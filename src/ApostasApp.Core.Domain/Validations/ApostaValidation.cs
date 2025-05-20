using ApostasApp.Core.Domain.Models.Apostas;
using FluentValidation;


namespace ApostasApp.Core.Domain.Validations
{
    public class ApostaValidation : AbstractValidator<Aposta>
    {
        public ApostaValidation()
        {

            RuleFor(a => a.JogoId)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

            RuleFor(a => a.ApostadorCampeonatoId)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

            //RuleFor(a => a.DataHoraAposta)
            //    .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");                      

        }

    }
}