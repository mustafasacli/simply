using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simply.Common
{
    /// <summary>
    /// Defines the <see cref="StringExtension"/>.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Checks string not null and whitespace.
        /// </summary>
        /// <param name="text">The s to act on.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool IsValid(this string text)
        {
            bool result = !string.IsNullOrWhiteSpace(text);

            return result;
        }

        /// <summary>
        /// Length of a string.
        /// </summary>
        /// <param name="text">The s to act on.</param>
        /// <returns>An int.</returns>
        public static int Len(this string text)
        {
            int len = -1;

            if (text != null)
            {
                len = text.Length;
            }

            return len;
        }

        /// <summary>
        /// The Trim All spaces.
        /// </summary>
        /// <param name="text">The s to act on.</param>
        /// <param name="trimNewLine">if true trims new line chars.</param>
        /// <param name="trimTabs">if true trims tab chars.</param>
        /// <returns>A string.</returns>
        public static string TrimAll(this string text, bool trimNewLine = true, bool trimTabs = true)
        {
            if (text.IsNullOrEmpty())
                return text;

            string result = text.Replace(" ", "");

            if (trimNewLine)
                result = result.Replace("\r", "").Replace("\n", "");

            if (trimTabs)
                result = result.Replace("\t", "");

            return result;
        }

        /// <summary>
        /// First Index O f.
        /// </summary>
        /// <param name="text">The str to act on.</param>
        /// <param name="char4Check">The ch.</param>
        /// <returns>An int.</returns>
        public static int FirstIndexOf(this string text, char char4Check)
        {
            int _index = -1;

            if (text.IsNullOrEmpty())
                return _index;

            if (char4Check.IsNull())
                return _index;

            _index = text.IndexOf(char4Check);

            return _index;
        }

        /// <summary>
        /// A string extension method that removes the under line and capitalize string described by
        /// str removes underline and point.
        /// </summary>
        /// <param name="text">The str to act on.</param>
        /// <returns>A string.</returns>
        public static string RemoveUnderLineAndCapitalizeString(this string text)
        {
            if (text.IsNullOrEmpty())
                return text;

            string[] s =
                (text.Split(new char[] { '_', '.' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0])
                .Select(q => q.CapitalizeString())
                .ToArray() ?? ArrayHelper.Empty<string>();

            string result = string.Join(string.Empty, s);

            return result;
        }

        /// <summary>
        /// Capitalize string with given string.
        /// </summary>
        /// <param name="text">The str to act on.</param>
        /// <param name="endPart">The end part.</param>
        /// <returns>A string.</returns>
        public static string CapitalizeEndPart(this string text, string endPart)
        {
            if (text.IsNullOrEmpty())
                return text;

            if (endPart.IsNullOrEmpty())
                return text;

            string result = text;

            if (result.ToLowerInvariant().EndsWith(endPart.ToLowerInvariant()))
            {
                result = text.Substring(0, text.Length - endPart.Length);
                result = string.Concat(result, endPart.CapitalizeString());
            }

            return result;
        }

        /// <summary>
        /// Capitalize String.
        /// </summary>
        /// <param name="text">The str to act on.</param>
        /// <returns>A string.</returns>
        public static string CapitalizeString(this string text)
        {
            string s = text.ToLowerInvariant();

            s = string.Concat(s[0].ToString().ToUpperInvariant(), s.Substring(1));

            return s;
        }

        /// <summary>
        /// Remove Spaces.
        /// </summary>
        /// <param name="text">The str to act on.</param>
        /// <returns>A string.</returns>
        public static string RemoveSpaces(this string text)
        {
            string result = text?.Replace(" ", string.Empty) ?? string.Empty;
            return result;
        }

        /// <summary>
        /// Remove given chars from string.
        /// </summary>
        /// <param name="text">The str to act on.</param>
        /// <param name="chars">A variable-length parameters list containing characters.</param>
        /// <returns>A string.</returns>
        public static string RemoveChars(this string text, char[] chars)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            if (chars == null)
                return text;

            if (chars.Length < 1)
                return text;

            chars
                .ToList()
                .ForEach(q => text = text.Replace(q.ToString(), ""));

            return text;
        }

        /// <summary>
        /// converts first char to upper for given string.
        /// </summary>
        /// <param name="input">The input to act on.</param>
        /// <returns>A string.</returns>
        public static string FirstCharToUpper(this string input)
        {
            if (input.IsNullOrSpace())
                return input;

            return input.First().ToString().ToUpperInvariant() + input.Substring(1);
        }

        /// <summary>
        /// converts first char to lower for given string.
        /// </summary>
        /// <param name="input">The input to act on.</param>
        /// <returns>A string.</returns>
        public static string FirstCharToLower(this string input)
        {
            if (input.IsNullOrSpace())
                return input;

            return input.First().ToString().ToLowerInvariant() + input.Substring(1);
        }

        /// <summary>
        /// converts first char to upper for given string.
        /// </summary>
        /// <param name="text">The s to act on.</param>
        /// <returns>A string.</returns>
        public static string UppercaseFirst(this string text)
        {
            // Check for empty string.
            if (text.IsNullOrSpace())
                return text;

            // Return char and concat substring.
            return char.ToUpperInvariant(text[0]) + text.Substring(1);
        }

        /// <summary>
        /// Replace strings with given dictionary.
        /// </summary>
        /// <param name="text">.</param>
        /// <param name="dictionary">.</param>
        /// <returns>.</returns>
        public static string ReplaceWithDictionary(this string text, Dictionary<string, string> dictionary)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            if (dictionary == null || dictionary.Count == 0)
                return text;

            List<KeyValuePair<string, string>> keyValues = dictionary.Where(q => !string.IsNullOrEmpty(q.Key) && q.Value != null).ToList();
            string s = text.CopyValue();

            foreach (KeyValuePair<string, string> item in keyValues)
            {
                s = s.Replace(item.Key, item.Value);
            }

            return s;
        }

        /// <summary>
        /// Removes Turkish Chars with .
        /// </summary>
        /// <param name="text">The s <see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string RemoveTurkishChars(this string text)
        {
            if (text.IsNullOrSpace())
                return text;

            string result = text.Replace("ğ", "g")
                .Replace("ı", "i").Replace("ç", "c")
                .Replace("ö", "o").Replace("ü", "u")
                .Replace("Ğ", "G").Replace("Ç", "C")
                .Replace("Ö", "O").Replace("Ü", "U")
                .Replace("İ", "I");

            return result;
        }

        /// <summary>
        /// Masks the string with given parameters. '1234567890', 2,2,'?' ==&gt; '12??????90'
        /// </summary>
        /// <param name="text">The s1.</param>
        /// <param name="leftUnmaskLength">Length of the left unmask.</param>
        /// <param name="rightUnmaskLength">Length of the right unmask.</param>
        /// <param name="maskChar">The mask character.</param>
        /// <returns></returns>
        public static string MaskString(this string text, uint leftUnmaskLength, uint rightUnmaskLength, char maskChar = '*')
        {
            if (string.IsNullOrEmpty(text)) return text;

            StringBuilder builder = new StringBuilder();
            if (text.Length <= (int)(leftUnmaskLength + rightUnmaskLength))
            {
                for (int counter = 0; counter < text.Length; counter++)
                { builder.Append(maskChar); }
            }
            else
            {
                builder.Append(text.Substring(0, (int)leftUnmaskLength));

                int count = text.Length - (int)(leftUnmaskLength + rightUnmaskLength);
                for (int counter = 0; counter < count; counter++)
                { builder.Append(maskChar); }

                builder.Append(text.Substring(text.Length - (int)rightUnmaskLength));
            }

            string maskedText = builder.ToString();
            return maskedText;
        }

        /// <summary>
        /// Copies the string value.
        /// </summary>
        /// <param name="text">The string.</param>
        /// <param name="checkIfNull">the string is null and checkIfNull is false returns null, else if str is null returns empty string.</param>
        /// <returns></returns>
        public static string CopyValue(this string text, bool checkIfNull = false)
        {
            if (text == null && !checkIfNull)
                return null;

            string resultText = (new StringBuilder())
                .Append(text ?? string.Empty)
                .ToString();

            return resultText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CapitalizeFirstLetters(this string text)
        {
            if (text.IsNullOrSpace())
                return text;

            bool isNewSentense = true;
            StringBuilder result = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                if (isNewSentense && char.IsLetter(text[i]))
                {
                    result.Append(char.ToUpper(text[i]));
                    isNewSentense = false;
                }
                else
                    result.Append(char.ToLower(text[i]));

                if (text[i] == ' ')
                {
                    isNewSentense = true;
                }
            }

            string str = result.ToString();
            return str;
        }

        /// <summary>
        /// Removes the apostrophe(').
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="replaceApostropheWithSpace">if true apostrophe(') replaced with " ", else empty string.</param>
        /// <returns>A string.</returns>
        public static string RemoveApostrophe(this string input, bool replaceApostropheWithSpace = false)
        {
            if (input.IsNullOrSpace())
                return input;

            string result = input
                .Replace("'", replaceApostropheWithSpace ? " " : string.Empty)
                ?? string.Empty;
            return result;
        }

        /// <summary>
        /// Removes double quote(").
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="replaceDoubleQuoteWithSpace">if true  double quote(") replaced with " ", else empty string.</param>
        /// <returns>A string.</returns>
        public static string RemoveDoubleQuote(this string input, bool replaceDoubleQuoteWithSpace = false)
        {
            if (input.IsNullOrSpace())
                return input;

            string result = input
                .Replace("\"", replaceDoubleQuoteWithSpace ? " " : string.Empty)
                ?? string.Empty;
            return result;
        }

        /// <summary>
        /// Removes the apostrophe(') and double quote(").
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="replaceApostropheWithSpace">if true apostrophe(') replaced with " ", else empty string.</param>
        /// <param name="replaceDoubleQuoteWithSpace">if true  double quote(") replaced with " ", else empty string.</param>
        /// <returns>A string.</returns>
        public static string RemoveApostropheAndDoubleQuote(this string input, bool replaceApostropheWithSpace = false, bool replaceDoubleQuoteWithSpace = false)
        {
            if (input.IsNullOrSpace())
                return input;

            string result = input
                .Replace("'", replaceApostropheWithSpace ? " " : string.Empty)
                .Replace("\"", replaceDoubleQuoteWithSpace ? " " : string.Empty)
                ?? string.Empty;
            return result;
        }

        /// <summary>
        /// Chunks the specified text.
        /// </summary>
        /// <param name="chunkText">The chunk text.</param>
        /// <param name="chunkSize">Size of the chunk.</param>
        /// <returns>string is null or empty or white space returns empty array, else returns chunked strings</returns>
        public static IEnumerable<string> Chunk(this string chunkText, uint chunkSize = 10)
        {
            if (chunkText.IsNullOrSpace())
                return Enumerable.Empty<string>();

            chunkSize = chunkSize < 1 ? 1 : chunkSize;
            int chunkLen = (int)chunkSize;

            if (chunkText.Length <= chunkLen)
                return new string[] { chunkText };

            var chunkList = new List<string> { chunkText.Substring(0, chunkLen) };
            var tmpChunks = Chunk(chunkText.Substring(chunkLen), chunkSize);
            chunkList.AddRange(tmpChunks);

            return chunkList;
        }
    }
}