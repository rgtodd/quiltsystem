//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MCommunication_Email
    {
        public long EmailRequestId { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public string EmailRequestStatusCode { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
        public string SenderEmail { get; set; }
        public string SenderEmailName { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientEmailName { get; set; }
        public long? RecipientParticipantId { get; set; }
        public string Subject { get; set; }
        public string BodyText { get; set; }
        public string BodyHtml { get; set; }
        public string BodyTypeCode { get; set; }
    }
}
