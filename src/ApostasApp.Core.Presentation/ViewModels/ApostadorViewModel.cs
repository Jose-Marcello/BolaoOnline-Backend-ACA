using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Usuarios;
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Presentation.ViewModels
{
    public class ApostadorViewModel
    {
        [Key]
        public Guid Id { get; set; }
        public StatusApostador Status { get; set; }
        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public IEnumerable<ApostadorCampeonato> ApostadoresCampeonatos { get; set; }



    }
}