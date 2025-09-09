using ApostasApp.Core.Domain.Models.Apostas;
using FluentValidation;


namespace ApostasApp.Core.Application.Validations
{
    public class PalpiteValidation : AbstractValidator<Palpite>
    {
        public PalpiteValidation()


        {

            RuleFor(a => a.JogoId)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

           /* RuleFor(a => a.ApostadorCampeonatoId)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");*/

            //RuleFor(a => a.DataHoraAposta)
            //    .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");                      

        }

    }
}