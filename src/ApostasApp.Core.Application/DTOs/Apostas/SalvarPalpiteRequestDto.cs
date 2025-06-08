using System;
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.Apostas
{
    /// <summary>
    /// DTO para a requisição de adicionar ou atualizar um palpite.
    /// Contém os dados necessários para registrar um palpite de jogo.
    /// </summary>
    public class SalvarPalpiteRequestDto
    {
        [Required(ErrorMessage = "O ID do Jogo é obrigatório.")]
        public Guid JogoId { get; set; }

        [Required(ErrorMessage = "O ID do Apostador Campeonato é obrigatório.")]
        public Guid ApostadorCampeonatoId { get; set; }

        [Required(ErrorMessage = "O placar da casa é obrigatório.")]
        [Range(0, 99, ErrorMessage = "O placar da casa deve ser entre 0 e 99.")] // Exemplo de validação
        public int PlacarApostaCasa { get; set; }

        [Required(ErrorMessage = "O placar do visitante é obrigatório.")]
        [Range(0, 99, ErrorMessage = "O placar do visitante deve ser entre 0 e 99.")] // Exemplo de validação
        public int PlacarApostaVisita { get; set; }

        // Pontuação será calculada pelo sistema, não enviada pelo usuário
        // DataHoraAposta será preenchida no serviço, não enviada pelo usuário
    }
}
