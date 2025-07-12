using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Domain.Models.Usuarios; // Certifique-se que este using está presente

namespace ApostasApp.Core.Domain.Models.Apostadores
{
    public class Apostador : Entity
    {
        public Apostador()
        {
            ApostadoresCampeonatos = new List<ApostadorCampeonato>();
        }            

        // CORRIGIDO: Chave estrangeira para o Usuário - DEVE SER STRING PARA O IDENTITY
        public string UsuarioId { get; set; } // <<-- Voltou a ser string!

        public Saldo Saldo { get; set; } // Propriedade de navegação para Saldo

        public string NomeCompleto { get; set; }       
       
        public StatusApostador Status { get; set; } = StatusApostador.AguardandoAssociacao;

        /* EF Relations (Propriedades de Navegação) */
        public Usuario Usuario { get; set; } // Isso está correto, pois Usuario.Id também é string no Identity

        public ICollection<ApostadorCampeonato> ApostadoresCampeonatos { get; set; }
        

       
    }
}