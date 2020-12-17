//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MOrder_OrderTransaction : MCommon_Transaction
    {
        public override string Source => MSources.Order;
        public long OrderTransactionId => TransactionId;

        public decimal FundsRequired { get; set; }
        public decimal FundsReceived { get; set; }
        public MOrder_OrderStatus? OrderStatus { get; set; }
    }
}
