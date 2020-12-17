//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Globalization;

namespace RichTodd.QuiltSystem.Extensions
{
    public static class DecimalExtensions
    {
        private static readonly CultureInfo s_currencyCulture = new CultureInfo("en-US");

        public static string AsFormattedDollars(this decimal value)
        {
            return string.Format(s_currencyCulture, "{0:C}", value);
        }
    }
}