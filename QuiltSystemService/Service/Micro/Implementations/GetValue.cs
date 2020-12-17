//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal static class GetValue
    {
        public static MCommon_UnitsOfMeasure MCommon_UnitOfMeasure(string code)
        {
            return code switch
            {
                UnitOfMeasureCodes.FatQuarter => MCommon_UnitsOfMeasure.FatQuarter,
                UnitOfMeasureCodes.HalfYardage => MCommon_UnitsOfMeasure.HalfYardage,
                UnitOfMeasureCodes.ThreeYards => MCommon_UnitsOfMeasure.ThreeYards,
                UnitOfMeasureCodes.TwoYards => MCommon_UnitsOfMeasure.TwoYards,
                UnitOfMeasureCodes.Yardage => MCommon_UnitsOfMeasure.Yardage,
                _ => throw new ArgumentException($"Unknown value {code}."),
            };
        }

        public static MOrder_OrderStatus MOrder_OrderStatus(string code)
        {
            return code switch
            {
                OrderStatusCodes.Pending => Abstractions.Data.MOrder_OrderStatus.Pending,
                OrderStatusCodes.Submitted => Abstractions.Data.MOrder_OrderStatus.Submitted,
                OrderStatusCodes.Fulfilling => Abstractions.Data.MOrder_OrderStatus.Fulfilling,
                OrderStatusCodes.Closed => Abstractions.Data.MOrder_OrderStatus.Closed,
                _ => throw new ArgumentException($"Unknown value {code}."),
            };
        }

        public static MCommunication_AlertTypes MCommunication_AlertType(string code)
        {
            return code switch
            {
                AlertTypeCodes.OperationException => MCommunication_AlertTypes.OperationException,
                AlertTypeCodes.OrderPaymentMismatch => MCommunication_AlertTypes.OrderPaymentMismatch,
                AlertTypeCodes.OrderReceipt => MCommunication_AlertTypes.OrderReceipt,
                AlertTypeCodes.OrderReceiptFailure => MCommunication_AlertTypes.OrderReceiptFailure,
                AlertTypeCodes.OrderReceiptMismatch => MCommunication_AlertTypes.OrderReceiptMismatch,
                AlertTypeCodes.UnexpectedOrderPayment => MCommunication_AlertTypes.UnexpectedOrderPayment,
                _ => throw new ArgumentException($"Unknown value {code}."),
            };
        }

        public static MCommunication_NotificationTypes MCommunication_NotificationType(string code)
        {
            return code switch
            {
                NotificationTypeCodes.OrderShipped => MCommunication_NotificationTypes.OrderShipped,
                NotificationTypeCodes.OrderShipping => MCommunication_NotificationTypes.OrderShipping,
                NotificationTypeCodes.RefundIssued => MCommunication_NotificationTypes.RefundIssued,
                _ => throw new ArgumentException($"Unknown value {code}."),
            };
        }

        public static MFulfillment_FulfillableStatus MFulfillment_FulfillableStatus(string code)
        {
            return code switch
            {
                FulfillableStatusCodes.Open => Abstractions.Data.MFulfillment_FulfillableStatus.Open,
                FulfillableStatusCodes.Closed => Abstractions.Data.MFulfillment_FulfillableStatus.Closed,
                _ => throw new ArgumentException($"Unknown value {code}."),
            };
        }

        public static MFulfillment_FulfillmentEventTypes MFulfillment_FulfillmentEventType(string code)
        {
            return code switch
            {
                FulfillmentEventTypeCodes.Shipment => MFulfillment_FulfillmentEventTypes.Shipment,
                FulfillmentEventTypeCodes.Return => MFulfillment_FulfillmentEventTypes.Return,
                _ => throw new ArgumentException($"Unknown value {code}."),
            };
        }

        public static MFulfillment_ShipmentEventTypes MFulfillment_ShipmentEventType(string code)
        {
            return code switch
            {
                ShipmentEventTypeCodes.Cancel => MFulfillment_ShipmentEventTypes.Cancel,
                ShipmentEventTypeCodes.Post => MFulfillment_ShipmentEventTypes.Post,
                ShipmentEventTypeCodes.Process => MFulfillment_ShipmentEventTypes.Process,
                _ => throw new ArgumentException($"Unknown value {code}."),
            };
        }

        public static MFulfillment_ShipmentRequestEventTypes MFulfillment_ShipmentRequestEventType(string code)
        {
            return code switch
            {
                ShipmentRequestEventTypeCodes.Cancel => MFulfillment_ShipmentRequestEventTypes.Cancel,
                ShipmentRequestEventTypeCodes.Post => MFulfillment_ShipmentRequestEventTypes.Post,
                ShipmentRequestEventTypeCodes.Process => MFulfillment_ShipmentRequestEventTypes.Process,
                ShipmentRequestEventTypeCodes.Complete => MFulfillment_ShipmentRequestEventTypes.Complete,
                _ => throw new ArgumentException($"Unknown value {code}."),
            };
        }

        public static MFulfillment_ShipmentRequestStatus MFulfillment_ShipmentRequestStatus(string code)
        {
            return code switch
            {
                ShipmentRequestStatusCodes.Pending => Abstractions.Data.MFulfillment_ShipmentRequestStatus.Pending,
                ShipmentRequestStatusCodes.Open => Abstractions.Data.MFulfillment_ShipmentRequestStatus.Open,
                ShipmentRequestStatusCodes.Complete => Abstractions.Data.MFulfillment_ShipmentRequestStatus.Complete,
                ShipmentRequestStatusCodes.Cancelled => Abstractions.Data.MFulfillment_ShipmentRequestStatus.Cancelled,
                ShipmentRequestStatusCodes.Exception => Abstractions.Data.MFulfillment_ShipmentRequestStatus.Exception,
                _ => throw new ArgumentException($"Unknown value {code}."),
            };
        }

        public static MFulfillment_ShipmentStatus MFulfillment_ShipmentStatus(string code)
        {
            return code switch
            {
                ShipmentStatusCodes.Cancelled => Abstractions.Data.MFulfillment_ShipmentStatus.Cancelled,
                ShipmentStatusCodes.Complete => Abstractions.Data.MFulfillment_ShipmentStatus.Complete,
                ShipmentStatusCodes.Exception => Abstractions.Data.MFulfillment_ShipmentStatus.Exception,
                ShipmentStatusCodes.Open => Abstractions.Data.MFulfillment_ShipmentStatus.Open,
                ShipmentStatusCodes.Posted => Abstractions.Data.MFulfillment_ShipmentStatus.Posted,
                _ => throw new ArgumentException($"Unknown value {code}."),
            };
        }

        public static MFulfillment_ReturnEventTypes MFulfillment_ReturnEventType(string code)
        {
            return code switch
            {
                ReturnEventTypeCodes.Cancel => MFulfillment_ReturnEventTypes.Cancel,
                ReturnEventTypeCodes.Post => MFulfillment_ReturnEventTypes.Post,
                ReturnEventTypeCodes.Process => MFulfillment_ReturnEventTypes.Process,
                _ => throw new ArgumentException($"Unknown value {code}."),
            };
        }

        public static MFulfillment_ReturnRequestEventTypes MFulfillment_ReturnRequestEventType(string code)
        {
            return code switch
            {
                ReturnRequestEventTypeCodes.Cancel => MFulfillment_ReturnRequestEventTypes.Cancel,
                ReturnRequestEventTypeCodes.Post => MFulfillment_ReturnRequestEventTypes.Post,
                ReturnRequestEventTypeCodes.Process => MFulfillment_ReturnRequestEventTypes.Process,
                _ => throw new ArgumentException($"Unknown value {code}."),
            };
        }

        public static MFulfillment_ReturnRequestStatus MFulfillment_ReturnRequestStatus(string code)
        {
            return code switch
            {
                ReturnRequestStatusCodes.Cancelled => Abstractions.Data.MFulfillment_ReturnRequestStatus.Cancelled,
                ReturnRequestStatusCodes.Complete => Abstractions.Data.MFulfillment_ReturnRequestStatus.Complete,
                ReturnRequestStatusCodes.Exception => Abstractions.Data.MFulfillment_ReturnRequestStatus.Exception,
                ReturnRequestStatusCodes.Open => Abstractions.Data.MFulfillment_ReturnRequestStatus.Open,
                ReturnRequestStatusCodes.Posted => Abstractions.Data.MFulfillment_ReturnRequestStatus.Posted,
                _ => throw new ArgumentException($"Unknown value {code}."),
            };
        }

        public static MFulfillment_ReturnStatus MFulfillment_ReturnStatus(string code)
        {
            return code switch
            {
                ReturnStatusCodes.Cancelled => Abstractions.Data.MFulfillment_ReturnStatus.Cancelled,
                ReturnStatusCodes.Complete => Abstractions.Data.MFulfillment_ReturnStatus.Complete,
                ReturnStatusCodes.Exception => Abstractions.Data.MFulfillment_ReturnStatus.Exception,
                ReturnStatusCodes.Open => Abstractions.Data.MFulfillment_ReturnStatus.Open,
                ReturnStatusCodes.Posted => Abstractions.Data.MFulfillment_ReturnStatus.Posted,
                _ => throw new ArgumentException($"Unknown value {code}."),
            };
        }

        public static MFulfillment_ReturnRequestTypes MFulfillment_ReturnRequestType(string code)
        {
            return code switch
            {
                ReturnRequestTypeCodes.Manual => MFulfillment_ReturnRequestTypes.Manual,
                ReturnRequestTypeCodes.Return => MFulfillment_ReturnRequestTypes.Return,
                ReturnRequestTypeCodes.Replace => MFulfillment_ReturnRequestTypes.Replace,
                _ => throw new ArgumentException($"Unknown value {code}."),
            };
        }
    }
}
