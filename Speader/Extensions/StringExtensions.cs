using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Speader.Extensions
{
    public static class StringExtensions
    {
        public static string StripHtmlTags(this string input)
        {
            var stripHtmlExp = new Regex(@"(<\/?[^>]+>)");
            
            return WebUtility.HtmlDecode(stripHtmlExp.Matches(input).Cast<Match>().Aggregate(input, (current, tag) => ReplaceFirst(current, tag.Value, string.Empty)));
        }

        private static string ReplaceFirst(string haystack, string needle, string replacement)
        {
            int pos = haystack.IndexOf(needle, StringComparison.Ordinal);
            if (pos < 0) return haystack;
            return haystack.Substring(0, pos) + replacement + haystack.Substring(pos + needle.Length);
        }

        public static List<string> ToWordList(this string[] words, int wordsAtATime)
        {
            var i = 0;
            var wordCount = 0;
            var list = new List<string>();
            var stringToAdd = string.Empty;
            foreach (var word in words)
            {
                if (i < wordsAtATime)
                {
                    stringToAdd += " " + word;
                    if (wordCount >= words.Length - 1)
                    {
                        list.Add(stringToAdd);
                    }
                }
                else
                {
                    i = 0;
                    list.Add(stringToAdd.TrimStart());
                    stringToAdd = word;
                }
                i++;
                wordCount++;
            }

            return list;
        }

        public static string ToExcerpt(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            return text.Length > 200 ? text.Substring(0, 200).Clean() : text.Clean();
        }

        public static string Clean(this string text)
        {
            return text.Replace("\n", " ").Replace("\r", " ").Replace("  ", " ").Trim();
        }
    }
}
