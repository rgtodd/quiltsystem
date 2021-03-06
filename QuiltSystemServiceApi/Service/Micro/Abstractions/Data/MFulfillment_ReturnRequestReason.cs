﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_ReturnRequestReason
    {
        public string ReturnRequestReasonTypeCode { get; set; }

        public string Name { get; set; }
        public bool AllowRefund { get; set; }
        public bool AllowReplacement { get; set; }
    }
}
