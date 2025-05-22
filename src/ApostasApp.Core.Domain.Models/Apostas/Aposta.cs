using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Jogos;


namespace ApostasApp.Core.Domain.Models.Apostas{ 
    
    public class Aposta : Entity
    {
        public Guid JogoId { get; set; }
        public Guid ApostadorCampeonatoId { get; set; }

        public DateTime? DataHoraAposta { get; set; }
        public int PlacarApostaCasa { get; set; }
        public int PlacarApostaVisita { get; set; }

        public bool Enviada { get; set; }

        public int Pontos { get; set; }

        /* EF Relations */
        public Jogo Jogo { get; set; }
        public ApostadorCampeonato ApostadorCampeonato { get; set; }

        //public IEnumerable<Jogo> Jogos { get; set; }


    }
}