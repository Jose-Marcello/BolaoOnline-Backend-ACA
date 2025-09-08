using ApostasApp.Core.Domain.Models.Interfaces.Rodadas;
using ApostasApp.Core.Domain.Models.Usuarios;
using ApostasApp.Core.Domain.Validations.Documentos;
using FluentValidation;

namespace ApostasApp.Core.Application.Validations
{
    public class UsuarioValidation : AbstractValidator<Usuario>
    {
        // ADICIONAR ISSO AQUI:
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioValidation(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;

            // Regra para verificar se o e-mail já existe
            //RuleFor(u => u.Email)
            //    .NotEmpty().WithMessage("O campo E-mail precisa ser fornecido.")
            //    .EmailAddress().WithMessage("O e-mail fornecido é inválido.");
                //.MustAsync(async (email, cancellation) =>
                //{
                //    return !await _usuarioRepository.EmailExiste(email);
                //}).WithMessage("Este e-mail já foi cadastrado.");

            // Regra para verificar se o apelido já existe
            //RuleFor(u => u.Apelido)
            //    .NotEmpty().WithMessage("O apelido é obrigatório.")
             //   .MustAsync(async (apelido, cancellation) =>
            //    {
           //         return !await _usuarioRepository.ApelidoExiste(apelido);
            //    }).WithMessage("Este apelido já está em uso.");

            // Regra para verificar se o CPF já existe
            //RuleFor(u => u.CPF)
            //    .NotEmpty().WithMessage("O campo CPF precisa ser fornecido.")
            //    .MustAsync(async (cpf, cancellation) =>
            //    {
            //        return !await _usuarioRepository.CpfExiste(cpf);
           //    }).WithMessage("Este CPF já foi cadastrado.");

            // Regra para validar o tamanho e a validade do CPF
            RuleFor(u => u.CPF.Length).Equal(CpfValidacao.TamanhoCpf)
                .WithMessage("O campo CPF precisa ter {ComparisonValue} caracteres e foi fornecido {PropertyValue}.");

            RuleFor(u => CpfValidacao.Validar(u.CPF)).Equal(true)
                .WithMessage("O CPF fornecido é inválido.");

            // Regra para validar o celular
            RuleFor(u => u.Celular)
                .NotEmpty().WithMessage("O campo Celular precisa ser fornecido.")
                .Length(11, 11)
                .WithMessage("O campo Celular precisa ter {MinLength} caracteres.");


            // Nova regra para a validação dos termos de uso
            RuleFor(u => u.TermsAccepted)
            .Equal(true).WithMessage("Você precisa aceitar os termos de uso para se registrar.");


        }




    }
}