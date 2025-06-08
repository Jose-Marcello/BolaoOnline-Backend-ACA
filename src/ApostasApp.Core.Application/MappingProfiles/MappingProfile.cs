using ApostasApp.Application.DTOs.Jogos;
using ApostasApp.Core.Application.DTOs.ApostadorCampeonatos; // Para ApostadorCampeonatoDto
using ApostasApp.Core.Application.DTOs.Apostadores; // Para ApostadorDto
using ApostasApp.Core.Application.DTOs.Apostas; // Para PalpiteDto
using ApostasApp.Core.Application.DTOs.Campeonatos; // Para CampeonatoDto
using ApostasApp.Core.Application.DTOs.Equipes; // Para EquipeDto, EquipeCampeonatoDto
using ApostasApp.Core.Application.DTOs.Estadios; // Para EstadioDto
using ApostasApp.Core.Application.DTOs.Financeiro; // Para SaldoDto
using ApostasApp.Core.Application.DTOs.Jogos; // Para JogoDetalheDto, JogoDto
using ApostasApp.Core.Application.DTOs.Rodadas; // Para RodadaDto
using ApostasApp.Core.Application.DTOs.Usuarios; // Para RegisterRequestDto, UsuarioProfileDto
using ApostasApp.Core.Domain.Models.Apostadores; // Para a entidade Apostador
using ApostasApp.Core.Domain.Models.Apostas; // Para a entidade Palpite
using ApostasApp.Core.Domain.Models.Campeonatos; // Para a entidade Campeonato
using ApostasApp.Core.Domain.Models.Equipes; // Para a entidade Equipe, EquipeCampeonato
using ApostasApp.Core.Domain.Models.Estadios; // Para a entidade Estadio
using ApostasApp.Core.Domain.Models.Financeiro; // Para a entidade Saldo
using ApostasApp.Core.Domain.Models.Jogos; // Para a entidade Jogo, StatusJogo
using ApostasApp.Core.Domain.Models.Rodadas; // Para a entidade Rodada
using ApostasApp.Core.Domain.Models.Usuarios; // Para a entidade Usuario
using AutoMapper;

namespace ApostasApp.Core.Application.MappingProfiles
{
    /// <summary>
    /// Perfil de mapeamento do AutoMapper para a aplicação.
    /// Define como as entidades de domínio são mapeadas para os DTOs e vice-versa.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeamentos para a entidade Jogo e seus DTOs
            CreateMap<Jogo, JogoDetalheDto>(); // Detalhes completos do jogo
            CreateMap<Jogo, JogoDto>();        // DTO de jogo para listagens/transferência simples

            // Mapeamentos para a entidade Rodada e seus DTOs
            CreateMap<Rodada, RodadaDto>();

            // Mapeamentos para a entidade Campeonato e seus DTOs
            CreateMap<Campeonato, CampeonatoDto>();

            // Mapeamentos para a entidade Equipe e seus DTOs
            CreateMap<Equipe, EquipeDto>(); // Inclui Escudo e Sigla
            CreateMap<EquipeCampeonato, EquipeCampeonatoDto>();

            // Mapeamentos para a entidade Estadio e seus DTOs
            CreateMap<Estadio, EstadioDto>();

            // Mapeamentos para a entidade Apostador e seus DTOs
            CreateMap<Apostador, ApostadorDto>()
                .ForMember(dest => dest.Apelido, opt => opt.MapFrom(src => src.Usuario.Apelido)) // Mapeia Apelido do Usuario
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email));     // Mapeia Email do Usuario

            // Mapeamentos para a entidade Saldo e seus DTOs
            CreateMap<Saldo, SaldoDto>();

            // Mapeamentos para a entidade ApostadorCampeonato e seus DTOs
            CreateMap<ApostadorCampeonato, ApostadorCampeonatoDto>();

            // Mapeamentos para a entidade Palpite e seus DTOs
            CreateMap<Palpite, PalpiteDto>();

            // Mapeamentos para a entidade Usuario e seus DTOs de autenticação/perfil
            CreateMap<RegisterRequestDto, Usuario>();    // Mapeamento de DTO de requisição para entidade Usuario
            CreateMap<Usuario, UsuarioProfileDto>();     // Mapeamento de entidade Usuario para DTO de perfil de resposta
            // Nota: DTOs como LoginRequestDto, ForgotPasswordRequestDto, ResetPasswordRequestDto
            // geralmente não precisam de mapeamento direto para entidades de domínio,
            // pois seus dados são usados diretamente pelos serviços de identidade.
        }
    }
}
