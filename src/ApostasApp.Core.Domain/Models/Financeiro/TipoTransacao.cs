namespace ApostasApp.Core.Domain.Models.Financeiro
{
    public enum TipoTransacao
    {
        // Pagamentos realizados pelo Apostador
        AdesaoCampeonato = 1,
        ApostaRodada = 2,
        TaxaAdministrativa = 3, // Se houver taxas extras

        // Recebimentos pelo Apostador (Prêmios)
        PremioRodada = 10,
        PremioCampeonatoFinal = 11,

        // Outras
        CreditoManual = 20, // Para ajustes manuais, ex: bônus
        DebitoManual = 21   // Para ajustes manuais, ex: estorno
    }
}