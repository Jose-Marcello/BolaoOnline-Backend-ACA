// Ex: ApostasApp.Core.Application/Utils/StringExtensions.cs
// Ou crie uma nova classe CpfHelper.cs

using System.Text.RegularExpressions;

namespace ApostasApp.Core.Application.Utils
{
    public static class StringExtensions
    {
        /// <summary>
        /// Limpa uma string, removendo todos os caracteres não numéricos.
        /// Útil para CPFs, telefones, etc.
        /// </summary>
        /// <param name="input">A string a ser limpa.</param>
        /// <returns>A string contendo apenas dígitos.</returns>
        public static string CleanNumbers(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            // Usa Regex para remover tudo que não for dígito (0-9)
            return Regex.Replace(input, @"[^\d]", "");
        }
    }
}
