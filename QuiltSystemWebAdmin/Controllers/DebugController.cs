//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

using RichTodd.QuiltSystem.Business.Report;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Core.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Feedback;
using RichTodd.QuiltSystem.WebAdmin.Models.Debug;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class DebugController : ApplicationController<DebugModelFactory>
    {
        private IQuiltContextFactory QuiltContextFactory { get; }
        private IOptionsMonitor<ApplicationOptions> ApplicationOptions { get; }
        private ISquareMicroService SquareMicroService { get; }

        public DebugController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IQuiltContextFactory quiltContextFactory,
            IOptionsMonitor<ApplicationOptions> applicationOptions,
            ISquareMicroService squareMicroService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            QuiltContextFactory = quiltContextFactory ?? throw new ArgumentNullException(nameof(quiltContextFactory));
            ApplicationOptions = applicationOptions ?? throw new ArgumentNullException(nameof(applicationOptions));
            SquareMicroService = squareMicroService ?? throw new ArgumentNullException(nameof(squareMicroService));
        }

        // GET: Debug
        public ActionResult Index()
        {
            var model = ModelFactory.CreateDebugModel(ApplicationOptions.CurrentValue);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> IndexSubmit(DebugModel model)
        {
            if (model.SquareWebookTransactionId != null)
            {
                try
                {
                    await SquareMicroService.ProcessWebhookPayloadAsync(model.SquareWebookTransactionId.Value);
                }
                catch (Exception ex)
                {
                    AddFeedbackMessage(FeedbackMessageTypes.Error, ex.ToString());
                }
            }

            model = ModelFactory.CreateDebugModel(ApplicationOptions.CurrentValue);

            return View("Index", model);
        }

        public async Task<ActionResult> PDF()
        {
            var testReport = new InvoiceDetailReport(QuiltContextFactory); //, m_designLibrary);
            var pdf = await testReport.Print(GetUserId());

            return new FileContentResult(pdf, new MediaTypeHeaderValue("application/pdf"));
        }

    }
}