
namespace ApostasApp.Core.Domain.Interfaces.Relatorios
{
    public interface IConferenciaPalpite
    {
        string ApelidoApostador { get; set; }
        string IdentificadorAposta { get; set; }
        DateTime DataHoraEnvio { get; set; }
        string NomeEquipeCasa { get; set; }
        int PlacarPalpiteCasa { get; set; }
        int PlacarPalpiteVisita { get; set; }
        string NomeEquipeVisita { get; set; }
    }
}