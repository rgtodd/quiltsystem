//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MLedger_LedgerTransactionSummary : MCommon_TransactionSummary
    {
        public override string Source => MSources.LedgerTransaction;
        public long LedgerTransactionId => TransactionId;

        public DateTime PostDateTime { get; set; }
    }
}
