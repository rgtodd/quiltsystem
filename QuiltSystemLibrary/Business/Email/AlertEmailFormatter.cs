//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using RichTodd.QuiltSystem.Properties;

namespace RichTodd.QuiltSystem.Business.Email
{
    public class AlertEmailFormatter : EmailFormatter
    {
        private readonly string m_message;

        public AlertEmailFormatter(string message)
        {
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

            m_message = message;
        }

        public override string GetHtml()
        {
            var html = Resources.EmailUserRegistrationHtmlTemplate;

            html = html.Replace("$URL-ROOT$", Constants.WebsiteFullUrl);
            html = html.Replace("$URL-NAME$", Constants.WebsiteUrl);
            html = html.Replace("$LINK$", m_message);
            html = html.Replace("$SUPPORT-EMAIL$", Constants.AdminMailEmail);

            return html;
        }

        public override string GetText()
        {
            var text = Resources.EmailUserRegistrationTextTemplate;

            text = text.Replace("$URL-ROOT$", Constants.WebsiteFullUrl);
            text = text.Replace("$URL-NAME$", Constants.WebsiteUrl);
            text = text.Replace("$LINK$", m_message);
            text = text.Replace("$SUPPORT-EMAIL$", Constants.AdminMailEmail);

            return text;
        }

        public override string GetSubject()
        {
            return Constants.WebsiteUrl + " Email Confirmation";
        }
    }
}