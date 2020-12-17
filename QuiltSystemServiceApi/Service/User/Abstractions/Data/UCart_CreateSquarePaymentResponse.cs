//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.User.Abstractions.Data
{
    public class UCart_CreateSquarePaymentResponse
    {
        public IList<Cart_CreateSquarePaymentResponseErrorData> Errors { get; set; }
    }

    public class Cart_CreateSquarePaymentResponseErrorData
    {
        public string Category { get; set; }
        public string Code { get; set; }
        public string Detail { get; set; }
        public string Field { get; set; }
    }
}
