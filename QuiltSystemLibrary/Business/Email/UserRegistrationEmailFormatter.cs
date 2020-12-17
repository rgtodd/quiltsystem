//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using RichTodd.QuiltSystem.Properties;

namespace RichTodd.QuiltSystem.Business.Email
{
    public class UserRegistrationEmailFormatter : EmailFormatter
    {
        private readonly string m_link;

        public UserRegistrationEmailFormatter(string link)
        {
            if (string.IsNullOrEmpty(link)) throw new ArgumentNullException(nameof(link));

            m_link = link;
        }

        public override string GetHtml()
        {
            var html = Resources.EmailHtmlTemplate;
            html = html.Replace(TemplateVariables.TemplateBody, Resources.EmailUserRegistrationHtmlTemplate);

            html = html.Replace(TemplateVariables.AdminMailEmail, Constants.AdminMailEmail);
            html = html.Replace(TemplateVariables.Link, m_link);
            html = html.Replace(TemplateVariables.Subject, GetSubject());
            html = html.Replace(TemplateVariables.WebsiteFullUrl, Constants.WebsiteFullUrl);
            html = html.Replace(TemplateVariables.WebsiteName, Constants.WebsiteName);
            html = html.Replace(TemplateVariables.WebsiteUrl, Constants.WebsiteUrl);

            return html;
        }

        public override string GetSubject()
        {
            return Constants.WebsiteName + " Email Confirmation";
        }

        public override string GetText()
        {
            var text = Resources.EmailTextTemplate;
            text = text.Replace(TemplateVariables.TemplateBody, Resources.EmailUserRegistrationTextTemplate);

            text = text.Replace(TemplateVariables.AdminMailEmail, Constants.AdminMailEmail);
            text = text.Replace(TemplateVariables.Link, m_link);
            text = text.Replace(TemplateVariables.Subject, GetSubject());
            text = text.Replace(TemplateVariables.WebsiteFullUrl, Constants.WebsiteFullUrl);
            text = text.Replace(TemplateVariables.WebsiteName, Constants.WebsiteName);
            text = text.Replace(TemplateVariables.WebsiteUrl, Constants.WebsiteUrl);

            return text;
        }
    }
}