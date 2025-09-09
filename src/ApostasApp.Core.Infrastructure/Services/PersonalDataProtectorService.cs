using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace ApostasApp.Core.Infrastructure.Services
{
    public class PersonalDataProtectorService : IPersonalDataProtector
    {
        private readonly IDataProtector _protector;

        public PersonalDataProtectorService(IDataProtectionProvider dataProtectionProvider)
        {
            _protector = dataProtectionProvider.CreateProtector("PersonalData");
        }

        public string Protect(string data)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var protectedBytes = _protector.Protect(dataBytes);
            return Encoding.UTF8.GetString(protectedBytes);
        }

        public string Unprotect(string data)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var unprotectedBytes = _protector.Unprotect(dataBytes);
            return Encoding.UTF8.GetString(unprotectedBytes);
        }
    }
}