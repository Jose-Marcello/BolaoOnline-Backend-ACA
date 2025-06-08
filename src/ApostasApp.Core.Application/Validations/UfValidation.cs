using ApostasApp.Core.Domain.Models.Ufs;
using FluentValidation;

namespace ApostasApp.Core.Application.Validations
{
    public class UfValidation : AbstractValidator<Uf>
    {
        public UfValidation()
        {
            RuleFor(c => c.Nome)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(2, 40)
                .WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(c => c.Sigla)
               .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
               .Length(2, 2)
               .WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");
        }


    }
}