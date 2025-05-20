using ApostasApp.Core.Domain.Models.Campeonatos;
using FluentValidation;

namespace ApostasApp.Core.Domain.Validations
{
    public class EquipeCampeonatoValidation : AbstractValidator<EquipeCampeonato>
    {
        public EquipeCampeonatoValidation()
        {

            //RuleFor(ec => ec.Id)
            //   .NotEmpty().WithMessage("O campo Id precisa ser fornecido");

            RuleFor(ec => ec.CampeonatoId)
                .NotNull().WithMessage("O campo Campeonato precisa ser fornecido");
            //.Length(2, 100)

            RuleFor(ec => ec.EquipeId)
                 .NotNull().WithMessage("O campo Equipe precisa ser fornecido");
            //.Length(2, 100)
        }


    }
}