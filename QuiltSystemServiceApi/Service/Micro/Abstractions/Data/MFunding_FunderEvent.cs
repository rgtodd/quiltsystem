//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFunding_FunderEvent
    {
        public string EventTypeCode { get; set; }
        public long FunderId { get; set; }
        public string FunderReference { get; set; }
        public decimal FundsAvailable { get; set; }
        public string UnitOfWork { get; set; }
    }
}
