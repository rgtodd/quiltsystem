//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Base
{
    public static class ParseProjectSnapshotId
    {
        public static long FromOrderableReference(string reference)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (!reference.StartsWith(ReferencePrefixes.ProjectSnapshot))
            {
                throw new ArgumentException($"Reference {reference} is not an project snapshot.");
            }

            return long.Parse(reference.Substring(ReferencePrefixes.ProjectSnapshot.Length));
        }
    }
}
