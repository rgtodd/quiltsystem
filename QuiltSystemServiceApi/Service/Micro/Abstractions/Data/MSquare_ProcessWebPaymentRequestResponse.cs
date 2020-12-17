//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MSquare_ProcessWebPaymentRequestResponse
    {
        public long SquareWebPaymentRequestId { get; set; }
        public IList<MSquare_ProcessWebPaymentRequestResponseError> Errors { get; set; }
    }

    public class MSquare_ProcessWebPaymentRequestResponseError
    {
        public string Category { get; set; }
        public string Code { get; set; }
        public string Detail { get; set; }
        public string Field { get; set; }
    }
}
