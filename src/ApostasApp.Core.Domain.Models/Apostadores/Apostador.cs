using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Usuarios;


namespace ApostasApp.Core.Domain.Models.Apostadores
{
    public class Apostador : Entity
    {
        public StatusApostador Status { get; set; }
        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public IEnumerable<ApostadorCampeonato> ApostadoresCampeonatos { get; set; }


    }
}