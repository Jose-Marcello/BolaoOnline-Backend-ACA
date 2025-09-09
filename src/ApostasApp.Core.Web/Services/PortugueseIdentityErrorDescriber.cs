using Microsoft.AspNetCore.Identity;

namespace ApostasApp.Web.Services
{
    public class PortugueseIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"As senhas devem ter pelo menos {length} caracteres."
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "As senhas devem conter pelo menos um dígito ('0'-'9')."
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "As senhas devem conter pelo menos uma letra minúscula ('a'-'z')."
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "As senhas devem conter pelo menos uma letra maiúscula ('A'-'Z')."
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "As senhas devem conter pelo menos um caractere não alfanumérico."
            };
        }

       
        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUniqueChars),
                Description = $"As senhas devem conter pelo menos {uniqueChars} caracteres exclusivos."
            };
        }

        public override IdentityError PasswordMismatch()
        {
            return new IdentityError
            {
                Code = "PasswordMismatch",
                Description = "A senha e a confirmação da senha não correspondem."
            };
        }

        // Você pode sobrescrever outras mensagens conforme necessário
    }
}