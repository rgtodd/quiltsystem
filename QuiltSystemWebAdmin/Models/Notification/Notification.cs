//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Notification
{
    public class Notification
    {
        public ANotification_Notification ANotification { get; }
        public IApplicationLocale Locale { get; }

        public Notification(
            ANotification_Notification aNotification,
            IApplicationLocale locale)
        {
            ANotification = aNotification;
            Locale = locale;
        }

        public MCommunication_Notification MNotification => ANotification.MNotification;
        public MUser_User MUser => ANotification.MUser;

        [Display(Name = "Notification ID")]
        public long NotificationId => MNotification.NotificationId;

        [Display(Name = "Notification Type")]
        public string NotificationType => MNotification.NotificationType.ToString();

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        [Display(Name = "Notification Date/Time")]
        public DateTime CreatedDateTime => Locale.GetLocalTimeFromUtc(MNotification.CreateDateTimeUtc);

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        [Display(Name = "Acknowledgement Date/Time")]
        public DateTime? AcknowledgementDateTime => Locale.GetLocalTimeFromUtc(MNotification.AcknowledgementDateTimeUtc);

        [Display(Name = "User ID")]
        public string UserId => MUser?.UserId;

        [Display(Name = "User Email")]
        public string UserEmail => MUser?.Email;

        [Display(Name = "Topic ID")]
        public long? TopicId => MNotification.TopicId;

        [Display(Name = "Topic Reference")]
        public string TopicReference => MNotification.TopicReference;

        private ReferenceValues m_topicReferenceValues;
        public ReferenceValues TopicReferenceValues
        {
            get
            {
                if (m_topicReferenceValues == null)
                {
                    m_topicReferenceValues = TopicReference != null
                        ? ParseReferenceValues.From(TopicReference)
                        : new ReferenceValues();
                }

                return m_topicReferenceValues;
            }
        }

        [Display(Name = "Order ID")]
        public long? OrderId => TopicReferenceValues.OrderId;

        [Display(Name = "Return ID")]
        public long? ReturnId => TopicReferenceValues.ReturnId;

        [Display(Name = "Return Request ID")]
        public long? ReturnRequestId => TopicReferenceValues.ReturnRequestId;

        [Display(Name = "Shipment ID")]
        public long? ShipmentId => TopicReferenceValues.ShipmentId;

        [Display(Name = "Shipment Request ID")]
        public long? ShipmentRequestId => TopicReferenceValues.ShipmentRequestId;
    }
}