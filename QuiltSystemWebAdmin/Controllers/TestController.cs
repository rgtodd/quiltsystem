//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.WebAdmin.Models.Test;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class TestController : ApplicationController<TestModelFactory>
    {
        private readonly ISalesTaxAdminService m_salesTaxService;

        public TestController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            ISalesTaxAdminService salesTaxService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            m_salesTaxService = salesTaxService ?? throw new ArgumentNullException(nameof(salesTaxService));
        }

        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for polymorphism.")]
        public ActionResult Index(TestCreateTestsModel model)
        {
            _ = new Random();

            //await TestRequestProcessJob.Create(m_logger, m_requestServices.Locale.GetUtcNow(), "random", model.TestCount).ExecuteAsync(m_requestServices.Configuration);

            return View();
        }

        public ActionResult SalesTax()
        {
            var model = new TestSalesTaxModel();
            return View(model);
        }

        [HttpPost]
#pragma warning disable IDE0060 // Remove unused parameter
        public async Task<ActionResult> SalesTax(TestSalesTaxModel model)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var svcRequest = new ASalesTax_LookupSalesTax()
            {
                Address = "17340 West 156 Terrace",
                City = "OLATHE",
                PostalCode = "66062",
                PaymentDate = new DateTime(2016, 10, 15)
            };
            var svcResponse = await m_salesTaxService.LookupSalesTaxAsync(svcRequest);

            ModelState.Clear();

            model = new TestSalesTaxModel()
            {
                SalesTax = svcResponse.SalesTax,
                SalesTaxJurisdiction = svcResponse.SalesTaxJurisdiction
            };
            return View(model);
        }

    }
}