//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Database.Readers
{
    public partial class ReportOrderLedgerAccountBalance
    {
        public decimal? Cash_1000 { get; set; }
        public decimal? Income_4000 { get; set; }
        public decimal? Income_4100 { get; set; }
        public decimal? IncomePayable_2100 { get; set; }
        public decimal? IncomeReceivableBalance_1100 { get; set; }
        public DateTime OrderDateTimeUtc { get; set; }
        public int? OrderMonth { get; set; }
        public string OrderNumber { get; set; }
        public int? OrderYear { get; set; }
        public decimal? SquareFees_5100 { get; set; }
        public decimal? SalesTaxPayable_2200 { get; set; }
        public decimal? SalesTaxReceivable_1200 { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
    }
}