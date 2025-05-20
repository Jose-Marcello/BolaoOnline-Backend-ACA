using System.ComponentModel.DataAnnotations;
using ApostasApp.Core.Domain.Models.Apostadores;
using Microsoft.AspNetCore.Identity;

namespace ApostasApp.Core.Domain.Models.Usuarios
{
    public class Usuario : IdentityUser
    {

        [Required]
        [MaxLength(11)]
        public string CPF { get; set; }

        [MaxLength(20)]
        public string Celular { get; set; }

        [MaxLength(50)]
        public string Apelido { get; set; }

        public Apostador Apostador { get; set; }

        public DateTime? RegistrationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }

}