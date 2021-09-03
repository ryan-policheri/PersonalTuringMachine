using System;
using System.Linq;
using System.Text.Json;

namespace PersonalTuringMachine.Extensions
{
    public static class StringExtensions
    {
        public static string TrimEnd(this string target, string trimString)
        {
            if (string.IsNullOrEmpty(trimString)) return target;

            string result = target;
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, result.Length - trimString.Length);
            }

            return result;
        }

        public static string CapsAndTrim(this string source)
        {
            if (source == null) return null;
            return source.ToUpper().Trim();
        }

        public static string RemoveWhitespace(this string source)
        {
            return new string(source.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }

        public static string Quotify(this string source)
        {
            if (source == null) return null;
            return "\"" + source + "\"";
        }

        public static string ToJson<T>(this T source)
        {
            return JsonSerializer.Serialize<T>(source);
        }

        public static T ConvertJsonToObject<T>(this string source)
        {
            return JsonSerializer.Deserialize<T>(source);
        }
    }
}
