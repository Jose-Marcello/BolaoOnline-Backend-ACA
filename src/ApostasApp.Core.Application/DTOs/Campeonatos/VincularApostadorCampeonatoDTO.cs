// VincularApostadorCampeonatoDTO.cs (Backend - assumindo que você o tem)
// Mude Guid CampeonatoId para string CampeonatoId e Guid ApostadorId para string ApostadorId
public class VincularApostadorCampeonatoDto
{
    public string ApostadorId { get; set; } // << Alterar de Guid para string
    public string CampeonatoId { get; set; } // << Alterar de Guid para string
}