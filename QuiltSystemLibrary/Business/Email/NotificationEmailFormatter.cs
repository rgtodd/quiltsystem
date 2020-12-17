//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using RichTodd.QuiltSystem.Properties;

namespace RichTodd.QuiltSystem.Business.Email
{
    public class NotificationEmailFormatter : EmailFormatter
    {
        private readonly string m_message;

        public NotificationEmailFormatter(string message)
        {
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

            m_message = message;
        }

        public override string GetHtml()
        {
            var html = Resources.EmailUserRegistrationHtmlTemplate;

            html = html.Replace(TemplateVariables.WebsiteFullUrl, Constants.WebsiteFullUrl);
            html = html.Replace(TemplateVariables.WebsiteUrl, Constants.WebsiteUrl);
            html = html.Replace(TemplateVariables.Link, m_message);
            html = html.Replace(TemplateVariables.AdminMailEmail, Constants.AdminMailEmail);

            return html;
        }

        public override string GetText()
        {
            var text = Resources.EmailUserRegistrationTextTemplate;

            text = text.Replace(TemplateVariables.WebsiteFullUrl, Constants.WebsiteFullUrl);
            text = text.Replace(TemplateVariables.WebsiteUrl, Constants.WebsiteUrl);
            text = text.Replace(TemplateVariables.Link, m_message);
            text = text.Replace(TemplateVariables.AdminMailEmail, Constants.AdminMailEmail);

            return text;
        }

        public override string GetSubject()
        {
            return Constants.WebsiteUrl + " Email Confirmation";
        }
    }
}