//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.AspNetCore.Http;

namespace RichTodd.QuiltSystem.Web.View
{
    public class ViewOptions
    {
        private static readonly object s_key = new object();

        private readonly bool m_ecommerceEnabled;
        private readonly bool m_publicRegistrationEnabled;

        public ViewOptions(bool ecommerceEnabled, bool publicRegistrationEnabled)
        {
            m_ecommerceEnabled = ecommerceEnabled;
            m_publicRegistrationEnabled = publicRegistrationEnabled;
        }

        public static object Key
        {
            get { return s_key; }
        }

        public bool EcommerceEnabled
        {
            get
            {
                return m_ecommerceEnabled;
            }
        }

        public bool PublicRegistrationEnabled
        {
            get
            {
                return m_publicRegistrationEnabled;
            }
        }

        public static ViewOptions Lookup(HttpContext httpContext)
        {
            var userLocale = (ViewOptions)httpContext.Items[Key];

            return userLocale;
        }

        public void AddTo(HttpContext httpContext)
        {
            httpContext.Items[Key] = this;
        }
    }
}