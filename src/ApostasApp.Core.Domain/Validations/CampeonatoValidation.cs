using ApostasApp.Core.Domain.Models.Campeonatos;
using FluentValidation;

namespace ApostasApp.Core.Domain.Validations
{
    public class CampeonatoValidation : AbstractValidator<Campeonato>
    {
        public CampeonatoValidation()
        {
            RuleFor(c => c.Nome)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(2, 100)
                .WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(c => c.DataInic)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

            RuleFor(c => c.DataFim)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");
            //.Must(DataFimMaiorQueDataInic).WithMessage("A data FINAL do Campeonato tem que ser maior que a data INICIAL");


            RuleFor(c => c).Custom((obj, context) =>
            {
                if (obj.DataFim <= obj.DataInic)
                    context.AddFailure("A data Final não pode ser menor (ou igual) a data inicial de um Campeonato.");
            });

            RuleFor(c => c.NumRodadas)
               .GreaterThan(0).WithMessage("O campo {PropertyName} precisa ser maior que {ComparisonValue}");


            //RuleFor(c => c).Must(c => !c.Tipo.GetType().IsEnum)
            //    .WithMessage("O campo Tipo está inválido - Use 1 para Pontos Corridos ou 2 para Copas");


        }


    }
}