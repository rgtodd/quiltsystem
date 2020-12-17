//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Base
{
    internal static class TryParseUserId
    {
        public static bool FromFunderReference(string reference, out string userId)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (reference.StartsWith(ReferencePrefixes.User))
            {
                userId = reference.Substring(ReferencePrefixes.User.Length);
                return true;
            }
            else
            {
                userId = default;
                return false;
            }
        }

        public static bool FromOrdererReference(string reference, out string userId)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (reference.StartsWith(ReferencePrefixes.User))
            {
                userId = reference.Substring(ReferencePrefixes.User.Length);
                return true;
            }
            else
            {
                userId = default;
                return false;
            }
        }

        public static bool FromParticipantReference(string reference, out string userId)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (reference.StartsWith(ReferencePrefixes.User))
            {
                userId = reference.Substring(ReferencePrefixes.User.Length);
                return true;
            }
            else
            {
                userId = default;
                return false;
            }
        }

        public static bool FromSquareCustomerReference(string reference, out string userId)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (reference.StartsWith(ReferencePrefixes.User))
            {
                userId = reference.Substring(ReferencePrefixes.User.Length);
                return true;
            }
            else
            {
                userId = default;
                return false;
            }
        }
    }
}
