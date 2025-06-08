using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Equipes;
using ApostasApp.Core.Domain.Models.Estadios;

namespace ApostasApp.Core.Domain.Models.Ufs
{     
    public class Uf : Entity
    {
        public Uf()
        {
            Equipes = new List<Equipe>();
            Estadios = new List<Estadio>();            
        }

        public string Nome { get; set; }

        public string Sigla { get; set; }

        /* EF Relations */
        public ICollection<Equipe> Equipes { get; set; }

        public ICollection<Estadio> Estadios { get; set; }

    }
}