//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Microsoft.AspNetCore.Http;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Web.Locale;

namespace RichTodd.QuiltSystem.Service.Core.Implementations
{
    public class ApplicationLocaleWeb : IApplicationLocale
    {
        private readonly IHttpContextAccessor m_httpContext;

        public ApplicationLocaleWeb(
            IHttpContextAccessor httpContext)
        {
            m_httpContext = httpContext;
        }

        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;

        }

        public DateTime GetLocalNow()
        {
            return DateTime.Now;
        }

        public DateTime GetLocalTimeFromUtc(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, GetLocalTimeZoneInfo());
        }

        public DateTime? GetLocalTimeFromUtc(DateTime? dateTime)
        {
            return dateTime.HasValue ? (DateTime?)GetLocalTimeFromUtc(dateTime.Value) : null;
        }

        public DateTime GetUtcFromLocalTime(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, GetLocalTimeZoneInfo());
        }

        public DateTime? GetUtcFromLocalTime(DateTime? dateTime)
        {
            return dateTime.HasValue ? (DateTime?)GetUtcFromLocalTime(dateTime.Value) : null;
        }

        public TimeZoneInfo GetLocalTimeZoneInfo()
        {
            var userLocale = UserLocale.Lookup(m_httpContext.HttpContext);

            return userLocale != null && !string.IsNullOrEmpty(userLocale.TimeZoneId)
                ? TimeZoneInfo.FindSystemTimeZoneById(userLocale.TimeZoneId)
                : TimeZoneInfo.Local;
        }
    }
}
