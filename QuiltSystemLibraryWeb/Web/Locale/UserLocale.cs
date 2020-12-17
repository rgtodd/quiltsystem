//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Microsoft.AspNetCore.Http;

namespace RichTodd.QuiltSystem.Web.Locale
{
    public class UserLocale
    {
        private const string s_cookieName = "UserLocale";
        private string m_timeZoneId;

        public UserLocale(string timeZoneId)
        {
            if (string.IsNullOrEmpty(timeZoneId)) throw new ArgumentNullException(nameof(timeZoneId));

            m_timeZoneId = timeZoneId;
        }

        public static string CookieName
        {
            get { return s_cookieName; }
        }

        public static object Key { get; } = new object();

        public string TimeZoneId
        {
            get
            {
                return m_timeZoneId;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                m_timeZoneId = value;
            }
        }

        public static UserLocale Lookup(HttpContext httpContext)
        {
            var userLocale = (UserLocale)httpContext.Items[Key];

            return userLocale;
        }

        public static UserLocale Parse(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            var userLocal = new UserLocale(value);

            return userLocal;
        }

        public static void RemoveFrom(HttpContext httpContextBase)
        {
            _ = httpContextBase.Items.Remove(Key);
        }

        public void AddTo(HttpContext httpContextBase)
        {
            httpContextBase.Items[Key] = this;
        }

        public override string ToString()
        {
            return m_timeZoneId;
        }
    }
}