//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.Data.SqlClient;

namespace RichTodd.QuiltSystem.Database.Extensions
{
    public static class SqlDataReaderExtensions
    {
        public static int? GetOptionalInt32(this SqlDataReader rdr, int i)
        {
            return !rdr.IsDBNull(i)
                ? (int?)rdr.GetInt32(i)
                : null;
        }

        public static decimal? GetOptionalDecimal(this SqlDataReader rdr, int i)
        {
            return !rdr.IsDBNull(i)
                ? (decimal?)rdr.GetDecimal(i)
                : null;
        }
    }
}