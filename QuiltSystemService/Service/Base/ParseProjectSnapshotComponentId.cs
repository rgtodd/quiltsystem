//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Base
{
    internal static class ParseProjectSnapshotComponentId
    {
        public static long FromOrderableComponentReference(string reference)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (!reference.StartsWith(ReferencePrefixes.ProjectSnapshotComponent))
            {
                throw new ArgumentException($"Reference {reference} is not an project snapshot component.");
            }

            return long.Parse(reference.Substring(ReferencePrefixes.ProjectSnapshotComponent.Length));
        }
    }
}
