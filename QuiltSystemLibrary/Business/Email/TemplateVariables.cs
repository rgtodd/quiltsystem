//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Business.Email
{
    public static class TemplateVariables
    {
        public static string AdminMailEmail = "$SUPPORT-EMAIL$";
        public static string AdminWebsiteFullUrl = "$ADMIN-FULL-URL$";
        public static string AdminWebsiteUrl = "$ADMIN-URL$";
        public static string DeferredAlertId = "%ALERT-ID%";
        public static string DeferredMessageId = "%MESSAGE-ID%";
        public static string DeferredNotificationId = "%NOTIFICATION-ID%";
        public static string Link = "$LINK$";
        public static string MessageSubject = "$MESSAGE-SUBJECT$";
        public static string MessageText = "$MESSAGE-TEXT$";
        public static string OrderComponent = "$ORDER-COMPONENT$";
        public static string OrderId = "$ORDER-ID$";
        public static string OrderNumber = "$ORDER-NUMBER$";
        public static string SenderEmail = "$SENDER-EMAIL$";
        public static string SenderName = "$SENDER-NAME$";
        public static string Subject = "$SUBJECT$";
        public static string TemplateBody = "$TEMPLATE-BODY$";
        public static string WebsiteFullUrl = "$FULL-URL$";
        public static string WebsiteName = "$WEBSITE-NAME$";
        public static string WebsiteUrl = "$URL$";
    }
}