//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ReturnRequest
    {
        public ReturnRequest()
        {
            ReturnRequestItems = new HashSet<ReturnRequestItem>();
            ReturnRequestTransactions = new HashSet<ReturnRequestTransaction>();
        }

        public long ReturnRequestId { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public string ReturnRequestStatusCode { get; set; }
        public DateTime ReturnRequestStatusDateTimeUtc { get; set; }
        public string ReturnRequestNumber { get; set; }
        public string ReturnRequestTypeCode { get; set; }
        public string ReturnRequestReasonCode { get; set; }
        public string Notes { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ReturnRequestReason ReturnRequestReasonCodeNavigation { get; set; }
        public virtual ReturnRequestStatusType ReturnRequestStatusCodeNavigation { get; set; }
        public virtual ReturnRequestType ReturnRequestTypeCodeNavigation { get; set; }
        public virtual ICollection<ReturnRequestItem> ReturnRequestItems { get; set; }
        public virtual ICollection<ReturnRequestTransaction> ReturnRequestTransactions { get; set; }
    }
}
