using AutoMapper;
using ApostasApp.Core.Application.DTOs.Jogos; // Para JogoDetalheDto, JogoViewModel
using ApostasApp.Core.Application.Services.Interfaces.Jogos; // Para IJogoService
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork (se BaseService o expõe como protected)
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Domain.Interfaces.Jogos; // Para IJogoRepository
using ApostasApp.Core.Domain.Models.Jogos;

namespace ApostasApp.Core.Application.Services.Jogos
{
    /// <summary>
    /// Serviço de aplicação para operações relacionadas a Jogos.
    /// Delega a persistência e notificação para a BaseService.
    /// </summary>
    public class JogoService : BaseService, IJogoService
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly IMapper _mapper;

        // O construtor agora recebe IUnitOfWork e o passa para a BaseService.
        // A BaseService é responsável por gerenciar o IUnitOfWork.
        public JogoService(IJogoRepository jogoRepository,
                           IMapper mapper,
                           INotificador notificador,
                           IUnitOfWork uow) : base(notificador, uow) // <--- CORREÇÃO AQUI!
        {
            _jogoRepository = jogoRepository;
            _mapper = mapper;
            // Não é necessário atribuir _unitOfWork aqui, pois a BaseService já faz isso.
        }

        /// <summary>
        /// Obtém os detalhes de um jogo específico, incluindo informações de rodada, campeonato, equipes e estádio.
        /// </summary>
        /// <param name="id">O ID do jogo.</param>
        /// <returns>Um DTO com os detalhes do jogo, ou null se não encontrado.</returns>
        public async Task<JogoDetalheDto> ObterDetalhesJogo(Guid id)
        {
            var jogo = await _jogoRepository.ObterJogoComRodadaCampeonatoEquipesEstadio(id);

            if (jogo == null)
            {
                Notificar("Alerta", "Jogo não encontrado.");
                return null;
            }

            return _mapper.Map<JogoDetalheDto>(jogo);
        }

        /// <summary>
        /// Obtém uma coleção de jogos para uma rodada específica, formatados para exibição,
        /// incluindo informações de equipes e estádio.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>Uma coleção de DTOs de jogos.</returns>
        public async Task<IEnumerable<JogoDto>> ObterJogosDaRodada(Guid rodadaId)
        {
            var jogos = await _jogoRepository.ObterJogosDaRodadaComEquipesEEstadio(rodadaId);

            if (!jogos.Any())
            {
                Notificar("Alerta", "Nenhum jogo encontrado para a rodada especificada.");
                return new List<JogoDto>();
            }

            return _mapper.Map<IEnumerable<JogoDto>>(jogos);
        }

        /// <summary>
        /// Adiciona um novo jogo.
        /// </summary>
        /// <param name="jogoDetalheDto">Os dados do jogo a ser adicionado.</param>
        /// <returns>O JogoDetalheDto do jogo adicionado, ou null em caso de falha.</returns>
        public async Task<JogoDetalheDto> AdicionarJogo(JogoDetalheDto jogoDetalheDto)
        {
            // Mapeia o DTO para a entidade de domínio
            var jogo = _mapper.Map<Jogo>(jogoDetalheDto);

            // Exemplo de alguma validação de negócio antes de adicionar
            // Assumindo que JogoDetalheDto tem DataJogo e HoraJogo
            DateTime dataHoraJogo = jogoDetalheDto.DataJogo.Date + jogoDetalheDto.HoraJogo;
            if (dataHoraJogo < DateTime.Now)
            {
                Notificar("Erro", "Não é possível adicionar jogos com data/hora no passado.");
                return null;
            }

            // Adiciona a entidade ao repositório
            await _jogoRepository.Adicionar(jogo);

            // Chama o Commit() da BaseService para persistir as alterações
            // A BaseService é responsável por chamar _unitOfWork.CommitAsync()
            var saved = await CommitAsync(); // Assumindo que BaseService tem um método Commit()

            if (saved)
            {
                Notificar("Sucesso", "Jogo adicionado com sucesso.");
                return _mapper.Map<JogoDetalheDto>(jogo);
            }
            else
            {
                Notificar("Erro", "Não foi possível adicionar o jogo.");
                return null;
            }
        }

        // Você pode adicionar outros métodos CRUD (Atualizar, Remover) aqui,
        // sempre chamando Commit() da BaseService após as operações de repositório.
    }
}
