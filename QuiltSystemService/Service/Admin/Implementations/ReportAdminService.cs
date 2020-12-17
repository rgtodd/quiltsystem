//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Business.Report;
using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;

namespace RichTodd.QuiltSystem.Service.Admin.Implementations
{
    internal class ReportAdminService : BaseService, IReportAdminService
    {
        public ReportAdminService(
            IApplicationRequestServices requestServices,
            ILogger<ReportAdminService> logger)
            : base(requestServices, logger)
        { }

        #region IAdmin_ReportService

        public async Task<AReport_Report> GetReportAsync(AReport_GetReport request)
        {
            using var log = BeginFunction(nameof(ReportAdminService), nameof(AReport_Report), request);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var report = request.ReportName switch
                {
                    "RecordCountReport" => new RecordCountReport(),
                    "OrderLedgerAccountBalancesReport" => new OrderLedgerAccountBalancesReport(),
                    "OrderStatusReport" => new OrderStatusReport(),
                    "TypeTableSummaryReport" => new TypeTableSummaryReport(),
                    _ => (IReport)null,
                };

                var result = new AReport_Report();

                //if (report != null)
                //{
                //    var writer = new HtmlReportWriter();
                //    report.Run(writer, QuiltContextFactory);

                //    using var stringWriter = new StringWriter();

                //    writer.RenderHtmlTable(stringWriter);
                //    result.Html = stringWriter.ToString();
                //}

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #endregion
    }
}
