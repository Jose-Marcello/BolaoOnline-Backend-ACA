// ApostasApp.Core.Presentation.ViewModels/ApostasPorRodadaViewModel.cs
using System;
using System.Collections.Generic;

namespace ApostasApp.Core.Presentation.ViewModels
{
    public class ApostasPorRodadaViewModel
    {
        // IDs necessários para a chamada AJAX e outros links
        public Guid ApostadorCampeonatoId { get; set; }
        public Guid RodadaId { get; set; }

        // Dados para exibição no cabeçalho da View (substituindo TempData)
        public string ApostadorApelido { get; set; }
        public string CampeonatoNome { get; set; }
        public int NumeroRodada { get; set; }

        // Informações sobre a última aposta (substituindo TempData)
        public string DataAposta { get; set; } // Formato de string para exibição
        public string HoraAposta { get; set; } // Formato de string para exibição
        public string StatusEnvioAposta { get; set; } // "ENVIADA" ou "AINDA NÃO ENVIADA"

        // Propriedade para os dados da DataTable (lista de apostas)
        // Note: A DataTable fará uma chamada AJAX separada para obter esses dados.
        // Esta propriedade é mais para documentação do que para uso direto aqui,
        // mas pode ser útil se decidir pré-carregar os dados no Model.
        // Por enquanto, a DataTable fará sua própria chamada POST.
        // public IEnumerable<ApostaViewModel> Apostas { get; set; } // Comentar por enquanto, a DataTable busca via AJAX
    }

    
}