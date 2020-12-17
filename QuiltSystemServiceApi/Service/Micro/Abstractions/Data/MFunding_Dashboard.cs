//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFunding_Dashboard
    {
        public int TotalFunders { get; set; }
        public int TotalFundables { get; set; }
        public int TotalTransactions { get; set; }
        public int TotalEvents { get; set; }
    }
}
