//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MSquare_CustomerSummary
    {
        public long SquareCustomerId { get; set; }
        public string SquareCustomerReference { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
    }
}
