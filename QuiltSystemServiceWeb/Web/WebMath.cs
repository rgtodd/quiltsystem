//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Web
{
    public static class WebMath
    {
        public static int GetPageNumber(int? page, int count, int pageSize)
        {
            int result = Math.Min(page ?? 1, (count + pageSize - 1) / pageSize);
            return Math.Max(result, 1);
        }
    }
}
