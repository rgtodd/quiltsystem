//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Base
{
    internal static class TryParseSquarePaymentId
    {
        public static bool FromFunderReference(string reference, out long squarePaymentId)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (reference.StartsWith(ReferencePrefixes.SquarePayment))
            {
                squarePaymentId = long.Parse(reference.Substring(ReferencePrefixes.SquarePayment.Length));
                return true;
            }
            else
            {
                squarePaymentId = default;
                return false;
            }
        }
    }
}
