CREATE view [dbo].[FulfillableToShipmentRequestView] as
select distinct
	FI.FulfillableId,
	SRI.ShipmentRequestId
from
	FulfillableItem FI inner join
	ShipmentRequestItem SRI on SRI.FulfillableItemId = FI.FulfillableItemId;
GO
CREATE view [dbo].[ShipmentRequestSummaryView] as
select 
	SR.ShipmentRequestId,
	SR.ShipmentRequestNumber,
	SR.ShipmentRequestStatusCode,
	SR.ShipmentRequestStatusDateTimeUtc,
	SR.CreateDateTimeUtc,
	F.FulfillableId,
	F.Name as FulfillableName,
	F.FulfillableReference
from
	ShipmentRequest SR inner join
	FulfillableToShipmentRequestView V on V.ShipmentRequestId = SR.ShipmentRequestId inner join
	Fulfillable F on F.FulfillableId = V.FulfillableId;
GO
create view [dbo].[FulfillableToShipmentView] as
select distinct
	FI.FulfillableId,
	SI.ShipmentId
from
	FulfillableItem FI inner join
	ShipmentRequestItem SRI on SRI.FulfillableItemId = FI.FulfillableItemId inner join
	ShipmentItem SI on SI.ShipmentRequestItemId = SRI.ShipmentRequestItemId;
GO
create view [dbo].[ShipmentSummaryView] as 
select 
	S.ShipmentId,
	S.ShipmentNumber,
	S.ShipmentStatusCode,
	S.ShipmentStatusDateTimeUtc,
	S.CreateDateTimeUtc,
	S.ShippingVendorId,
	S.TrackingCode,
	F.FulfillableId,
	F.FulfillableReference,
	F.Name as FulfillableName
from
	Shipment S inner join
	FulfillableToShipmentView V on V.ShipmentId = S.ShipmentId inner join
	Fulfillable F on F.FulfillableId = V.FulfillableId;
GO
CREATE view [dbo].[FulfillableToReturnRequestView] as
select distinct
	FI.FulfillableId,
	RRI.ReturnRequestId
from
	FulfillableItem FI inner join
	ReturnRequestItem RRI on RRI.FulfillableItemId = FI.FulfillableItemId;
GO
create view [dbo].[ReturnRequestSummaryView] as 
select 
	RR.ReturnRequestId,
	RR.ReturnRequestNumber,
	RR.ReturnRequestStatusCode,
	RR.ReturnRequestStatusDateTimeUtc,
	RR.CreateDateTimeUtc,
	F.FulfillableId,
	F.FulfillableReference,
	F.Name as FulfillableName
from
	ReturnRequest RR inner join
	FulfillableToReturnRequestView V on V.ReturnRequestId = RR.ReturnRequestId inner join
	Fulfillable F on F.FulfillableId = V.FulfillableId;
GO
create view [dbo].[FulfillableToReturnView] as
select distinct
	FI.FulfillableId,
	RI.ReturnId
from
	FulfillableItem FI inner join
	ReturnRequestItem RRI on RRI.FulfillableItemId = FI.FulfillableItemId inner join
	ReturnItem RI on RI.ReturnRequestItemId = RRI.ReturnRequestItemId;
GO
create view [dbo].[ReturnSummaryView] as 
select 
	R.ReturnId,
	R.ReturnNumber,
	R.ReturnStatusCode,
	R.ReturnStatusDateTimeUtc,
	R.CreateDateTimeUtc,
	F.FulfillableId,
	F.FulfillableReference,
	F.Name as FulfillableName
from
	[Return] R inner join
	FulfillableToReturnView V on V.ReturnId = R.ReturnId inner join
	Fulfillable F on F.FulfillableId = V.FulfillableId;
GO
create view [dbo].[FulfillableSummaryView] as
with FulfillableQuantities as (
	select 
		FI.FulfillableId,
		sum(FI.RequestQuantity) as TotalRequestQuantity,
		sum(FI.CompleteQuantity) as TotalCompleteQuantity,
		sum(FI.ReturnQuantity) as TotalReturnQuantity
	from
		FulfillableItem FI
	group by
		FI.FulfillableId
)
select
	F.FulfillableId,
	F.FulfillableReference,
	F.Name as FulfillableName,
	F.FulfillableStatusCode,
	F.FulfillableStatusDateTimeUtc,
	F.CreateDateTimeUtc,
	FQ.TotalRequestQuantity,
	FQ.TotalCompleteQuantity,
	FQ.TotalReturnQuantity
from
	Fulfillable F inner join
	FulfillableQuantities FQ on FQ.FulfillableId = F.FulfillableId;
GO
create view [dbo].[ReturnRequestToReturnView] as
select distinct
	RRI.ReturnRequestId,
	RI.ReturnId
from
	ReturnRequestItem RRI inner join
	ReturnItem RI on RI.ReturnRequestItemId = RRI.ReturnRequestItemId;
GO
create view [dbo].[ShipmentRequestToShipmentView] as
select distinct
	SRI.ShipmentRequestId,
	SI.ShipmentId
from
	ShipmentRequestItem SRI inner join
	ShipmentItem SI on SI.ShipmentRequestItemId = SRI.ShipmentRequestItemId;