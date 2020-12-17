//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Core.Abstractions.Data;

using SendGrid;
using SendGrid.Helpers.Mail;

namespace RichTodd.QuiltSystem.Service.Core.Implementations
{
    public class ApplicationEmailSender : IApplicationEmailSender
    {
        private IOptionsMonitor<ApplicationOptions> ApplicationOptions { get; }
        private ILogger<ApplicationEmailSender> Logger { get; }

        public ApplicationEmailSender(
            IOptionsMonitor<ApplicationOptions> applicationOptions,
            ILogger<ApplicationEmailSender> logger)
        {
            ApplicationOptions = applicationOptions ?? throw new ArgumentNullException(nameof(applicationOptions));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendEmailAsync(ApplicationEmailRequest emailRequest)
        {
            var apiKey = ApplicationOptions.CurrentValue.SendGridApiKey;
            if (!string.IsNullOrEmpty(apiKey))
            {
                var sg = new SendGridClient(apiKey);

                var from = new EmailAddress(emailRequest.SenderEmail, emailRequest.SenderEmailName);
                var subject = emailRequest.Subject;
                var to = new EmailAddress(emailRequest.RecipientEmail, emailRequest.RecipientEmailName);
                var mail = MailHelper.CreateSingleEmail(from, to, subject, emailRequest.BodyText, emailRequest.BodyHtml);

                _ = await sg.SendEmailAsync(mail);
            }
            else
            {
                Logger.LogInformation($"From = {emailRequest.SenderEmailName}({emailRequest.SenderEmailName})");
                Logger.LogInformation($"To = {emailRequest.RecipientEmail}({emailRequest.RecipientEmailName})");
                Logger.LogInformation($"Subject = {emailRequest.Subject}");
                Logger.LogInformation($"Text = {emailRequest.BodyText}");
                Logger.LogInformation($"Html = {emailRequest.BodyHtml}");
            }
        }
    }
}
