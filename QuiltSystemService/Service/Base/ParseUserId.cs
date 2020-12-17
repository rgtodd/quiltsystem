//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Base
{
    internal static class ParseUserId
    {
        public static string FromFunderReference(string reference)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (!reference.StartsWith(ReferencePrefixes.User))
            {
                throw new ArgumentException($"Reference {reference} is not a user.");
            }

            return reference.Substring(ReferencePrefixes.User.Length);
        }

        public static string FromOwnerReference(string reference)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (!reference.StartsWith(ReferencePrefixes.User))
            {
                throw new ArgumentException($"Reference {reference} is not a user.");
            }

            return reference.Substring(ReferencePrefixes.User.Length);
        }

        public static string FromOrdererReference(string reference)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (!reference.StartsWith(ReferencePrefixes.User))
            {
                throw new ArgumentException($"Reference {reference} is not a user.");
            }

            return reference.Substring(ReferencePrefixes.User.Length);
        }

        public static string FromParticipantReference(string reference)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (!reference.StartsWith(ReferencePrefixes.User))
            {
                throw new ArgumentException($"Reference {reference} is not a user.");
            }

            return reference.Substring(ReferencePrefixes.User.Length);
        }

        public static string FromSquareCustomerReference(string reference)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (!reference.StartsWith(ReferencePrefixes.User))
            {
                throw new ArgumentException($"Reference {reference} is not a user.");
            }

            return reference.Substring(ReferencePrefixes.User.Length);
        }
    }
}
