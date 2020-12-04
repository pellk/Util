using System.Text.RegularExpressions;

namespace nuell
{
    public static class Util
    {
        public static string RemoveTags(this string s, int maxLength = 0)
        {
            s = Regex.Replace(s, @"<[^<>]*>", "");
            return maxLength > 0 && s.Length > maxLength
                ? s.Substring(0, maxLength) + "..." : s;
        }

        public static string UrlSlug(this string phrase)
        {
            string str = NumeralConvert.LatinDigits(phrase.ToLowerInvariant());
            str = Regex.Replace(str, @"[^a-z0-9آابپتثجچحخدذرزژسشصضطظعغفقکگلمنوهیكيئؤأإءڕڤۆێھە\s-]", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str.Trim(), @"\s+", "-");
            return str;
        }

        public static string Nullable(this string s)
            => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        public static string Compress(string code)
        {
            string s2 = Regex.Replace(code, @"([;{})>+-,:])\s*\n\s*", "$1", RegexOptions.Multiline);
            s2 = Regex.Replace(s2, @"\s*\n\s*",  " ", RegexOptions.Multiline);
            return Regex.Replace(s2, @"\s*([(){},;:=<>/*+\-?&|])\s*", "$1", RegexOptions.Multiline);
        }
    }
}