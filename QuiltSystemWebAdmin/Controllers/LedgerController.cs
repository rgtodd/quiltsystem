//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Security;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.WebAdmin.Models.Ledger;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    [Authorize(Policy = ApplicationPolicies.AllowViewFinancial)]
    public class LedgerController : ApplicationController<LedgerModelFactory>
    {
        private ILedgerMicroService LedgerMicroService { get; }

        public LedgerController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            ILedgerMicroService ledgerMicroService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            LedgerMicroService = ledgerMicroService ?? throw new ArgumentNullException(nameof(ledgerMicroService));
        }

        public async Task<ActionResult> Index(long? id)
        {
            if (id.HasValue)
            {
                var mLedgerTransaction = await LedgerMicroService.GetLedgerTransactionAsync(id.Value);
                var model = ModelFactory.CreateLedgerTransaction(mLedgerTransaction);

                return View("Detail", model);
            }
            else
            {
                //this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(null, null, null, ModelFactory.DefaultRecordCount));

                var model = await GetLedgerTransactionListAsync();

                return View("List", model);
            }
        }

        public async Task<ActionResult> Summary()
        {
            var accountingYear = Locale.GetLocalNow().Year;

            var mSummaryList = await LedgerMicroService.GetLedgerAccountSummariesAsync(accountingYear);
            var model = ModelFactory.CreateLedgerSummary(mSummaryList);

            return View(model);
        }

        public Task<ActionResult> ListSubmit()
        {
            return Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(LedgerTransactionList model)
        {
            this.SetPagingState(ModelFactory.CreatePagingStateFilter(model.Filter));

            model = await GetLedgerTransactionListAsync();

            return View("List", model);
        }

        private async Task<LedgerTransactionList> GetLedgerTransactionListAsync()
        {
            var pagingState = this.GetPagingState(0);

            var (postDate, ledgerAccountNumber, unitOfWork, recordCount) = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var mLedgerTransactionList = await LedgerMicroService.GetLedgerTransactionsAsync(postDate, ledgerAccountNumber, unitOfWork, recordCount);

            var model = ModelFactory.CreateLedgerTransactionList(mLedgerTransactionList.Transactions, pagingState);

            return model;
        }
    }
}