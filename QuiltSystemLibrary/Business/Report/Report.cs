//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using Microsoft.Data.SqlClient;

using RichTodd.QuiltSystem.Service.Database.Abstractions;

namespace RichTodd.QuiltSystem.Business.Report
{
    public abstract class Report<TRecord> : IReport where TRecord : class
    {

        protected abstract int BreakLevelMax { get; }

        protected abstract int BreakLevelMin { get; }

        public void Run(IReportWriter wtr, IQuiltContextFactory quiltContextFactory)
        {
            WriteHeading(wtr);

            using (var conn = quiltContextFactory.CreateConnection())
            {
                conn.Open();

                var records = GetRecords(conn);

                TRecord previousRecord = null;

                foreach (var record in records)
                {
                    var currentBreakLevel = GetBreakLevel(record, previousRecord);

                    for (int breakLevel = BreakLevelMax; breakLevel >= currentBreakLevel; --breakLevel)
                    {
                        WriteTotals(wtr, breakLevel);
                    }

                    WriteRecord(wtr, record, previousRecord, currentBreakLevel);

                    AccumulateTotals(record);

                    previousRecord = record;
                }
            }

            for (int breakLevel = BreakLevelMax; breakLevel >= BreakLevelMin; --breakLevel)
            {
                WriteTotals(wtr, breakLevel);
            }
        }

        protected abstract void AccumulateTotals(TRecord record);

        protected abstract int GetBreakLevel(TRecord record, TRecord previousRecord);

        protected abstract IEnumerable<TRecord> GetRecords(SqlConnection conn);

        protected abstract void WriteHeading(IReportWriter wtr);

        protected abstract void WriteRecord(IReportWriter wtr, TRecord record, TRecord previousRecord, int breakLevel);

        protected abstract void WriteTotals(IReportWriter wtr, int breakLevel);

    }
}