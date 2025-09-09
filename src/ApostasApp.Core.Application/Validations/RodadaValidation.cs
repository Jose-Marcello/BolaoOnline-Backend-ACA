using ApostasApp.Core.Domain.Models.Rodadas;
using FluentValidation;

namespace ApostasApp.Core.Application.Validations
{
    public class RodadaValidation : AbstractValidator<Rodada>
    {
        public RodadaValidation()
        {

            RuleFor(c => c.CampeonatoId)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

            //RuleFor(c => c).Must(c => c.Campeonato.Ativo)
            //   .WithMessage("O campeonato tem que ser ATIVO para alterar RODADAS");            

            RuleFor(c => c.NumeroRodada)
               .GreaterThan(0).WithMessage("O campo {PropertyName} precisa ser maior que {ComparisonValue}");

            RuleFor(c => c.DataInic)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

            RuleFor(c => c.DataFim)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");
            //.Must(DataFimMaiorQueDataInic).WithMessage("A data FINAL do Campeonato tem que ser maior que a data INICIAL");


            RuleFor(c => c).Custom((obj, context) =>
            {
                if (obj.DataFim <= obj.DataInic)
                    context.AddFailure("A data Final não pode ser menor (ou igual) a data inicial de uma Rodada.");
            });

            RuleFor(c => c.NumJogos)
               .GreaterThan(0).WithMessage("O campo {PropertyName} precisa ser maior que {ComparisonValue}");

           
            //RuleFor(c => c).Must(c => !c.Tipo.GetType().IsEnum)
            //    .WithMessage("O campo Tipo está inválido - Use 1 para Pontos Corridos ou 2 para Copas");

            
        }


    }
}