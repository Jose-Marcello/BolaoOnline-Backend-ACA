using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Ufs;

namespace ApostasApp.Core.Domain.Models.Estadios

{     
    public class Estadio : Entity
    {       
        public string Nome { get; set; }
        
        public Guid UfId { get; set; }

        /* EF Relations */
        public Uf Uf { get; set; }           

    }
}