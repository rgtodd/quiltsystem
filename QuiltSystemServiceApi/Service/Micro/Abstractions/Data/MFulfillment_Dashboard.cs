//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_Dashboard
    {
        public int TotalFulfillables { get; set; }
        public int TotalShipmentRequests { get; set; }
        public int TotalShipments { get; set; }
        public int TotalReturnRequests { get; set; }
        public int TotalReturns { get; set; }
        public int TotalTransactions { get; set; }
        public int TotalEvents { get; set; }
    }
}
