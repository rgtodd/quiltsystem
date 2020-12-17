//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Return
    {
        public Return()
        {
            ReturnItems = new HashSet<ReturnItem>();
            ReturnTransactions = new HashSet<ReturnTransaction>();
        }

        public long ReturnId { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public string ReturnStatusCode { get; set; }
        public DateTime ReturnStatusDateTimeUtc { get; set; }
        public string ReturnNumber { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ReturnStatusType ReturnStatusCodeNavigation { get; set; }
        public virtual ICollection<ReturnItem> ReturnItems { get; set; }
        public virtual ICollection<ReturnTransaction> ReturnTransactions { get; set; }
    }
}
