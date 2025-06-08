using ApostasApp.Core.Domain.Models.Campeonatos;
using FluentValidation;

namespace ApostasApp.Core.Application.Validations
{
    public class ApostadorCampeonatoValidation : AbstractValidator<ApostadorCampeonato>
    {
        public ApostadorCampeonatoValidation()
        {

            //RuleFor(ec => ec.Id)
            //   .NotEmpty().WithMessage("O campo Id precisa ser fornecido");

            RuleFor(ec => ec.CampeonatoId)
                .NotNull().WithMessage("O campo Campeonato precisa ser fornecido");
            //.Length(2, 100)

            RuleFor(ec => ec.ApostadorId)
                 .NotNull().WithMessage("O campo Apostador precisa ser fornecido");
            //.Length(2, 100)
        }


    }
}