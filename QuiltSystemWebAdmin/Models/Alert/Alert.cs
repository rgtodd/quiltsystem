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

namespace RichTodd.QuiltSystem.WebAdmin.Models.Alert
{
    public class Alert
    {
        public AAlert_Alert AAlert { get; set; }
        public IApplicationLocale Locale { get; set; }

        public Alert(AAlert_Alert aAlert, IApplicationLocale locale)
        {
            AAlert = aAlert;
            Locale = locale;
        }

        public MCommunication_Alert MAlert => AAlert.MAlert;
        public MUser_User MUser => AAlert.MUser;

        [Display(Name = "Alert ID")]
        public long AlertId => MAlert.AlertId;

        [Display(Name = "Alert Type")]
        public string AlertType => MAlert.AlertType.ToString();

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        [Display(Name = "Alert Date/Time")]
        public DateTime AlertDateTime => Locale.GetLocalTimeFromUtc(MAlert.CreateDateTimeUtc);

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        [Display(Name = "Acknowledgement Date/Time")]
        public DateTime? CompletedDateTime => Locale.GetLocalTimeFromUtc(MAlert.AcknowledgementDateTimeUtc);

        [Display(Name = "Description")]
        public string Description => MAlert.Description;

        [Display(Name = "Exception")]
        public string Exception => MAlert.Exception;

        [Display(Name = "User ID")]
        public string UserId => MUser?.UserId;

        [Display(Name = "User Email")]
        public string UserEmail => MUser?.Email;

        [Display(Name = "Topic ID")]
        public long? TopicId => MAlert.TopicId;

        [Display(Name = "Topic Reference")]
        public string TopicReference => MAlert.TopicReference;

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