// Localização: ApostasApp.Core.Infrastructure\Identity\Seed\IdentitySeed.cs

using Microsoft.AspNetCore.Identity;
using ApostasApp.Core.Domain.Models.Usuarios; // Para a classe Usuario
using System.Threading.Tasks;
using System.Linq;
using System;

namespace ApostasApp.Core.Infrastructure.Identity.Seed
{
    public static class IdentitySeed
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // Cria a role 'Admin' se ela não existir
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            // Adicione outras roles aqui, se necessário
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }

        public static async Task SeedAdminUserAsync(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Verifica se o usuário admin já existe
            var adminUser = await userManager.FindByEmailAsync("admin@apostasapp.com");
            if (adminUser == null)
            {
                // Cria o usuário admin
                adminUser = new Usuario
                {
                  UserName = "josemarcellogardeldealemar@gmail.com", 
                  Email = "josemarcellogardeldealemar@gmail.com",
                  Apelido = "AdminMaster",
                  CPF = "84062274787", // CPF de exemplo corrigido
                  Celular = "21999734776", // Celular de exemplo
                  EmailConfirmed = true, // Confirma o e-mail para o admin
                  RegistrationDate = DateTime.Now,
                  RefreshToken = "", // Adicione um valor padrão
                  RefreshTokenExpiryTime = DateTime.UtcNow, // Adicione um valor padrão
                  TwoFactorEnabled = false,
                  AccessFailedCount = 0,
                  TermsAccepted = false // Ou true, dependendo da sua regra de negócio
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123"); // Senha forte para o admin
                if (result.Succeeded)
                {
                    // Adiciona o usuário à role 'Admin'
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    // Adiciona o usuário à role 'User' também, se for o caso
                    await userManager.AddToRoleAsync(adminUser, "User");
                }
                else
                {
                    // Logar erros se a criação do admin falhar
                    // Em um ambiente real, você logaria isso para depuração
                    Console.WriteLine($"Erro ao criar usuário admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
