using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Specialized;

namespace CrawlDataWebsiteToolBasic.Helpers
{
    public static class StringHelper
    {
        private static string RemoveSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }

        public static string RemoveBreakLineTab(this string input)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input)) return "";

            var line = input
                .Replace("\t", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .RemoveSpaces()
                .Trim();

            return line;
        }

        public static string ReplaceMultiToEmpty(this string input, IEnumerable<string> listForRemove)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input)) return "";

            return listForRemove.Aggregate(input, (current, t) => current.Replace(t, string.Empty));
        }

        public static string ProcessUrl(string url)
        {
            string[] parts = url.Split('=');

            if (parts.Length >= 3)
            {
                return parts[2];
            }

            return "";
        }

        public static string ProcessTel(string tel)
        {
            string[] parts = tel.Split(':');

            if (parts.Length >= 2)
            {
                return parts[1];
            }

            return "";
        }

    }

}