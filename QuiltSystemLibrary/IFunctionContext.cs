//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem
{
    public interface IFunctionContext : IDisposable
    {
        void Message(string message);

        void Exception(Exception ex);

        void Result(object result);
    }
}
