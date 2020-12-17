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
    public class TypeTableSummaryReport : StandardReport<ReportTypeTableSummary>
    {

        protected override IEnumerable<ReportTypeTableSummary> GetRecords(SqlConnection conn)
        {
            var sql = @"select
	'AccountPayment' as TableName,
	'AccountPaymentStatusType' as FieldName,
	T.Name,
	count(D.AccountPaymentId) as RecordCount
from
	AccountPaymentStatusType T left join
	AccountPayment D on D.AccountPaymentStatusTypeCode = T.AccountPaymentStatusTypeCode
group by T.Name

union all

select
	'AccountReceipt' as TableName,
	'AccountReceiptStatusType' as FieldName,
	T.Name,
	count(D.AccountReceiptId) as RecordCount
from
	AccountReceiptStatusType T left join
	AccountReceipt D on D.AccountReceiptStatusTypeCode = T.AccountReceiptStatusTypeCode
group by T.Name

union all

select
	'UserAddress' as TableName,
	'AddressType' as FieldName,
	T.Name,
	count(D.UserAddressId) as RecordCount
from
	AddressType T left join
	UserAddress D on D.AddressTypeCode = T.AddressTypeCode
group by
	T.Name

union all

select
	'Alert' as TableName,
	'AlertType' as FieldName,
	T.Name,
	count(D.AlertId) as RecordCount
from
	AlertType T left join
	Alert D on D.AlertTypeCode = T.AlertTypeCode
group by
	T.Name

union all

select
	'Artifact' as TableName,
	'ArtifactType' as FieldName,
	T.Name,
	count(D.ArtifactId) as RecordCount
from
	ArtifactType T left join
	Artifact D on D.ArtifactTypeCode = T.ArtifactTypeCode
group by
	T.Name

union all

select
	'Artifact' as TableName,
	'ArtifactValueType' as FieldName,
	T.Name,
	count(D.ArtifactId) as RecordCount
from
	ArtifactValueType T left join
	Artifact D on D.ArtifactValueTypeCode = T.ArtifactValueTypeCode
group by
	T.Name

union all

select
	'InventoryItemTransaction' as TableName,
	'InventoryItemTransactionType' as FieldName,
	T.Name,
	count(D.InventoryItemTransactionId) as RecordCount
from
	InventoryItemTransactionType T left join
	InventoryItemTransaction D on D.InventoryItemTransactionTypeCode = T.InventoryItemTransactionTypeCode
group by
	T.Name

union all

select
	'InventoryItem' as TableName,
	'InventoryItemType' as FieldName,
	T.Name,
	count(D.InventoryItemId) as RecordCount
from
	InventoryItemType T left join
	InventoryItem D on D.InventoryItemTypeCode = T.InventoryItemTypeCode
group by
	T.Name

union all

select
	'LogEntry' as TableName,
	'LogEntryType' as FieldName,
	T.Name,
	count(D.LogEntryId) as RecordCount
from
	LogEntryType T left join
	LogEntry D on D.LogEntryTypeCode = T.LogEntryTypeCode
group by
	T.Name

union all

select
	'Notification' as TableName,
	'NotificationType' as FieldName,
	T.Name,
	count(D.NotificationId) as RecordCount
from
	NotificationType T left join
	Notification D on D.NotificationTypeCode = T.NotificationTypeCode
group by
	T.Name

union all

select
	'OrderItem' as TableName,
	'OrderItemStatusType' as FieldName,
	T.Name,
	count(D.OrderItemId) as RecordCount
from
	OrderItemStatusType T left join
	OrderItem D on D.OrderItemStatusTypeCode = T.OrderItemStatusTypeCode
group by
	T.Name

union all

select
	'OrderLedgerAccount' as TableName,
	'LedgerAccountType' as FieldName,
	T.Name,
	count(D.OrderLedgerAccountId) as RecordCount
from
	LedgerAccountType T left join
	OrderLedgerAccount D on D.LedgerAccountTypeCode = T.LedgerAccountTypeCode
group by
	T.Name

union all

select
	'OrderReturnRequest' as TableName,
	'OrderReturnRequestStatusType' as FieldName,
	T.Name,
	count(D.OrderReturnRequestId) as RecordCount
from
	OrderReturnRequestStatusType T left join
	OrderReturnRequest D on D.OrderReturnRequestStatusTypeCode = T.OrderReturnRequestStatusTypeCode
group by
	T.Name

union all

select
	'OrderReturnRequest' as TableName,
	'OrderReturnRequestType' as FieldName,
	T.Name,
	count(D.OrderReturnRequestId) as RecordCount
from
	OrderReturnRequestType T left join
	OrderReturnRequest D on D.OrderReturnRequestTypeCode = T.OrderReturnRequestTypeCode
group by
	T.Name

union all

select
	'OrderReturn' as TableName,
	'OrderReturnStatusType' as FieldName,
	T.Name,
	count(D.OrderReturnId) as RecordCount
from
	OrderReturnStatusType T left join
	OrderReturn D on D.OrderReturnStatusTypeCode = T.OrderReturnStatusTypeCode
group by
	T.Name

union all

select
	'OrderShipmentRequest' as TableName,
	'OrderShipmentRequestStatusType' as FieldName,
	T.Name,
	count(D.OrderShipmentRequestId) as RecordCount
from
	OrderShipmentRequestStatusType T left join
	OrderShipmentRequest D on D.OrderShipmentRequestStatusTypeCode = T.OrderShipmentRequestStatusTypeCode
group by
	T.Name

union all

select
	'OrderShipment' as TableName,
	'OrderShipmentStatusType' as FieldName,
	T.Name,
	count(D.OrderShipmentId) as RecordCount
from
	OrderShipmentStatusType T left join
	OrderShipment D on D.OrderShipmentStatusTypeCode = T.OrderShipmentStatusTypeCode
group by
	T.Name

union all

select
	'Order' as TableName,
	'OrderStatusType' as FieldName,
	T.Name,
	count(D.OrderId) as RecordCount
from
	OrderStatusType T left join
	[Order] D on D.OrderStatusTypeCode = T.OrderStatusTypeCode
group by
	T.Name

union all

select
	'OrderTransaction' as TableName,
	'OrderTransactionType' as FieldName,
	T.Name,
	count(D.OrderTransactionId) as RecordCount
from
	OrderTransactionType T left join
	OrderTransaction D on D.OrderTransactionTypeCode = T.OrderTransactionTypeCode
group by
	T.Name

union all

select
	'Resource' as TableName,
	'ResourceType' as FieldName,
	T.Name,
	count(D.ResourceId) as RecordCount
from
	ResourceType T left join
	[Resource] D on D.ResourceTypeId = T.ResourceTypeId
group by
	T.Name
";
            using var cmd = new SqlCommand(sql, conn);

            using var rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                var record = new ReportTypeTableSummary()
                {
                    TableName = rdr.GetString(0),
                    FieldName = rdr.GetString(1),
                    Name = rdr.GetString(2),
                    RecordCount = rdr.GetOptionalInt32(3)
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
                    Aggregates[breakLevel, idxAggregate] = new IntegerSumAggregate();
                }
            }
        }

        protected override void OnDefineColumns()
        {
            GroupColumns.Add(new Column() { Name = "Table", FieldAccessor = r => r.TableName });
            GroupColumns.Add(new Column() { Name = "Field", FieldAccessor = r => r.FieldName });

            DataColumns.Add(new Column() { Name = "Value", FieldAccessor = r => r.Name });

            AggregateColumns.Add(new Column() { Name = "# Records", FieldAccessor = r => r.RecordCount });
        }

    }
}