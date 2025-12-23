
using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Domain.Models.Usuarios;
using ApostasApp.Core.Infrastructure.Services.Email;
using ApostasApp.Core.Infrastructure.Services.Financeiro;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace ApostasApp.Core.Web.Controllers
{
    [ApiController]
    [Route("api/testes")]
    public class TestController : ControllerBase
    {
        // Endpoints para e-mail
        [HttpGet("emails")]
        public IActionResult GetMockEmails([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("O endereço de e-mail não pode ser nulo ou vazio.");
            }
            var emails = MockEmailSender.EmailsEnviados.GetValueOrDefault(email, null);
            if (emails == null || !emails.Any())
            {
                return NotFound("Nenhum e-mail simulado encontrado para o endereço fornecido.");
            }
            return Ok(emails);
        }

        [HttpPost("emails/limpar")]
        public IActionResult ClearMockEmails()
        {
            MockEmailSender.EmailsEnviados.Clear();
            return Ok("Caixa de entrada de e-mails simulados limpa com sucesso.");
        }

        // Endpoints para pagamentos
        /// <summary>
        /// Obtém o status de um pagamento simulado.
        /// </summary>
        /// <param name="idTransacao">O ID da transação de pagamento.</param>
        [HttpGet("pagamentos/{idTransacao}")]
        public IActionResult GetMockPagamento(string idTransacao)
        {
            if (MockPagamentoService.PagamentosSimulados.TryGetValue(idTransacao, out var pagamento))
            {
                return Ok(pagamento);
            }
            return NotFound($"Pagamento com ID {idTransacao} não encontrado.");
        }

        /// <summary>
        /// Simula a chamada de um webhook para alterar o status de um pagamento.
        /// Este endpoint é útil para automatizar testes do fluxo de pagamento.
        /// </summary>
        /// <param name="idTransacao">O ID da transação de pagamento.</param>
        /// <param name="novoStatus">O novo status (ex: "CONCLUIDO", "REJEITADO").</param>
        [HttpPost("pagamentos/{idTransacao}/status")]
        public IActionResult SetMockPagamentoStatus(string idTransacao, [FromQuery] string novoStatus)
        {
            if (MockPagamentoService.PagamentosSimulados.TryGetValue(idTransacao, out var pagamento))
            {
                pagamento.Status = novoStatus.ToUpper();
                return Ok($"Status do pagamento {idTransacao} atualizado para {pagamento.Status}.");
            }
            return NotFound($"Pagamento com ID {idTransacao} não encontrado para atualizar o status.");
        }

        /// <summary>
        /// Limpa todos os pagamentos simulados.
        /// </summary>
        [HttpPost("pagamentos/limpar")]
        public IActionResult LimparMockPagamentos()
        {
            MockPagamentoService.LimparPagamentos();
            return Ok("Pagamentos simulados limpos com sucesso.");
        }


   

        [HttpGet("generate-temp-hash")]
        // 🛑 ATENÇÃO: Use a classe Usuario, conforme seu projeto.
         public ActionResult<string> GenerateTempHash()
         {
            // Crie o PasswordHasher usando o tipo do seu Identity (Usuario)
            var passwordHasher = new PasswordHasher<Usuario>();

            // 🛑 ESCOLHA A SENHA QUE VOCÊ VAI USAR PARA LOGAR 🛑
            // Esta é a senha em texto plano que você usará no Front-end
            string plainPassword = "SenhaTemp@123!";

            // O Identity gera o hash e o salt (componente de segurança)
            string hashedPassword = passwordHasher.HashPassword(null, plainPassword);

            // Retorna o hash para que você possa copiá-lo
            return Ok(hashedPassword);
         }
     }
}
