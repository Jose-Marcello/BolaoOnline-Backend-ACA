🏆 ApostasApp.Core - Back-end (.NET 8)
Este repositório contém o núcleo de processamento e inteligência de negócio do sistema de Bolão Online, desenvolvido com o que há de mais moderno no ecossistema .NET. A arquitetura foi desenhada para ser resiliente, testável e escalável, utilizando princípios de Clean Architecture e Domain-Driven Design (DDD).

🏗️ Arquitetura e Design Patterns
A solução é dividida em camadas bem definidas, garantindo que as regras de negócio permaneçam isoladas de detalhes de infraestrutura:

ApostasApp.Core.Domain: O coração da aplicação. Contém as Entidades, Interfaces, Results e a lógica pura de negócio, totalmente desacoplada de frameworks externos.

ApostasApp.Core.Application: Camada de orquestração que contém os DTOs, Services, Mappings (AutoMapper) e validações de fluxo (FluentValidation).

ApostasApp.Core.Infrastructure: Implementação técnica. Aqui reside o acesso a dados com Entity Framework Core, configurações de banco de dados via FluentAPI, serviços de E-mail (SendGrid) e Identity para segurança.

ApostasApp.Core.Web (API): Ponto de entrada via RESTful API, documentada com Swagger, onde são gerenciadas as Controllers e a Injeção de Dependência.

🛠️ Expertise Técnica Implementada
O projeto demonstra domínio avançado em padrões e ferramentas essenciais para um Consultor Sênior:

Injeção de Dependência (DI): Gerenciamento nativo de ciclo de vida de objetos para garantir baixo acoplamento.

Unit of Work (UoW): Implementação de transações atômicas para assegurar a integridade dos dados em operações complexas.

ORM Eficiente: Uso de EF Core com suporte a SQL Server e PostgreSQL, utilizando FluentAPI para uma modelagem de banco de dados limpa e desacoplada das entidades.

Validation & Mapping: Uso de FluentValidation para regras de entrada robustas e AutoMapper para conversão eficiente entre entidades e DTOs.

Segurança: Implementação de autenticação e autorização via ASP.NET Core Identity.

🚀 Roadmap de Evolução
O projeto está em constante evolução, com foco em escalabilidade horizontal:

[x] Implementação de Gateway de E-mail (SendGrid).

[ ] Mensageria com RabbitMQ: Processamento assíncrono de apostas e notificações.

[ ] Gateways de Pagamento: Integração para automação de transações financeiras.

[ ] Microserviços: Migração de módulos críticos para serviços independentes.

⚙️ CI/CD e Infraestrutura
Utilizo GitHub Actions e Azure DevOps para automação de builds e deploys, garantindo uma esteira de entrega contínua (CI/CD) profissional, além de suporte a ambientes containerizados com Docker.