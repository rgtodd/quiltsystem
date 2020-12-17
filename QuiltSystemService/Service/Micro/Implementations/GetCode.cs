//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal static class GetCode
    {
        public static string From(MOrder_OrderStatus value)
        {
            return value switch
            {
                MOrder_OrderStatus.Pending => OrderStatusCodes.Pending,
                MOrder_OrderStatus.Submitted => OrderStatusCodes.Submitted,
                MOrder_OrderStatus.Fulfilling => OrderStatusCodes.Fulfilling,
                MOrder_OrderStatus.Closed => OrderStatusCodes.Closed,
                _ => throw new ArgumentException($"Unknown value {value}."),
            };
        }

        public static string From(MFulfillment_FulfillableStatus value)
        {
            return value switch
            {
                MFulfillment_FulfillableStatus.Open => FulfillableStatusCodes.Open,
                MFulfillment_FulfillableStatus.Closed => FulfillableStatusCodes.Closed,
                _ => throw new ArgumentException($"Unknown value {value}."),
            };
        }

        public static string From(MFulfillment_ShipmentEventTypes value)
        {
            return value switch
            {
                MFulfillment_ShipmentEventTypes.Cancel => ShipmentEventTypeCodes.Cancel,
                MFulfillment_ShipmentEventTypes.Post => ShipmentEventTypeCodes.Post,
                MFulfillment_ShipmentEventTypes.Process => ShipmentEventTypeCodes.Process,
                _ => throw new ArgumentException($"Unknown value {value}."),
            };
        }

        public static string From(MFulfillment_ShipmentRequestEventTypes value)
        {
            return value switch
            {
                MFulfillment_ShipmentRequestEventTypes.Cancel => ShipmentRequestEventTypeCodes.Cancel,
                MFulfillment_ShipmentRequestEventTypes.Post => ShipmentRequestEventTypeCodes.Post,
                MFulfillment_ShipmentRequestEventTypes.Process => ShipmentRequestEventTypeCodes.Process,
                MFulfillment_ShipmentRequestEventTypes.Complete => ShipmentRequestEventTypeCodes.Complete,
                _ => throw new ArgumentException($"Unknown value {value}."),
            };
        }

        public static string From(MFulfillment_ShipmentRequestStatus value)
        {
            return value switch
            {
                MFulfillment_ShipmentRequestStatus.Pending => ShipmentRequestStatusCodes.Pending,
                MFulfillment_ShipmentRequestStatus.Open => ShipmentRequestStatusCodes.Open,
                MFulfillment_ShipmentRequestStatus.Complete => ShipmentRequestStatusCodes.Complete,
                MFulfillment_ShipmentRequestStatus.Cancelled => ShipmentRequestStatusCodes.Cancelled,
                MFulfillment_ShipmentRequestStatus.Exception => ShipmentRequestStatusCodes.Exception,
                _ => throw new ArgumentException($"Unknown value {value}."),
            };
        }

        public static string From(MFulfillment_ShipmentStatus value)
        {
            return value switch
            {
                MFulfillment_ShipmentStatus.Cancelled => ShipmentStatusCodes.Cancelled,
                MFulfillment_ShipmentStatus.Complete => ShipmentStatusCodes.Complete,
                MFulfillment_ShipmentStatus.Exception => ShipmentStatusCodes.Exception,
                MFulfillment_ShipmentStatus.Open => ShipmentStatusCodes.Open,
                MFulfillment_ShipmentStatus.Posted => ShipmentStatusCodes.Posted,
                _ => throw new ArgumentException($"Unknown value {value}."),
            };
        }

        public static string From(MFulfillment_ReturnEventTypes value)
        {
            return value switch
            {
                MFulfillment_ReturnEventTypes.Cancel => ReturnEventTypeCodes.Cancel,
                MFulfillment_ReturnEventTypes.Post => ReturnEventTypeCodes.Post,
                MFulfillment_ReturnEventTypes.Process => ReturnEventTypeCodes.Process,
                _ => throw new ArgumentException($"Unknown value {value}."),
            };
        }

        public static string From(MFulfillment_ReturnRequestEventTypes value)
        {
            return value switch
            {
                MFulfillment_ReturnRequestEventTypes.Cancel => ReturnRequestEventTypeCodes.Cancel,
                MFulfillment_ReturnRequestEventTypes.Post => ReturnRequestEventTypeCodes.Post,
                MFulfillment_ReturnRequestEventTypes.Process => ReturnRequestEventTypeCodes.Process,
                _ => throw new ArgumentException($"Unknown value {value}."),
            };
        }

        public static string From(MFulfillment_ReturnRequestStatus value)
        {
            return value switch
            {
                MFulfillment_ReturnRequestStatus.Cancelled => ReturnRequestStatusCodes.Cancelled,
                MFulfillment_ReturnRequestStatus.Complete => ReturnRequestStatusCodes.Complete,
                MFulfillment_ReturnRequestStatus.Exception => ReturnRequestStatusCodes.Exception,
                MFulfillment_ReturnRequestStatus.Open => ReturnRequestStatusCodes.Open,
                MFulfillment_ReturnRequestStatus.Posted => ReturnRequestStatusCodes.Posted,
                _ => throw new ArgumentException($"Unknown value {value}."),
            };
        }

        public static string From(MFulfillment_ReturnStatus value)
        {
            return value switch
            {
                MFulfillment_ReturnStatus.Cancelled => ReturnStatusCodes.Cancelled,
                MFulfillment_ReturnStatus.Complete => ReturnStatusCodes.Complete,
                MFulfillment_ReturnStatus.Exception => ReturnStatusCodes.Exception,
                MFulfillment_ReturnStatus.Open => ReturnStatusCodes.Open,
                MFulfillment_ReturnStatus.Posted => ReturnStatusCodes.Posted,
                _ => throw new ArgumentException($"Unknown value {value}."),
            };
        }

        public static string From(MFulfillment_ReturnRequestTypes value)
        {
            return value switch
            {
                MFulfillment_ReturnRequestTypes.Manual => ReturnRequestTypeCodes.Manual,
                MFulfillment_ReturnRequestTypes.Replace => ReturnRequestTypeCodes.Replace,
                MFulfillment_ReturnRequestTypes.Return => ReturnRequestTypeCodes.Return,
                _ => throw new ArgumentException($"Unknown value {value}."),
            };
        }
    }
}
