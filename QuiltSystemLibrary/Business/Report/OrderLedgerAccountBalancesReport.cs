//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using Microsoft.Data.SqlClient;

using RichTodd.QuiltSystem.Database.Extensions;
using RichTodd.QuiltSystem.Database.Readers;

namespace RichTodd.QuiltSystem.Business.Report
{
    public class OrderLedgerAccountBalancesReport : StandardReport<ReportOrderLedgerAccountBalance>
    {

        protected override IEnumerable<ReportOrderLedgerAccountBalance> GetRecords(SqlConnection conn)
        {
            var sql =
@"select
	O.OrderNumber,
	O.OrderDateTimeUtc,
	year(O.OrderDateTimeUtc) OrderYear,
	month(O.OrderDateTimeUtc) OrderMonth,
	O.StatusDateTimeUtc,
	coalesce(OLA_1000.Balance, 0) as Cash_1000,
	coalesce(OLA_1100.Balance, 0) as IncomeReceivableBalance_1100,
	coalesce(OLA_1200.Balance, 0) as SalesTaxReceivable_1200,
	coalesce(OLA_2100.Balance, 0) as IncomePayable_2100,
	coalesce(OLA_2200.Balance, 0) as SalesTaxPayable_2200,
	coalesce(OLA_4000.Balance, 0) as Income_4000,
	coalesce(OLA_4100.Balance, 0) as Income_4100,
	coalesce(OLA_5100.Balance, 0) as SquareFees_5100
from
	[Order] O left join
	[OrderLedgerAccount] OLA_1000 on OLA_1000.OrderId = O.OrderId and
	                                 OLA_1000.OrderLedgerAccountTypeCode = 1000 left join
	[OrderLedgerAccountType] OLAT_1000 on OLAT_1000.OrderLedgerAccountTypeCode = OLA_1000.OrderLedgerAccountTypeCode left join
	[OrderLedgerAccount] OLA_1100 on OLA_1100.OrderId = O.OrderId and
	                                 OLA_1100.OrderLedgerAccountTypeCode = 1100 left join
	[OrderLedgerAccountType] OLAT_1100 on OLAT_1100.OrderLedgerAccountTypeCode = OLA_1100.OrderLedgerAccountTypeCode left join
	[OrderLedgerAccount] OLA_1200 on OLA_1200.OrderId = O.OrderId and
	                                 OLA_1200.OrderLedgerAccountTypeCode = 1200 left join
	[OrderLedgerAccountType] OLAT_1200 on OLAT_1200.OrderLedgerAccountTypeCode = OLA_1200.OrderLedgerAccountTypeCode left join
	[OrderLedgerAccount] OLA_2100 on OLA_2100.OrderId = O.OrderId and
	                                 OLA_2100.OrderLedgerAccountTypeCode = 2100 left join
	[OrderLedgerAccountType] OLAT_2100 on OLAT_2100.OrderLedgerAccountTypeCode = OLA_2100.OrderLedgerAccountTypeCode left join
	[OrderLedgerAccount] OLA_2200 on OLA_2200.OrderId = O.OrderId and
	                                 OLA_2200.OrderLedgerAccountTypeCode = 2200 left join
	[OrderLedgerAccountType] OLAT_2200 on OLAT_2200.OrderLedgerAccountTypeCode = OLA_2200.OrderLedgerAccountTypeCode left join
	[OrderLedgerAccount] OLA_4000 on OLA_4000.OrderId = O.OrderId and
	                                 OLA_4000.OrderLedgerAccountTypeCode = 4000 left join
	[OrderLedgerAccountType] OLAT_4000 on OLAT_4000.OrderLedgerAccountTypeCode = OLA_4000.OrderLedgerAccountTypeCode left join
	[OrderLedgerAccount] OLA_4100 on OLA_4100.OrderId = O.OrderId and
	                                 OLA_4100.OrderLedgerAccountTypeCode = 4100 left join
	[OrderLedgerAccountType] OLAT_4100 on OLAT_4100.OrderLedgerAccountTypeCode = OLA_4100.OrderLedgerAccountTypeCode left join
	[OrderLedgerAccount] OLA_5100 on OLA_5100.OrderId = O.OrderId and
	                                 OLA_5100.OrderLedgerAccountTypeCode = 5100 left join
	[OrderLedgerAccountType] OLAT_5100 on OLAT_5100.OrderLedgerAccountTypeCode = OLA_5100.OrderLedgerAccountTypeCode";

            using var cmd = new SqlCommand(sql, conn);

            using var rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                var record = new ReportOrderLedgerAccountBalance()
                {
                    OrderNumber = rdr.GetString(0),
                    OrderDateTimeUtc = rdr.GetDateTime(1),
                    OrderYear = rdr.GetOptionalInt32(2),
                    OrderMonth = rdr.GetOptionalInt32(3),
                    StatusDateTimeUtc = rdr.GetDateTime(4),
                    Cash_1000 = rdr.GetOptionalDecimal(5),
                    IncomeReceivableBalance_1100 = rdr.GetOptionalDecimal(6),
                    SalesTaxReceivable_1200 = rdr.GetOptionalDecimal(7),
                    IncomePayable_2100 = rdr.GetOptionalDecimal(8),
                    SalesTaxPayable_2200 = rdr.GetOptionalDecimal(9),
                    Income_4000 = rdr.GetOptionalDecimal(10),
                    Income_4100 = rdr.GetOptionalDecimal(11),
                    SquareFees_5100 = rdr.GetOptionalDecimal(12)
                };

                yield return record;
            }
        }

        protected override void OnDefineAggregates()
        {
            for (int breakLevel = 0; breakLevel < GroupColumns.Count + 1; ++breakLevel)
            {
                for (int idxAggregate = 0; idxAggregate < AggregateColumns.Count; ++idxAggregate)
                {
                    Aggregates[breakLevel, idxAggregate] = new DecimalSumAggregate();
                }
            }
        }

        protected override void OnDefineColumns()
        {
            GroupColumns.Add(new Column() { Name = "Order Year", FieldAccessor = r => r.OrderYear });
            GroupColumns.Add(new Column() { Name = "Order Month", FieldAccessor = r => r.OrderMonth });

            DataColumns.Add(new Column() { Name = "Order #", FieldAccessor = r => r.OrderNumber });
            DataColumns.Add(new Column() { Name = "Order Date", FieldAccessor = r => r.OrderDateTimeUtc, Format = "{0:d}" });
            DataColumns.Add(new Column() { Name = "Status Date", FieldAccessor = r => r.StatusDateTimeUtc, Format = "{0:d}" });

            AggregateColumns.Add(new Column() { Name = "Cash (1000)", FieldAccessor = r => r.Cash_1000, Format = "{0:c}" });
            AggregateColumns.Add(new Column() { Name = "Income Receivable (1100)", FieldAccessor = r => r.IncomeReceivableBalance_1100, Format = "{0:c}" });
            AggregateColumns.Add(new Column() { Name = "Square Fees (5100)", FieldAccessor = r => r.SquareFees_5100, Format = "{0:c}" });
        }

    }
}