//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Test
{
    public class TestSalesTaxModel
    {
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal SalesTax { get; set; }

        public string SalesTaxJurisdiction { get; set; }
    }
}