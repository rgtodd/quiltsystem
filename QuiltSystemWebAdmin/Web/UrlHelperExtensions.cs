//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Base;

namespace RichTodd.QuiltSystem.WebAdmin.Web
{
    public static class UrlHelperExtensions
    {
        public static string Action(this IUrlHelper helper, ReferenceValues referenceValues)
        {
            if (referenceValues.OrderId != null)
            {
                return helper.Action("Index", "Order", new { id = referenceValues.OrderId.Value });
            }

            if (referenceValues.ReturnId != null)
            {
                return helper.Action("Index", "Return", new { id = referenceValues.ReturnId.Value });
            }

            if (referenceValues.ReturnRequestId != null)
            {
                return helper.Action("Index", "ReturnRequest", new { id = referenceValues.ReturnRequestId.Value });
            }

            if (referenceValues.ShipmentId != null)
            {
                return helper.Action("Index", "Shipment", new { id = referenceValues.ShipmentId.Value });
            }

            if (referenceValues.ShipmentRequestId != null)
            {
                return helper.Action("Index", "ShipmentRequest", new { id = referenceValues.ShipmentRequestId.Value });
            }

            if (referenceValues.SquareCustomerId != null)
            {
                return helper.Action("Index", "SquareCustomer", new { id = referenceValues.SquareCustomerId.Value });
            }

            if (referenceValues.SquarePaymentId != null)
            {
                return helper.Action("Index", "SquarePayment", new { id = referenceValues.SquarePaymentId.Value });
            }

            if (referenceValues.UserId != null)
            {
                return helper.Action("Index", "User", new { id = referenceValues.UserId });
            }

            return null;
        }

        public static string ActionLink(this IUrlHelper helper, ReferenceValues referenceValues)
        {
            if (referenceValues.OrderId != null)
            {
                return helper.ActionLink("Index", "Order", new { id = referenceValues.OrderId.Value });
            }

            return null;
        }
    }
}
