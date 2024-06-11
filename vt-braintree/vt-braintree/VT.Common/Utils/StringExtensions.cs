using System;
using System.Globalization;
using System.Text;

namespace VT.Common.Utils
{
    public static class StringExtensions
    {
        public static StringBuilder AppendLineIfNotEmpty(this StringBuilder sb, string line)
        {
            if (!string.IsNullOrEmpty(line))
                sb.AppendLine(line);

            return sb;
        }

        public static string TakeFromLeft(this string str, int amount, string suffix = "...")
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (str.Length <= amount)
                return str;

            return str.Substring(0, amount) + suffix;
        }

        public static string TakeFromRight(this string str, int amount)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (str.Length <= amount)
                return str;

            return str.Substring(str.Length - amount);
        }

        public static string Or(this string str, string alternative)
        {
            if (string.IsNullOrEmpty(str))
                return alternative;

            return str;
        }

        public static string ToSafeString(this string str)
        {
            if (str == null)
                return string.Empty;

            return str;
        }

        public static string Wrap(this string str, string prefix, string suffix)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            return string.Concat(prefix, str, suffix);
        }

        public static DateTime? GetConvertedDate(this string date)
        {
            DateTime? resultantDate = null;

            if (string.IsNullOrEmpty(date)) return null;

            DateTime dt;
            var converted = DateTime.TryParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
            resultantDate = converted ? dt : (DateTime?)null;
            return resultantDate;
        }
    }
}
