//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Text;

namespace RichTodd.QuiltSystem
{
    public static class TextUtility
    {
        public const char MultilineDelimiter = '|';
        public static readonly string[] EmptyStringArray = new string[0];

        // TODO: Handle values containing the delimiter.
        //
        public static string EncodeMultilineText(params string[] values)
        {
            var sb = new StringBuilder();
            for (int idx = 0; idx < values.Length; ++idx)
            {
                if (idx > 0)
                {
                    _ = sb.Append(MultilineDelimiter);
                }
                _ = sb.Append(values[idx]);
            }

            return sb.ToString();
        }

        public static string[] DecodeMultilineText(string value)
        {
            return !string.IsNullOrEmpty(value)
                ? value.Split(MultilineDelimiter)
                : EmptyStringArray;
        }
    }
}
