//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Diagnostics;

namespace RichTodd.QuiltSystem.Utility
{
    public static class ExceptionLogger
    {
        public static void Log(Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}
