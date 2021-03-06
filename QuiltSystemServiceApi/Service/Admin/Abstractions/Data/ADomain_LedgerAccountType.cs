﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class ADomain_LedgerAccountType
    {
        public int LedgerAccountTypeId { get; set; }
        public string Name { get; set; }
        public string DebitCreditCode { get; set; }
    }
}