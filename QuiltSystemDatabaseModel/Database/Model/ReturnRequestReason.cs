//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ReturnRequestReason
    {
        public ReturnRequestReason()
        {
            ReturnRequests = new HashSet<ReturnRequest>();
        }

        public string ReturnRequestReasonCode { get; set; }
        public string Name { get; set; }
        public bool AllowRefund { get; set; }
        public bool AllowReplacement { get; set; }
        public bool Active { get; set; }
        public int SortOrder { get; set; }

        public virtual ICollection<ReturnRequest> ReturnRequests { get; set; }
    }
}
