// Localização: ApostasApp.Core.Application.DTOs/Campeonatos/CampeonatoDto.cs
// Este DTO representa um Campeonato para ser enviado ao frontend.
// Ele é o DTO de SAÍDA padrão para a entidade Campeonato.

using System;
// Importe o namespace do seu enum TipoCampeonato, se ele for usado diretamente no DTO
// using ApostasApp.Core.Domain.Enums; 

namespace ApostasApp.Core.Application.DTOs.Campeonatos
{
    /// <summary>
    /// DTO de saída para representar um Campeonato, com informações básicas
    /// e sem referências circulares ou coleções de navegação.
    /// </summary>
    public class CampeonatoDto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int NumRodadas { get; set; }
        // Se TipoCampeonato é um enum, você pode convertê-lo para string aqui
        public string Tipo { get; set; }
        public bool Ativo { get; set; }
        public decimal CustoAdesao { get; set; }
        public bool AderidoPeloUsuario { get; set; } // Propriedade para indicar se o usuário atual aderiu

        // IMPORTANTE: NÃO INCLUA PROPRIEDADES DE NAVEGAÇÃO QUE CAUSEM CICLOS AQUI!
        // Como você já observou, o seu CampeonatoDto NÃO TEM estas, o que é ótimo!
        // Ex: public List<RodadaOutputDto> Rodadas { get; set; }
        // Ex: public List<EquipeCampeonatoOutputDto> EquipesCampeonatos { get; set; }
        // Ex: public List<ApostadorCampeonatoOutputDto> ApostadoresCampeonatos { get; set; }
    }
}
