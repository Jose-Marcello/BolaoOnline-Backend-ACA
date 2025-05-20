using ApostasApp.Core.Domain.Models.Jogos;
using FluentValidation;
using System.Runtime.ConstrainedExecution;

namespace ApostasApp.Core.Domain.Validations
{
    public class JogoValidation : AbstractValidator<Jogo>
    {
        public JogoValidation()
        {

            RuleFor(j => j.RodadaId)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");            
            
            RuleFor(c => c.DataJogo)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

            RuleFor(c => c.HoraJogo)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

            RuleFor(j => j.EquipeCasaId)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

            RuleFor(j => j.EquipeVisitanteId)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");           

            /*RuleFor(j => j).Custom((obj, context) =>
            {
                if (obj.EquipeCasaId == obj.EquipeVisitanteId)
                    context.AddFailure("A equipe da CASA não pode ser a equipe VISITANTE!! ");
            });*/

        }

    }
}