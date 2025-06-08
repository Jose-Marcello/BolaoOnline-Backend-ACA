using ApostasApp.Core.Domain.Models.Usuarios;
using ApostasApp.Core.Domain.Validations.Documentos;
using FluentValidation;

namespace ApostasApp.Core.Application.Validations
{
    public class UsuarioValidation : AbstractValidator<Usuario>
    {

        public UsuarioValidation()
        {

            RuleFor(u => u.CPF.Length).Equal(CpfValidacao.TamanhoCpf)
                .WithMessage("O campo CPF precisa ter {ComparisonValue} caracteres e foi fornecido {PropertyValue}.");

            RuleFor(u => CpfValidacao.Validar(u.CPF)).Equal(true)
                    .WithMessage("O CPF fornecido é inválido.");

            RuleFor(u => u.Celular)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(11, 11)
                .WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            /*RuleFor(u => u.Apelido)
             .NotEmpty().WithMessage("O apelido é obrigatório.")
             .MustAsync(async (apelido, cancellation) =>
             {
                 return !await _usuarioRepository.ApelidoExiste(apelido);
             }).WithMessage("Este apelido já está em uso.");*/

            //RuleFor(a => a.DataCadastro)
            //.NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

        }


    }
}