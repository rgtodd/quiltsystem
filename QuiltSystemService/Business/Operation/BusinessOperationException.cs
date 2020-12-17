//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Business.Operation
{
    public class BusinessOperationException : Exception
    {

        public BusinessOperationException()
        { }

        public BusinessOperationException(string message)
            : base(message)
        { }

        public BusinessOperationException(string message, Exception inner)
            : base(message, inner)
        { }

    }
}