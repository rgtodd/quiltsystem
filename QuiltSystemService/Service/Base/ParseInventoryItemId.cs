//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Base
{
    internal static class ParseInventoryItemId
    {
        public static long FromConsumableReference(string reference)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (!reference.StartsWith(ReferencePrefixes.InventoryItem))
            {
                throw new ArgumentException($"Reference {reference} is not an inventory item.");
            }

            return long.Parse(reference.Substring(ReferencePrefixes.InventoryItem.Length));
        }
    }
}
