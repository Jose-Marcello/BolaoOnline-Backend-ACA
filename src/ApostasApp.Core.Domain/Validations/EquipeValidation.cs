using ApostasApp.Core.Domain.Models.Equipes;
using FluentValidation;

namespace ApostasApp.Core.Domain.Validations
{
    public class EquipeValidation : AbstractValidator<Equipe>
    {
        public EquipeValidation()
        {
            RuleFor(c => c.Nome)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(2, 40)
                .WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(c => c.Sigla)
              .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
              .Length(3, 3)
              .WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");


        }


    }
}