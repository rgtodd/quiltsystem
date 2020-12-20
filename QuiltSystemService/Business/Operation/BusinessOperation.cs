//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Business.Email;
using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Database.Model.Extensions;
using RichTodd.QuiltSystem.Extensions;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;

namespace RichTodd.QuiltSystem.Business.Operation
{
    public class BusinessOperation
    {

        public BusinessOperation(
             ILogger applicationLogger,
             IApplicationLocale applicationLocale,
             IQuiltContextFactory quiltContextFactory,
             ICommunicationMicroService communicationMicroService)
        {
            Logger = applicationLogger ?? throw new ArgumentNullException(nameof(applicationLogger));
            Locale = applicationLocale ?? throw new ArgumentNullException(nameof(applicationLocale));
            QuiltContextFactory = quiltContextFactory ?? throw new ArgumentNullException(nameof(quiltContextFactory));
            CommunicationMicroService = communicationMicroService ?? throw new ArgumentNullException(nameof(communicationMicroService));
        }

        protected ILogger Logger { get; }

        protected IApplicationLocale Locale { get; }

        protected IQuiltContextFactory QuiltContextFactory { get; }

        protected ICommunicationMicroService CommunicationMicroService { get; }

        protected IFunctionContext BeginFunction(string className, string functionName, params object[] args)
        {
            return Function.BeginFunction(Logger, className, functionName, args);
        }
        //protected void BeginFunction(string className, string functionName, params object[] args)
        //{
        //    Logger.LogBeginFunction(className, functionName, args);
        //}

        //protected async Task CreateNotificationAsync(QuiltContext ctx, string userId, string notificationTypeCode, long? orderId)
        //{
        //    long? topicId;
        //    if (orderId.HasValue)
        //    {
        //        var topicReference = CreateTopicReference.FromOrderId(orderId.Value);
        //        topicId = await CommunicationMicroService.AllocateTopicAsync(topicReference, null);
        //    }
        //    else
        //    {
        //        topicId = null;
        //    }

        //    var participantReference = CreateParticipantReference.FromUserId(userId);
        //    var participantId = await CommunicationMicroService.AllocateParticipantAsync(participantReference).ConfigureAwait(false);

        //    var dbNotification = new Notification()
        //    {
        //        ParticipantId = participantId,
        //        NotificationTypeCode = notificationTypeCode,
        //        TopicId = topicId,
        //        CreateDateTimeUtc = Locale.GetUtcNow()
        //    };
        //    _ = ctx.Notifications.Add(dbNotification);

        //    OnNotificationCreated(ctx, dbNotification);
        //}

        protected void CreateOperationExceptionAlert(QuiltContext ctx, Exception ex, long? topicId = null, long? emailRequestId = null)
        {
            var description = "Operation Exception - " + GetType().Name;

            var dbAlert = new Alert()
            {
                AlertTypeCode = AlertTypeCodes.OperationException,
                CreateDateTimeUtc = Locale.GetUtcNow(),
                Description = description,
                Exception = ex.GetDetail(),
                TopicId = topicId,
                EmailRequestId = emailRequestId
            };
            _ = ctx.Alerts.Add(dbAlert);

            OnAlertCreated(ctx, dbAlert);
        }

        //protected void EndFunction()
        //{
        //    Logger.LogEndFunction();
        //}

        //protected void LogException(Exception ex)
        //{
        //    Logger.LogException(ex);
        //}

        //protected void LogResult(object result)
        //{
        //    Logger.LogResult(result);
        //}

        protected void OnAlertCreated(QuiltContext ctx, Alert dbAlert)
        {
            // Do not generate an email request if the alert involves an email request.
            //
            if (dbAlert.EmailRequestId != null)
            {
                return;
            }

            var dbAlertType = ctx.AlertTypes.Find(dbAlert.AlertTypeCode);

            var formatter = new AlertEmailFormatter(dbAlertType.Name);

            var dbEmailRequest = new EmailRequest()
            {
                EmailRequestStatusCode = EmailRequestStatusCodes.Posted,
                SenderEmail = Constants.DoNotReplyEmail,
                SenderEmailName = Constants.DoNotReplyEmailName,
                RecipientEmail = Constants.AdminMailEmail,
                RecipientEmailName = Constants.AdminMailEmailName,
                RecipientParticipantId = null,
                Subject = formatter.GetSubject(),
                BodyText = formatter.GetText(),
                BodyHtml = formatter.GetHtml(),
                BodyTypeCode = EmailBodyTypes.Alert,
                CreateDateTimeUtc = Locale.GetUtcNow(),
                EmailRequestStatusDateTimeUtc = Locale.GetUtcNow(),
            };
            _ = ctx.EmailRequests.Add(dbEmailRequest);

            var dbAlertEmailRequest = new AlertEmailRequest()
            {
                Alert = dbAlert,
                EmailRequest = dbEmailRequest,
            };
            _ = ctx.AlertEmailRequests.Add(dbAlertEmailRequest);
        }

        protected void OnNotificationCreated(QuiltContext ctx, Notification dbNotification)
        {
            var dbNotificationType = ctx.NotificationTypes.Find(dbNotification.NotificationTypeCode);

            var formatter = new NotificationEmailFormatter(dbNotificationType.Subject);

            var dbParticipant = ctx.Participants.Where(r => r.ParticipantId == dbNotification.ParticipantId).Single();
            var participantUserId = ParseUserId.FromParticipantReference(dbParticipant.ParticipantReference);
            var dbAspNetUser = ctx.AspNetUsers.Where(r => r.Id == participantUserId).Single();

            var dbEmailRequest = new EmailRequest()
            {
                EmailRequestStatusCode = EmailRequestStatusCodes.Posted,
                SenderEmail = Constants.DoNotReplyEmail,
                SenderEmailName = Constants.DoNotReplyEmailName,
                RecipientEmail = dbAspNetUser.Email,
                RecipientEmailName = dbAspNetUser.EmailName(),
                RecipientParticipant = dbNotification.Participant,
                Subject = formatter.GetSubject(),
                BodyText = formatter.GetText(),
                BodyHtml = formatter.GetHtml(),
                BodyTypeCode = EmailBodyTypes.Notification,
                CreateDateTimeUtc = Locale.GetUtcNow(),
                EmailRequestStatusDateTimeUtc = Locale.GetUtcNow(),
            };
            _ = ctx.EmailRequests.Add(dbEmailRequest);

            var dbNotificationEmailRequest = new NotificationEmailRequest()
            {
                Notification = dbNotification,
                EmailRequest = dbEmailRequest
            };
            _ = ctx.NotificationEmailRequests.Add(dbNotificationEmailRequest);
        }

        //private void CreateOrderReceiptAlert(QuiltContext ctx, Guid orderId)
        //{
        //    string description = string.Format("Order Receipt - {0}.", orderId);

        //    var dbAlert = ctx.Alerts.Create();
        //    {
        //        dbAlert.AlertTypeCode = AlertTypes.OrderReceipt;
        //        dbAlert.CreateDateTimeUtc = Locale.GetUtcNow();
        //        dbAlert.Description = description;
        //        dbAlert.OrderId = orderId;
        //    }
        //    ctx.Alerts.Add(dbAlert);

        //    OnAlertCreated(ctx, dbAlert);
        //}

        //private void CreateOrderReceiptFailureAlert(QuiltContext ctx, Guid orderId)
        //{
        //    string description = string.Format("Order Receipt Failure - {0}.", orderId);

        //    var dbAlert = ctx.Alerts.Create();
        //    {
        //        dbAlert.AlertTypeCode = AlertTypes.OrderReceiptFailure;
        //        dbAlert.CreateDateTimeUtc = Locale.GetUtcNow();
        //        dbAlert.Description = description;
        //        dbAlert.OrderId = orderId;
        //    }
        //    ctx.Alerts.Add(dbAlert);

        //    OnAlertCreated(ctx, dbAlert);
        //}

    }
}