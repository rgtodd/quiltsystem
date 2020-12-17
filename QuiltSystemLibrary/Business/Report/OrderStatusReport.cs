//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using Microsoft.Data.SqlClient;

using RichTodd.QuiltSystem.Database.Readers;

namespace RichTodd.QuiltSystem.Business.Report
{
    public class OrderStatusReport : StandardReport<ReportOrder>
    {

        protected override IEnumerable<ReportOrder> GetRecords(SqlConnection conn)
        {
            var sql = @"select
	O.OrderNumber,
	OST.Name as OrderStatus,
	case
		when exists (
			select * from
				OrderShipmentRequest OSR inner join
				OrderShipmentRequestStatusType OSRST on OSRST.OrderShipmentRequestStatusTypeCode = OSR.OrderShipmentRequestStatusTypeCode
			where
				OSR.OrderId = O.OrderId
			and OSRST.Name in ('Open', 'Posted')) then 1 else 0
	end as HasOpenOrderShipmentRequest,
	case
		when exists (
			select * from
				OrderLedgerAccount OLA
			where
				OLA.OrderId = O.OrderId
			and OLA.OrderLedgerAccountTypeCode = 1100 -- Income Receivable
			and OLA.Balance > 0
		) then 1 else 0
	end as HasIncomeReceivable
from
	[Order] O inner join
	OrderStatusType OST on OST.OrderStatusTypeCode = O.OrderStatusTypeCode";

            using var cmd = new SqlCommand(sql, conn);

            using var rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                var record = new ReportOrder()
                {
                    OrderNumber = rdr.GetString(0),
                    OrderStatus = rdr.GetString(1),
                    HasOpenOrderShipmentRequest = rdr.GetInt32(2),
                    HasIncomeReceivable = rdr.GetInt32(3)
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
                    Aggregates[breakLevel, idxAggregate] = new CountAggregate();
                }
            }
        }

        protected override void OnDefineColumns()
        {
            GroupColumns.Add(new Column() { Name = "Order Status", FieldAccessor = r => r.OrderStatus });
            GroupColumns.Add(new Column() { Name = "Income Receivable", FieldAccessor = r => r.HasIncomeReceivable });
            GroupColumns.Add(new Column() { Name = "Open Shipment Request", FieldAccessor = r => r.HasOpenOrderShipmentRequest });

            AggregateColumns.Add(new Column() { Name = "Order #", FieldAccessor = r => r.OrderNumber });
        }

    }
}