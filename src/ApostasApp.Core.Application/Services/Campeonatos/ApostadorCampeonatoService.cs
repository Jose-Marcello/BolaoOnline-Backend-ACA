using ApostasApp.Core.Application.Services.Interfaces;
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos;
using ApostasApp.Core.Application.Validations;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Application.DTOs.ApostadorCampeonatos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApostasApp.Core.Application.Services.Base; // Para AnyAsync

namespace ApostasApp.Core.Application.Services.Campeonatos
{
    public class ApostadorCampeonatoService : BaseService, IApostadorCampeonatoService
    {
        private readonly IApostadorCampeonatoRepository _apostadorCampeonatoRepository;
        private readonly IMapper _mapper;

        public ApostadorCampeonatoService(IApostadorCampeonatoRepository apostadorCampeonatoRepository,
                                          IMapper mapper,
                                          INotificador notificador,
                                          IUnitOfWork uow) : base(notificador, uow)
        {
            _apostadorCampeonatoRepository = apostadorCampeonatoRepository;
            _mapper = mapper;
        }

        public async Task<ApostadorCampeonato> ObterPorId(Guid apostadorCampeonatoId)
        {
            return await _apostadorCampeonatoRepository.ObterPorId(apostadorCampeonatoId);
        }

        public async Task Adicionar(ApostadorCampeonato apostadorCampeonato)
        {
            //if (!ExecutarValidacao(new ApostadorCampeonatoValidation(), apostadorCampeonato))
            //    return;

            if (await _apostadorCampeonatoRepository.Buscar(ec => ec.CampeonatoId == apostadorCampeonato.CampeonatoId
                                                             && ec.ApostadorId == apostadorCampeonato.ApostadorId).AnyAsync())
            {
                Notificar("Alerta", "Este APOSTADOR já foi associado a este CAMPEONATO!");
                return;
            }

            await _apostadorCampeonatoRepository.Adicionar(apostadorCampeonato);

            await CommitAsync();
            Notificar("Sucesso", "Associação Apostador-Campeonato adicionada com sucesso!");
        }

        public async Task Remover(ApostadorCampeonato apostadorCampeonato)
        {
            if (apostadorCampeonato == null)
            {
                Notificar("Alerta", "Associação Apostador-Campeonato não encontrada para remoção.");
                return;
            }

            await _apostadorCampeonatoRepository.Remover(apostadorCampeonato);
            await CommitAsync();
            Notificar("Sucesso", "Associação Apostador-Campeonato removida com sucesso!");
        }

        public async Task<ApostadorCampeonato> ObterApostadorCampeonatoPorApostadorECampeonato(string usuarioId, Guid campeonatoId)
        {
            return await _apostadorCampeonatoRepository.ObterApostadorCampeonatoPorApostadorECampeonato(usuarioId, campeonatoId);
        }

        /// <summary>
        /// Obtém uma lista de apostadores associados a um campeonato específico,
        /// incluindo informações do apostador e do usuário.
        /// </summary>
        /// <param name="campeonatoId">O ID do campeonato.</param>
        /// <returns>Uma coleção de DTOs de ApostadorCampeonato.</returns>
        public async Task<IEnumerable<ApostadorCampeonatoDto>> ObterApostadoresDoCampeonato(Guid campeonatoId)
        {
            // Chamando o método existente do repositório, que agora inclui Apostador e Usuario
            var apostadoresCampeonato = await _apostadorCampeonatoRepository.ObterApostadoresDoCampeonato(campeonatoId);

            if (!apostadoresCampeonato.Any())
            {
                Notificar("Alerta", "Nenhum apostador encontrado para este campeonato.");
                return new List<ApostadorCampeonatoDto>();
            }

            // Mapeia as entidades de domínio para os DTOs
            return _mapper.Map<IEnumerable<ApostadorCampeonatoDto>>(apostadoresCampeonato);
        }

        
    }
}
