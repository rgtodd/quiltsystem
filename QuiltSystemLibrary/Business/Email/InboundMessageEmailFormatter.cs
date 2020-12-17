//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using RichTodd.QuiltSystem.Properties;

namespace RichTodd.QuiltSystem.Business.Email
{
    public class InboundMessageEmailFormatter : EmailFormatter
    {
        private static readonly IDictionary<string, string> EMPTY_TOPIC_FIELD_DICTIONARY = new Dictionary<string, string>();

        private readonly string m_messageSubject;
        private readonly string m_messageText;
        private readonly IDictionary<string, string> m_topicFields;
        private readonly string m_senderEmail;
        private readonly string m_senderName;

        public InboundMessageEmailFormatter(string senderName, string senderEmail, string messageSubject, string messageText, IDictionary<string, string> topicFields)
        {
            if (string.IsNullOrEmpty(senderName)) throw new ArgumentNullException(nameof(senderName));
            if (string.IsNullOrEmpty(senderEmail)) throw new ArgumentNullException(nameof(senderEmail));
            if (string.IsNullOrEmpty(messageSubject)) throw new ArgumentNullException(nameof(messageSubject));
            if (string.IsNullOrEmpty(messageText)) throw new ArgumentNullException(nameof(messageText));

            m_senderName = senderName;
            m_senderEmail = senderEmail;
            m_messageSubject = messageSubject;
            m_messageText = messageText;
            m_topicFields = topicFields ?? EMPTY_TOPIC_FIELD_DICTIONARY;
        }

        public override string GetHtml()
        {
            var html = Resources.EmailHtmlTemplate;

            html = html.Replace(TemplateVariables.TemplateBody, Resources.EmailInboundMessageHtmlTemplate);

            html = m_topicFields.ContainsKey("OrderId")
                ? html.Replace(TemplateVariables.OrderComponent, Resources.EmailInboundMessageOrderHtmlComponent)
                : html.Replace(TemplateVariables.OrderComponent, string.Empty);

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
            text = text.Replace(TemplateVariables.TemplateBody, Resources.EmailInboundMessageTextTemplate);

            text = m_topicFields.ContainsKey("OrderId")
                ? text.Replace(TemplateVariables.OrderComponent, Resources.EmailInboundMessageOrderTextComponent)
                : text.Replace(TemplateVariables.OrderComponent, string.Empty);

            text = ReplaceVariables(text);

            return text;
        }

        private string ReplaceVariables(string html)
        {
            html = html.Replace(TemplateVariables.AdminWebsiteFullUrl, Constants.AdminWebsiteFullUrl);
            html = html.Replace(TemplateVariables.AdminMailEmail, Constants.AdminMailEmail);
            html = html.Replace(TemplateVariables.MessageSubject, m_messageSubject);
            html = html.Replace(TemplateVariables.MessageText, m_messageText);
            html = html.Replace(TemplateVariables.SenderEmail, m_senderEmail);
            html = html.Replace(TemplateVariables.SenderName, m_senderName);
            html = html.Replace(TemplateVariables.Subject, m_messageSubject);
            html = html.Replace(TemplateVariables.WebsiteFullUrl, Constants.WebsiteFullUrl);
            html = html.Replace(TemplateVariables.WebsiteName, Constants.WebsiteName);
            html = html.Replace(TemplateVariables.WebsiteUrl, Constants.WebsiteUrl);

            var orderId = m_topicFields.ContainsKey("OrderId") ? m_topicFields["OrderId"] : string.Empty;
            var orderNumber = m_topicFields.ContainsKey("OrderNumber") ? m_topicFields["OrderNumber"] : string.Empty;

            html = html.Replace(TemplateVariables.OrderId, orderId);
            html = html.Replace(TemplateVariables.OrderNumber, orderNumber);

            return html;
        }
    }
}