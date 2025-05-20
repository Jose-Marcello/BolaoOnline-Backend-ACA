using ApostasApp.Core.Domain.Models.Apostadores;
using FluentValidation;

namespace ApostasApp.Core.Domain.Validations
{
    public class ApostadorValidation : AbstractValidator<Apostador>
    {
        
        public ApostadorValidation()
        { 
             RuleFor(a => a.UsuarioId).NotEmpty()
                    .WithMessage("O Id do Usuário é obrigatório.");

            RuleFor(a => a.Status).IsInEnum().WithMessage("O Status do Apostador é inválido.");

        }

    }
}