//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Web
{
    public abstract class ApplicationModelFactory
    {
        private static readonly Regex s_postalCodeRegEx = new Regex(@"^\d{5}((-\d{4})|(\d{4}))?$");

        protected IApplicationModelFactoryContext Context { get; set; }

        public static TApplicationModelFactory Create<TApplicationModelFactory>(IApplicationModelFactoryContext context) where TApplicationModelFactory : ApplicationModelFactory, new()
        {
            var factory = new TApplicationModelFactory()
            { };

            factory.Context = context;

            return factory;
        }

        public virtual int DefaultRecordCount
        {
            get
            {
                return 100;
            }
        }

        protected IList<SelectListItem> CreateRecordCountList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem() { Text = "100", Value = "100" },
                new SelectListItem() { Text = "1000", Value = "1000" },
                new SelectListItem() { Text = "10000", Value = "10000" },
            };
        }

        protected const string NullValue = "*NULL";

        protected IApplicationLocale Locale
        {
            get
            {
                return Context.Locale;
            }
        }

        protected HttpContext HttpContext
        {
            get
            {
                return Context.HttpContext;
            }
        }

        protected bool IsValidPostalCode(string value)
        {
            value = value.Trim();

            return string.IsNullOrEmpty(value) || s_postalCodeRegEx.IsMatch(value);
        }

        protected string FormatOptional(string value)
        {
            return !string.IsNullOrEmpty(value) ? value : "(Not Set)";
        }

        protected IReadOnlyList<string> FormatOptional(IReadOnlyList<string> value)
        {
            return value.Count > 0
                ? value
                : new List<string>(new string[] { "(Not Set)" });
        }

        protected string FormatName(string firstName, string lastName)
        {
            var sb = new StringBuilder();

            string prefix = "";

            if (!string.IsNullOrEmpty(firstName))
            {
                _ = sb.Append(prefix); prefix = " ";
                _ = sb.Append(firstName);
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                _ = sb.Append(prefix); //prefix = " ";
                _ = sb.Append(lastName);
            }

            return sb.ToString();
        }

        protected IReadOnlyList<string> FormatAddress(MCommon_Address mAddress)
        {
            return FormatAddress(
                mAddress.AddressLine1,
                mAddress.AddressLine2,
                mAddress.City,
                mAddress.StateCode,
                mAddress.PostalCode,
                mAddress.CountryCode);
        }

        protected IReadOnlyList<string> FormatAddress(string addressLine1, string addressLine2, string city, string stateCode, string postalCode, string countryCode)
        {
            var result = new List<string>();

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

        protected string FormatAddress(string city, string stateCode, string postalCode, string countryCode)
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

        protected string GetDelimiterText(DelimiterTypes from, DelimiterTypes to)
        {
            return from switch
            {
                DelimiterTypes.None => string.Empty,
                DelimiterTypes.Numeric => to == DelimiterTypes.Numeric ? ", " : " ",
                DelimiterTypes.Text => to == DelimiterTypes.Text ? ", " : " ",
                _ => null,
            };
        }

        protected IList<SelectListItem> CreateNullableBooleanList()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem() { Text = "Yes", Value = "true" },
                    new SelectListItem() { Text = "No", Value = "false" },
                    new SelectListItem() { Text = "Any", Value = NullValue }
                };
        }

        protected bool? ParseNullableBoolean(string value)
        {
            return !string.IsNullOrEmpty(value) && value != NullValue
                ? (bool?)bool.Parse(value)
                : null;
        }

        protected string ToString(bool? value)
        {
            return value != null
                ? value.Value.ToString()
                : NullValue;
        }

        public enum DelimiterTypes
        {
            None,
            Text,
            Numeric
        }
    }
}