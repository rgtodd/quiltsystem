//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Data;
using System.Data.Common;

using Microsoft.Data.SqlClient;

namespace RichTodd.QuiltSystem.Database
{
    public static class DatabaseUtility
    {
        public static string GetOrderNumberX(DbConnection conn, DateTime utcNow)
        {
            string orderNumber;

            using (var tx = conn.BeginTransaction(IsolationLevel.Serializable))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetNextOrderNumber";

                    var orderDateUtcParameter = new SqlParameter("@OrderDateUtc", utcNow);
                    _ = cmd.Parameters.Add(orderDateUtcParameter);

                    var orderNumberParameter = new SqlParameter("@OrderNumber", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    _ = cmd.Parameters.Add(orderNumberParameter);

                    _ = cmd.ExecuteNonQuery();

                    var serialNumber = (int)orderNumberParameter.Value;

                    orderNumber = string.Format("{0:00}-{1:000}-{2:0000}", utcNow.Year - 2000, utcNow.DayOfYear, serialNumber);
                }

                tx.Commit();
            }

            return orderNumber;

            //using (var ctx = QuiltContext.Create())
            //{
            //    ctx.Database.Log = message => Trace.WriteLine(message);

            //    using (var tx = ctx.Database.BeginTransaction(IsolationLevel.Serializable))
            //    {
            //        var now = DateTime.UtcNow;

            //        var orderDateUtcParameter = new SqlParameter("@OrderDateUtc", now);
            //        var orderNumberParameter = new SqlParameter("@OrderNumber", SqlDbType.Int);
            //        orderNumberParameter.Direction = ParameterDirection.Output;

            //        ctx.Database.ExecuteSqlCommand("dbo.GetNextOrderNumber @OrderDateUtc, @OrderNumber output", orderDateUtcParameter, orderNumberParameter);

            //        int serialNumber = (int)orderNumberParameter.Value;

            //        var orderNumber = string.Format("{0:00}-{1:000}-{2:0000}", now.Year - 2000, now.DayOfYear, serialNumber);

            //        tx.Commit();

            //        return orderNumber;
            //    }
            //}
        }
    }
}
