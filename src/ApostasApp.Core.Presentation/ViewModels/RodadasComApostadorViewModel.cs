// Em ApostasApp.Core.Presentation.ViewModels/RodadasComApostadorViewModel.cs
using System.Collections.Generic; // Certifique-se de ter este using
using System; // Para Guid

namespace ApostasApp.Core.Presentation.ViewModels
{
    public class RodadasComApostadorViewModel
    {
        public IEnumerable<RodadaViewModel> Rodadas { get; set; }
        public Guid ApostadorCampeonatoId { get; set; }
        public string CampeonatoNome { get; set; } // Para exibir no título da View
    }
}