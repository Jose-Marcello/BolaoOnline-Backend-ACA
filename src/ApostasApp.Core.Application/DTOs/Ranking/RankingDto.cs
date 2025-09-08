namespace ApostasApp.Core.Application.DTOs.Ranking
{
    public class RankingDto
    {
        public Guid ApostadorId { get; set; }
        public string NomeApostador { get; set; }
        public int Pontuacao { get; set; }
        public int Posicao { get; set; } // Adicionado para a posição no ranking
        public DateTime DataAtualizacao { get; set; }
        public string Apelido { get; set; }
        public string FotoPerfil { get; set; }
    }
}
