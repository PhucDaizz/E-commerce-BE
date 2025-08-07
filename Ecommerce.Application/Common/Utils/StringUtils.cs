using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ecommerce.Application.Common.Utils
{
    public static class StringUtils
    {
        public static string Slugify(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            var slug = RemoveDiacritics(text);
            slug = Regex.Replace(slug, @"\s+", "_").ToLower(); 
            slug = Regex.Replace(slug, @"[^a-z0-9_\-]", ""); 
            return slug;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var c in normalizedString.EnumerateRunes())
            {
                if (Rune.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
