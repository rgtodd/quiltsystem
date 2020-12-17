//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Text;

using Microsoft.Extensions.Logging;

namespace RichTodd.QuiltSystem
{
    public static class Function
    {
        public static IFunctionContext BeginFunction(ILogger Logger, string className, string functionName, params object[] args)
        {
            var sb = new StringBuilder();
            _ = sb.Append(className)
                .Append(".")
                .Append(functionName)
                .Append("(");
            {
                string prefix = string.Empty;
                foreach (var arg in args)
                {
                    _ = sb.Append(prefix);
                    prefix = ", ";

                    _ = sb.Append(arg);
                }
            }
            _ = sb.Append(")");

            var functionSignature = sb.ToString();

            var scope = Logger.BeginScope(functionSignature);

            return new FunctionContext(Logger, scope);
        }
    }
}
