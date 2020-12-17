//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFunding_Funder
    {
        // Funder
        //
        public long FunderId { get; set; }
        public string FunderReference { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }

        // Account Totals
        //
        public decimal TotalFundsReceived { get; set; }
        public decimal TotalFundsAvailable { get; set; }
        public decimal TotalFundsRefunded { get; set; }
        public decimal TotalFundsRefundable { get; set; }
        public decimal TotalProcessingFee { get; set; }

        public IList<MFunding_FunderAccount> Accounts { get; set; }
        public IList<MFunding_FunderTransaction> FunderTransactions { get; set; }
    }

    public class MFunding_FunderAccount
    {
        public string FundableReference { get; set; }
        public decimal FundsReceived { get; set; }
        public decimal FundsAvailable { get; set; }
        public decimal FundsRefunded { get; set; }
        public decimal FundsRefundable { get; set; }
        public decimal ProcessingFee { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
    }
}
