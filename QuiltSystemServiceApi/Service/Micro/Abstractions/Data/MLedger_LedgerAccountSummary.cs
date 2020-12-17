﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MLedger_LedgerAccountSummary
    {
        public int LedgerAccountNumber { get; set; }
        public string Name { get; set; }
        public string DebitCreditCode { get; set; }
        public decimal Amount { get; set; }
    }
}