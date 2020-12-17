//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Database.Readers
{
    public partial class ReportOrder
    {
        public int HasIncomeReceivable { get; set; }
        public int HasOpenOrderShipmentRequest { get; set; }
        public string OrderNumber { get; set; }
        public string OrderStatus { get; set; }
    }
}