using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Equipes;
using ApostasApp.Core.Domain.Models.Estadios;

namespace ApostasApp.Core.Domain.Models.Ufs
{     
    public class Uf : Entity
    {        
        public string Nome { get; set; }

        public string Sigla { get; set; }

        /* EF Relations */
        public IEnumerable<Equipe> Equipes { get; set; }

        public IEnumerable<Estadio> Estadios { get; set; }



    }
}