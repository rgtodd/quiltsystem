//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Text;

using Microsoft.EntityFrameworkCore;

namespace RichTodd.QuiltSystem.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetDetail(this Exception exception)
        {
            var ex = exception;

            var sb = new StringBuilder();

            while (ex != null)
            {
                _ = sb.AppendLine(ex.Message);
                _ = sb.AppendLine(ex.GetType().ToString());
                _ = sb.AppendLine(ex.StackTrace);

                if (ex is DbUpdateException exDbUpdateException)
                {
                    var index = 0;
                    _ = sb.AppendLine("DbUpdateException Detail:");
                    foreach (var entityEntry in exDbUpdateException.Entries)
                    {
                        _ = sb.AppendLine($"{++index}) {entityEntry.Entity.GetType()}");
                    }
                }

                ex = ex.InnerException;
                if (ex != null)
                {
                    _ = sb.AppendLine("------------------------");
                }
            }

            return sb.ToString();
        }
    }
}
