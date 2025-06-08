using ApostasApp.Core.Application.DTOs.Apostas; // Para PalpiteDto, SalvarPalpiteRequestDto
using ApostasApp.Core.Application.Services.Interfaces.Palpites; // Para IPalpiteService (do serviço de aplicação)
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Domain.Interfaces.Apostas; // Para IPalpiteRepository (do domínio)
using ApostasApp.Core.Domain.Models.Apostas; // Para Palpite (entidade de domínio)
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApostasApp.Core.Application.Services.Base; // Para AnyAsync no PalpiteRepository.Buscar

namespace ApostasApp.Core.Application.Services.Palpites
{
    /// <summary>
    /// Serviço de aplicação para operações relacionadas a palpites/apostas.
    /// Implementa a interface IPalpiteService e utiliza IPalpiteRepository para acesso a dados.
    /// </summary>
    public class PalpiteService : BaseService, IPalpiteService
    {
        private readonly IPalpiteRepository _palpiteRepository;
        private readonly IMapper _mapper;

        public PalpiteService(IPalpiteRepository palpiteRepository,
                              IMapper mapper,
                              INotificador notificador,
                              IUnitOfWork uow) : base(notificador, uow)
        {
            _palpiteRepository = palpiteRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém uma coleção de palpites para uma rodada específica.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>Uma coleção de DTOs de palpites.</returns>
        public async Task<IEnumerable<PalpiteDto>> ObterPalpitesDaRodada(Guid rodadaId)
        {
            var palpites = await _palpiteRepository.ObterPalpitesDaRodada(rodadaId);

            if (!palpites.Any())
            {
                Notificar("Alerta", "Nenhum palpite encontrado para a rodada especificada.");
                return new List<PalpiteDto>();
            }

            return _mapper.Map<IEnumerable<PalpiteDto>>(palpites);
        }

        /// <summary>
        /// Adiciona um novo palpite.
        /// </summary>
        /// <param name="palpiteRequest">O DTO de requisição contendo os dados do palpite.</param>
        /// <returns>O DTO do palpite adicionado, ou null se falhar.</returns>
        public async Task<PalpiteDto> AdicionarPalpite(SalvarPalpiteRequestDto palpiteRequest)
        {
            // Mapeia o DTO de requisição para a entidade de domínio
            var palpite = _mapper.Map<Palpite>(palpiteRequest);

            // Adicione validações de negócio aqui (ex: verificar se o jogo está aberto para palpites)
            // if (!ExecutarValidacao(new PalpiteValidation(), palpite)) return null; // Se você tiver um PalpiteValidation

            // A pontuação inicial é 0 e é calculada depois que o jogo termina.
            palpite.Pontos = 0; // Ajustado para 'Pontos' conforme sua correção.

            await _palpiteRepository.Adicionar(palpite);
            var saved = await Commit();

            if (saved)
            {
                Notificar("Sucesso", "Palpite adicionado com sucesso!");
                return _mapper.Map<PalpiteDto>(palpite);
            }
            else
            {
                Notificar("Erro", "Não foi possível adicionar o palpite.");
                return null;
            }
        }

        /// <summary>
        /// Atualiza um palpite existente.
        /// </summary>
        /// <param name="palpiteId">O ID do palpite a ser atualizado.</param>
        /// <param name="palpiteRequest">O DTO de requisição contendo os novos dados do palpite.</param>
        /// <returns>O DTO do palpite atualizado, ou null se falhar.</returns>
        public async Task<PalpiteDto> AtualizarPalpite(Guid palpiteId, SalvarPalpiteRequestDto palpiteRequest)
        {
            var palpiteExistente = await _palpiteRepository.ObterPorId(palpiteId);
            if (palpiteExistente == null)
            {
                Notificar("Erro", "Palpite não encontrado para atualização.");
                return null;
            }

            // Atualiza as propriedades da entidade existente com os dados do DTO de requisição
            palpiteExistente.PlacarApostaCasa = palpiteRequest.PlacarApostaCasa;
            palpiteExistente.PlacarApostaVisita = palpiteRequest.PlacarApostaVisita;
            // O campo 'Pontos' não é atualizado aqui, pois ele é uma pontuação calculada.
            // As validações de negócio devem ser adicionadas aqui (ex: verificar se o jogo ainda permite atualização de palpites)

            await _palpiteRepository.Atualizar(palpiteExistente);
            var saved = await Commit();

            if (saved)
            {
                Notificar("Sucesso", "Palpite atualizado com sucesso!");
                return _mapper.Map<PalpiteDto>(palpiteExistente);
            }
            else
            {
                Notificar("Erro", "Não foi possível atualizar o palpite.");
                return null;
            }
        }

        /// <summary>
        /// Remove um palpite pelo seu ID.
        /// </summary>
        /// <param name="palpiteId">O ID do palpite a ser removido.</param>
        /// <returns>True se a remoção foi bem-sucedida, false caso contrário.</returns>
        public async Task<bool> RemoverPalpite(Guid palpiteId)
        {
            var palpite = await _palpiteRepository.ObterPorId(palpiteId);
            if (palpite == null)
            {
                Notificar("Erro", "Palpite não encontrado para remoção.");
                return false;
            }

            await _palpiteRepository.Remover(palpite);
            var saved = await Commit();

            if (saved)
            {
                Notificar("Sucesso", "Palpite removido com sucesso!");
                return true;
            }
            else
            {
                Notificar("Erro", "Não foi possível remover o palpite.");
                return false;
            }
        }

        /// <summary>
        /// Verifica se existem apostas (palpites) para uma rodada específica.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>True se existirem palpites, false caso contrário.</returns>
        public async Task<bool> ExistePalpitesParaRodada(Guid rodadaId)
        {
            return await _palpiteRepository.Buscar(p => p.Jogo.RodadaId == rodadaId).AnyAsync();
        }
    }
}
