//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MSources
    {
        // MFulfillment
        //
        public const string Fulfillable = "Fulfillable";
        public const string ShipmentRequest = "Shipment Request";
        public const string Shipment = "Shipment";
        public const string ReturnRequest = "Return Request";
        public const string Return = "Return";

        // MFundable
        //
        public const string Funder = "Funder";
        public const string Fundable = "Fundable";

        // Ledger 
        //
        public const string LedgerTransaction = "Ledger Transaction";

        // MOrder
        //
        public const string Order = "Order";

        // MSquare
        //
        public const string SquarePayment = "Square Payment";
        public const string SquareRefund = "Square Refund";
    }
}
