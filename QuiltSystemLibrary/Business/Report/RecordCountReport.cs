//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using Microsoft.Data.SqlClient;

using RichTodd.QuiltSystem.Service.Database.Abstractions;

namespace RichTodd.QuiltSystem.Business.Report
{
    public class RecordCountReport : IReport
    {

        public void Run(IReportWriter wtr, IQuiltContextFactory quiltContextFactory)
        {
            using var conn = quiltContextFactory.CreateConnection();

            conn.Open();

            // Get list of tables.
            //
            var tableNames = new List<string>();
            using (var cmd = new SqlCommand("select name from sys.tables order by name", conn))
            {
                using var rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var tableName = rdr.GetString(0);
                    tableNames.Add(tableName);
                }
            }

            // Get record count for each table.
            //
            var recordCounts = new Dictionary<string, int>();
            foreach (var tableName in tableNames)
            {
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                using var cmd = new SqlCommand("select count(*) from [" + tableName + "]", conn);

                using var rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    var recordCount = rdr.GetInt32(0);
                    recordCounts.Add(tableName, recordCount);
                }
            }

            // Write report.
            //
            wtr.DefineColumn("Table Name");
            wtr.DefineColumn("Record Count");
            foreach (var tableName in tableNames)
            {
                wtr.WriteCell(tableName, 1);
                wtr.WriteCell(recordCounts[tableName].ToString(), 1);
            }
        }

    }
}