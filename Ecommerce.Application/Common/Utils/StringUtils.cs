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
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Bước 1: Normalize Unicode
            var normalizedString = text.Normalize(NormalizationForm.FormD);

            // Bước 2: Chuyển đổi ký tự tiếng Việt
            var vietnameseConverted = ConvertVietnameseChars(normalizedString);

            // Bước 3: Loại bỏ dấu thanh điệu còn lại
            var stringBuilder = new StringBuilder();
            foreach (var c in vietnameseConverted)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // Bước 4: Chuyển về chữ thường
            var slug = stringBuilder.ToString().ToLowerInvariant();

            // Bước 5: Thay thế khoảng trắng và ký tự đặc biệt bằng dấu gạch ngang
            slug = slug.Replace(" ", "-")
                       .Replace("_", "-")
                       .Replace(".", "-")
                       .Replace(",", "-")
                       .Replace("(", "")
                       .Replace(")", "")
                       .Replace("[", "")
                       .Replace("]", "")
                       .Replace("{", "")
                       .Replace("}", "")
                       .Replace("\"", "")
                       .Replace("'", "")
                       .Replace("`", "")
                       .Replace("~", "")
                       .Replace("!", "")
                       .Replace("@", "")
                       .Replace("#", "")
                       .Replace("$", "")
                       .Replace("%", "")
                       .Replace("^", "")
                       .Replace("&", "")
                       .Replace("*", "")
                       .Replace("+", "")
                       .Replace("=", "")
                       .Replace("|", "")
                       .Replace("\\", "")
                       .Replace("/", "")
                       .Replace("?", "")
                       .Replace("<", "")
                       .Replace(">", "")
                       .Replace(":", "")
                       .Replace(";", "");

            // Bước 6: Loại bỏ ký tự không hợp lệ
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Bước 7: Loại bỏ dấu gạch ngang liên tiếp
            slug = Regex.Replace(slug, @"-+", "-");

            // Bước 8: Loại bỏ dấu gạch ngang ở đầu và cuối
            slug = slug.Trim('-');

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


        private static readonly Dictionary<string, string> VietnameseMap = new Dictionary<string, string>
        {
            // Chữ a
            {"à", "a"}, {"á", "a"}, {"ạ", "a"}, {"ả", "a"}, {"ã", "a"},
            {"â", "a"}, {"ầ", "a"}, {"ấ", "a"}, {"ậ", "a"}, {"ẩ", "a"}, {"ẫ", "a"},
            {"ă", "a"}, {"ằ", "a"}, {"ắ", "a"}, {"ặ", "a"}, {"ẳ", "a"}, {"ẵ", "a"},
        
            // Chữ e
            {"è", "e"}, {"é", "e"}, {"ẹ", "e"}, {"ẻ", "e"}, {"ẽ", "e"},
            {"ê", "e"}, {"ề", "e"}, {"ế", "e"}, {"ệ", "e"}, {"ể", "e"}, {"ễ", "e"},
        
            // Chữ i
            {"ì", "i"}, {"í", "i"}, {"ị", "i"}, {"ỉ", "i"}, {"ĩ", "i"},
        
            // Chữ o
            {"ò", "o"}, {"ó", "o"}, {"ọ", "o"}, {"ỏ", "o"}, {"õ", "o"},
            {"ô", "o"}, {"ồ", "o"}, {"ố", "o"}, {"ộ", "o"}, {"ổ", "o"}, {"ỗ", "o"},
            {"ơ", "o"}, {"ờ", "o"}, {"ớ", "o"}, {"ợ", "o"}, {"ở", "o"}, {"ỡ", "o"},
        
            // Chữ u
            {"ù", "u"}, {"ú", "u"}, {"ụ", "u"}, {"ủ", "u"}, {"ũ", "u"},
            {"ư", "u"}, {"ừ", "u"}, {"ứ", "u"}, {"ự", "u"}, {"ử", "u"}, {"ữ", "u"},
        
            // Chữ y
            {"ỳ", "y"}, {"ý", "y"}, {"ỵ", "y"}, {"ỷ", "y"}, {"ỹ", "y"},
        
            // Chữ đ
            {"đ", "d"},
        
            // Viết hoa
            {"À", "a"}, {"Á", "a"}, {"Ạ", "a"}, {"Ả", "a"}, {"Ã", "a"},
            {"Â", "a"}, {"Ầ", "a"}, {"Ấ", "a"}, {"Ậ", "a"}, {"Ẩ", "a"}, {"Ẫ", "a"},
            {"Ă", "a"}, {"Ằ", "a"}, {"Ắ", "a"}, {"Ặ", "a"}, {"Ẳ", "a"}, {"Ẵ", "a"},

            {"È", "e"}, {"É", "e"}, {"Ẹ", "e"}, {"Ẻ", "e"}, {"Ẽ", "e"},
            {"Ê", "e"}, {"Ề", "e"}, {"Ế", "e"}, {"Ệ", "e"}, {"Ể", "e"}, {"Ễ", "e"},

            {"Ì", "i"}, {"Í", "i"}, {"Ị", "i"}, {"Ỉ", "i"}, {"Ĩ", "i"},

            {"Ò", "o"}, {"Ó", "o"}, {"Ọ", "o"}, {"Ỏ", "o"}, {"Õ", "o"},
            {"Ô", "o"}, {"Ồ", "o"}, {"Ố", "o"}, {"Ộ", "o"}, {"Ổ", "o"}, {"Ỗ", "o"},
            {"Ơ", "o"}, {"Ờ", "o"}, {"Ớ", "o"}, {"Ợ", "o"}, {"Ở", "o"}, {"Ỡ", "o"},

            {"Ù", "u"}, {"Ú", "u"}, {"Ụ", "u"}, {"Ủ", "u"}, {"Ũ", "u"},
            {"Ư", "u"}, {"Ừ", "u"}, {"Ứ", "u"}, {"Ự", "u"}, {"Ử", "u"}, {"Ữ", "u"},

            {"Ỳ", "y"}, {"Ý", "y"}, {"Ỵ", "y"}, {"Ỷ", "y"}, {"Ỹ", "y"},

            {"Đ", "d"}
        };
        public static string GenerateSlug(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            // Bước 1: Normalize Unicode để xử lý các ký tự tổ hợp
            var normalizedString = name.Normalize(NormalizationForm.FormD);

            // Bước 2: Chuyển đổi ký tự tiếng Việt
            var vietnameseConverted = ConvertVietnameseChars(normalizedString);

            // Bước 3: Loại bỏ dấu thanh điệu còn lại (nếu có)
            var stringBuilder = new StringBuilder();
            foreach (var c in vietnameseConverted)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // Bước 4: Chuyển về chữ thường
            var slug = stringBuilder.ToString().ToLowerInvariant();

            // Bước 5: Thay thế khoảng trắng và ký tự đặc biệt
            slug = slug.Replace(" ", "-")
                      .Replace("_", "-")
                      .Replace(".", "-")
                      .Replace(",", "-")
                      .Replace("(", "")
                      .Replace(")", "")
                      .Replace("[", "")
                      .Replace("]", "")
                      .Replace("{", "")
                      .Replace("}", "")
                      .Replace("\"", "")
                      .Replace("'", "")
                      .Replace("`", "")
                      .Replace("~", "")
                      .Replace("!", "")
                      .Replace("@", "")
                      .Replace("#", "")
                      .Replace("$", "")
                      .Replace("%", "")
                      .Replace("^", "")
                      .Replace("&", "")
                      .Replace("*", "")
                      .Replace("+", "")
                      .Replace("=", "")
                      .Replace("|", "")
                      .Replace("\\", "")
                      .Replace("/", "")
                      .Replace("?", "")
                      .Replace("<", "")
                      .Replace(">", "")
                      .Replace(":", "")
                      .Replace(";", "");

            // Bước 6: Loại bỏ tất cả ký tự không phải a-z, 0-9, dấu gạch ngang
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Bước 7: Loại bỏ nhiều dấu gạch ngang liên tiếp
            slug = Regex.Replace(slug, @"-+", "-");

            // Bước 8: Loại bỏ dấu gạch ngang ở đầu và cuối
            slug = slug.Trim('-');

            return slug;
        }
        private static string ConvertVietnameseChars(string input)
        {
            var result = new StringBuilder();

            foreach (var c in input)
            {
                var charStr = c.ToString();
                if (VietnameseMap.ContainsKey(charStr))
                {
                    result.Append(VietnameseMap[charStr]);
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }
        public static string GenerateSlugSimple(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            // Chuyển đổi ký tự đặc biệt tiếng Việt
            var slug = ConvertVietnameseChars(name);

            // Loại bỏ dấu thanh điệu
            slug = RemoveDiacritics(slug);

            // Chuyển về chữ thường
            slug = slug.ToLowerInvariant();

            // Thay thế khoảng trắng bằng dấu gạch ngang
            slug = Regex.Replace(slug, @"\s+", "-");

            // Loại bỏ ký tự không mong muốn
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Loại bỏ nhiều dấu gạch ngang liên tiếp
            slug = Regex.Replace(slug, @"-+", "-");

            // Loại bỏ dấu gạch ngang ở đầu và cuối
            slug = slug.Trim('-');

            return slug;
        }

    }
}
