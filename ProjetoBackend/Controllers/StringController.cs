using System.Globalization;
using System.Text;

namespace ProjetoBackend.Controllers
{
    public static class StringController
    {
        public static string RemoverAcentos(this string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return texto;

            var normalizedString = texto.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}

