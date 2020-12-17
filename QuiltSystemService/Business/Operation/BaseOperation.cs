//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

//using System;

//using RichTodd.QuiltSystem.Business.Email;
//using RichTodd.QuiltSystem.Database.Domain;
//using RichTodd.QuiltSystem.Database.Extensions;
//using RichTodd.QuiltSystem.Database.Model;
//using RichTodd.QuiltSystem.Extensions;
//using RichTodd.QuiltSystem.Service.Core.Abstractions;

//namespace RichTodd.QuiltSystem.Business.Operation
//{
//    public class BaseOperation
//    {
//        

//        protected BaseOperation(IApplicationLogger logger, IApplicationLocale locale)
//        {
//            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
//            Locale = locale ?? throw new ArgumentNullException(nameof(locale));
//        }

//        

//        

//        protected IApplicationLogger Logger { get; }

//        protected IApplicationLocale Locale { get; }

//        

//        

//        protected void BeginFunction(string className, string functionName, params object[] args)
//        {
//            Logger.LogBeginFunction(className, functionName, args);
//        }

//        protected void CreateNotification(QuiltContext ctx, string userId, int notificationTypeCode, Guid? orderId)
//        {
//            var dbNotification = ctx.Notifications.Create();
//            {
//                dbNotification.RecipientAspNetUserId = userId;
//                dbNotification.NotificationTypeCode = notificationTypeCode;
//                dbNotification.OrderId = orderId;
//                dbNotification.CreateDateTimeUtc = Locale.GetUtcNow();
//            }
//            ctx.Notifications.Add(dbNotification);

//            OnNotificationCreated(ctx, dbNotification);
//        }

//        protected void CreateOperationExceptionAlert(QuiltContext ctx, Exception ex, Guid? orderId = null, int? accountReceiptId = null, int? accountPaymentId = null, int? orderShipmentRequestId = null, int? orderShipmentId = null, int? orderReturnRequestId = null, int? orderReturnId = null, int? emailRequestId = null)
//        {
//            string description = "Operation Exception - " + GetType().Name;

//            var dbAlert = ctx.Alerts.Create();
//            {
//                dbAlert.AlertTypeCode = AlertTypes.OperationException;
//                dbAlert.CreateDateTimeUtc = Locale.GetUtcNow();
//                dbAlert.Description = description;
//                dbAlert.Exception = ex.GetDetail();
//                dbAlert.OrderId = orderId;
//                dbAlert.AccountReceiptId = accountReceiptId;
//                dbAlert.AccountPaymentId = accountPaymentId;
//                dbAlert.OrderShipmentRequestId = orderShipmentRequestId;
//                dbAlert.OrderShipmentId = orderShipmentId;
//                dbAlert.OrderReturnRequestId = orderReturnRequestId;
//                dbAlert.OrderReturnId = orderReturnId;
//                dbAlert.EmailRequestId = emailRequestId;
//            }
//            ctx.Alerts.Add(dbAlert);

//            OnAlertCreated(ctx, dbAlert);
//        }

//        protected void EndFunction()
//        {
//            Logger.LogEndFunction();
//        }

//        protected void LogException(Exception ex)
//        {
//            Logger.log.LogException(ex);
//        }

//        protected void log.LogResult(object result)
//        {
//            Logger.log.LogResult(result);
//        }

//        protected void OnAlertCreated(QuiltContext ctx, Alert dbAlert)
//        {
//            // Do not generate an email request if the alert involves an email request.
//            //
//            if (dbAlert.EmailRequestId != null)
//            {
//                return;
//            }

//            var formatter = new AlertEmailFormatter(dbAlert.AlertType.Name);

//            var dbEmailRequest = ctx.EmailRequests.Create();
//            {
//                dbEmailRequest.EmailRequestStatusType = ctx.EmailRequestStatusType(EmailRequestStatusTypes.Posted);
//                dbEmailRequest.SenderEmail = Constants.DoNotReplyEmail;
//                dbEmailRequest.SenderEmailName = Constants.DoNotReplyEmailName;
//                dbEmailRequest.RecipientEmail = Constants.AdminMailEmail;
//                dbEmailRequest.RecipientEmailName = Constants.AdminMailEmailName;
//                dbEmailRequest.RecipientAspNetUserId = null;
//                dbEmailRequest.Subject = formatter.GetSubject();
//                dbEmailRequest.BodyText = formatter.GetText();
//                dbEmailRequest.BodyHtml = formatter.GetHtml();
//                dbEmailRequest.BodyType = EmailBodyTypes.Alert;
//                dbEmailRequest.CreateDateTimeUtc = Locale.GetUtcNow();
//                dbEmailRequest.StatusDateTimeUtc = Locale.GetUtcNow();
//            }
//            ctx.EmailRequests.Add(dbEmailRequest);

//            var dbAlertEmailRequest = ctx.AlertEmailRequests.Create();
//            {
//                dbAlertEmailRequest.Alert = dbAlert;
//                dbAlertEmailRequest.EmailRequest = dbEmailRequest;
//            }
//            ctx.AlertEmailRequests.Add(dbAlertEmailRequest);
//        }

//        protected void OnNotificationCreated(QuiltContext ctx, Notification dbNotification)
//        {
//            var body = dbNotification.NotificationType.Body;
//            if (dbNotification.OrderId.HasValue)
//            {
//                var url = "https://quiltagogo.azurewebsites.net/Order/Index/" + dbNotification.OrderId.Value.ToString();
//                body = body.Replace("%ORDER%", url);
//            }

//            var formatter = new NotificationEmailFormatter(dbNotification.NotificationType.Subject);

//            var dbEmailRequest = ctx.EmailRequests.Create();
//            {
//                dbEmailRequest.EmailRequestStatusType = ctx.EmailRequestStatusType(EmailRequestStatusTypes.Posted);
//                dbEmailRequest.SenderEmail = Constants.DoNotReplyEmail;
//                dbEmailRequest.SenderEmailName = Constants.DoNotReplyEmailName;
//                dbEmailRequest.RecipientEmail = dbNotification.RecipientAspNetUser.Email;
//                dbEmailRequest.RecipientEmailName = dbNotification.RecipientAspNetUser.EmailName;
//                dbEmailRequest.RecipientAspNetUserId = dbNotification.RecipientAspNetUserId;
//                dbEmailRequest.Subject = formatter.GetSubject();
//                dbEmailRequest.BodyText = formatter.GetText();
//                dbEmailRequest.BodyHtml = formatter.GetHtml();
//                dbEmailRequest.BodyType = EmailBodyTypes.Notification;
//                dbEmailRequest.CreateDateTimeUtc = Locale.GetUtcNow();
//                dbEmailRequest.StatusDateTimeUtc = Locale.GetUtcNow();
//            }
//            ctx.EmailRequests.Add(dbEmailRequest);

//            var dbNotificationEmailRequest = ctx.NotificationEmailRequests.Create();
//            {
//                dbNotificationEmailRequest.Notification = dbNotification;
//                dbNotificationEmailRequest.EmailRequest = dbEmailRequest;
//            }
//            ctx.NotificationEmailRequests.Add(dbNotificationEmailRequest);
//        }

//        

//        

//        private void CreateOrderReceiptAlert(QuiltContext ctx, Guid orderId)
//        {
//            string description = string.Format("Order Receipt - {0}.", orderId);

//            var dbAlert = ctx.Alerts.Create();
//            {
//                dbAlert.AlertTypeCode = AlertTypes.OrderReceipt;
//                dbAlert.CreateDateTimeUtc = Locale.GetUtcNow();
//                dbAlert.Description = description;
//                dbAlert.OrderId = orderId;
//            }
//            ctx.Alerts.Add(dbAlert);

//            OnAlertCreated(ctx, dbAlert);
//        }

//        private void CreateOrderReceiptFailureAlert(QuiltContext ctx, Guid orderId)
//        {
//            string description = string.Format("Order Receipt Failure - {0}.", orderId);

//            var dbAlert = ctx.Alerts.Create();
//            {
//                dbAlert.AlertTypeCode = AlertTypes.OrderReceiptFailure;
//                dbAlert.CreateDateTimeUtc = Locale.GetUtcNow();
//                dbAlert.Description = description;
//                dbAlert.OrderId = orderId;
//            }
//            ctx.Alerts.Add(dbAlert);

//            OnAlertCreated(ctx, dbAlert);
//        }

//        
//    }
//}