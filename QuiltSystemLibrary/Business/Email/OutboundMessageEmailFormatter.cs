//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using RichTodd.QuiltSystem.Properties;

namespace RichTodd.QuiltSystem.Business.Email
{
    public class OutboundMessageEmailFormatter : EmailFormatter
    {
        private readonly string m_messageSubject;
        private readonly string m_messageText;
        private readonly long? m_orderId;
        private readonly string m_orderNumber;

        public OutboundMessageEmailFormatter(string messageSubject, string messageText, long? orderId, string orderNumber)
        {
            if (string.IsNullOrEmpty(messageSubject)) throw new ArgumentNullException(nameof(messageSubject));
            if (string.IsNullOrEmpty(messageText)) throw new ArgumentNullException(nameof(messageText));

            m_messageSubject = messageSubject;
            m_messageText = messageText;
            m_orderId = orderId;
            m_orderNumber = orderNumber;
        }

        public override string GetHtml()
        {
            var html = Resources.EmailHtmlTemplate;
            html = html.Replace(TemplateVariables.TemplateBody, Resources.EmailOutboundMessageHtmlTemplate);
            if (m_orderId.HasValue)
            {
                html = html.Replace(TemplateVariables.OrderComponent, Resources.EmailOutboundMessageOrderHtmlComponent);
            }
            else
            {
                html = html.Replace(TemplateVariables.OrderComponent, string.Empty);
            }

            html = ReplaceVariables(html);

            return html;
        }

        public override string GetSubject()
        {
            return Constants.WebsiteName + " Message - " + m_messageSubject;
        }

        public override string GetText()
        {
            var text = Resources.EmailTextTemplate;
            text = text.Replace(TemplateVariables.TemplateBody, Resources.EmailOutboundMessageTextTemplate);
            if (m_orderId.HasValue)
            {
                text = text.Replace(TemplateVariables.OrderComponent, Resources.EmailOutboundMessageOrderTextComponent);
            }
            else
            {
                text = text.Replace(TemplateVariables.OrderComponent, string.Empty);
            }

            text = ReplaceVariables(text);

            return text;
        }

        private string ReplaceVariables(string html)
        {
            html = html.Replace(TemplateVariables.AdminWebsiteFullUrl, Constants.AdminWebsiteFullUrl);
            html = html.Replace(TemplateVariables.AdminMailEmail, Constants.AdminMailEmail);
            html = html.Replace(TemplateVariables.MessageSubject, m_messageSubject);
            html = html.Replace(TemplateVariables.MessageText, m_messageText);
            html = html.Replace(TemplateVariables.Subject, m_messageSubject);
            html = html.Replace(TemplateVariables.WebsiteFullUrl, Constants.WebsiteFullUrl);
            html = html.Replace(TemplateVariables.WebsiteName, Constants.WebsiteName);
            html = html.Replace(TemplateVariables.WebsiteUrl, Constants.WebsiteUrl);
            if (m_orderId.HasValue)
            {
                html = html.Replace(TemplateVariables.OrderId, m_orderId.ToString());
                html = html.Replace(TemplateVariables.OrderId, m_orderNumber);
            }

            return html;
        }
    }
}