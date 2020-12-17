//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_ReturnTransaction : MCommon_Transaction
    {
        public override string Source => MSources.Return;
        public long ReturnTransactionId => TransactionId;
        public long ReturnId => EntityId;

        public MFulfillment_ReturnStatus ReturnStatus { get; set; }
    }
}
