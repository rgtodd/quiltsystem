//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Utility
{
    public static class Is
    {
        public static bool Populated(ICollection collection)
        {
            return collection != null ? collection.Count > 0 : false;
        }

        public static bool Populated<T>(ICollection<T> collection)
        {
            return collection != null ? collection.Count > 0 : false;
        }
    }
}
