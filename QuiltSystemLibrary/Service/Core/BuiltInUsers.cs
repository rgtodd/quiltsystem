//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Core
{
    public static class BuiltInUsers
    {
        public static string ServiceUser
        {
            get
            {
                return new Guid("00000000-0000-0000-0000-000000000000").ToString();
            }
        }

        public static string AdminUser
        {
            get
            {
                return new Guid("5A72B158-7F3B-4F49-B4A2-4368DFCACCA1").ToString();
            }
        }
    }
}
