//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.Text;

namespace RichTodd.QuiltSystem.Web.Bootstrap
{
    public static class BootstrapUtility
    {
        public static IReadOnlyList<string> FormatAddress(IBootstrapAddress address)
        {
            return address != null
                ? FormatAddress(
                    address.Name,
                    address.AddressLine1,
                    address.AddressLine2,
                    address.City,
                    address.StateCode,
                    address.PostalCode,
                    address.CountryCode)
                : new List<string>();
        }

        public static IReadOnlyList<string> FormatAddress(string name, string addressLine1, string addressLine2, string city, string stateCode, string postalCode, string countryCode)
        {
            var result = new List<string>();

            if (!string.IsNullOrEmpty(name))
            {
                result.Add(name);
            }

            if (!string.IsNullOrEmpty(addressLine1))
            {
                result.Add(addressLine1);
            }

            if (!string.IsNullOrEmpty(addressLine2))
            {
                result.Add(addressLine2);
            }

            var addressLine3 = FormatAddress(city, stateCode, postalCode, countryCode);
            if (!string.IsNullOrEmpty(addressLine3))
            {
                result.Add(addressLine3);
            }

            return result;
        }

        public static string FormatAddress(string city, string stateCode, string postalCode, string countryCode)
        {
            var sb = new StringBuilder();

            var dt = DelimiterTypes.None;

            if (!string.IsNullOrEmpty(city))
            {
                _ = sb.Append(GetDelimiterText(dt, DelimiterTypes.Text));
                dt = DelimiterTypes.Text;

                _ = sb.Append(city);
            }

            if (!string.IsNullOrEmpty(stateCode))
            {
                _ = sb.Append(GetDelimiterText(dt, DelimiterTypes.Text));
                dt = DelimiterTypes.Text;

                _ = sb.Append(stateCode);
            }

            if (!string.IsNullOrEmpty(postalCode))
            {
                _ = sb.Append(GetDelimiterText(dt, DelimiterTypes.Numeric));
                dt = DelimiterTypes.Numeric;

                _ = sb.Append(postalCode);
            }

            if (!string.IsNullOrEmpty(countryCode))
            {
                _ = sb.Append(GetDelimiterText(dt, DelimiterTypes.Text));
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                dt = DelimiterTypes.Text;
#pragma warning restore IDE0059 // Unnecessary assignment of a value

                _ = sb.Append(countryCode);
            }

            return sb.ToString();
        }

        private static string GetDelimiterText(DelimiterTypes from, DelimiterTypes to)
        {
            return from switch
            {
                DelimiterTypes.None => string.Empty,
                DelimiterTypes.Numeric => to == DelimiterTypes.Numeric ? ", " : " ",
                DelimiterTypes.Text => to == DelimiterTypes.Text ? ", " : " ",
                _ => null,
            };
        }

        private enum DelimiterTypes
        {
            None,
            Text,
            Numeric
        }
    }
}
