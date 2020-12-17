//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Database
{
    public class DbRetryException : Exception
    {
        public DbRetryException()
        {
        }

        public DbRetryException(string message)
            : base(message)
        {
        }

        public DbRetryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
