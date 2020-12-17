//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Report
{
    public class ReportModelFactory : ApplicationModelFactory
    {

        public ReportModel CreateReportModel(string html, string filter)
        {
            var model = new ReportModel()
            {
                HtmlTable = html,
                Filter = filter,
                Filters = new List<SelectListItem>
                {
                    new SelectListItem() { Text = "Record Counts", Value = "RecordCountReport" },
                    new SelectListItem() { Text = "Order Ledger Account Balances", Value = "OrderLedgerAccountBalancesReport" },
                    new SelectListItem() { Text = "Order Status", Value = "OrderStatusReport" },
                    new SelectListItem() { Text = "Type Table Summary", Value = "TypeTableSummaryReport" }
                }
            };

            return model;
        }

    }
}