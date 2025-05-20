
using ApostasApp.Core.Domain.Models.Estadios;
using FluentValidation;

namespace ApostasApp.Core.Domain.Validations
{
    public class EstadioValidation : AbstractValidator<Estadio>
    {
        public EstadioValidation()
        {
            RuleFor(c => c.Nome)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(2, 60)
                .WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

        }


    }
}