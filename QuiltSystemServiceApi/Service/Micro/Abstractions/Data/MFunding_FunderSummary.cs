//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFunding_FunderSummary
    {
        public long FunderId { get; set; }
        public string FunderReference { get; set; }
        public decimal TotalFundsReceived { get; set; }
        public decimal TotalFundsAvailable { get; set; }
        public decimal TotalFundsRefunded { get; set; }
        public decimal TotalFundsRefundable { get; set; }
        public decimal TotalProcessingFee { get; set; }
    }
}
