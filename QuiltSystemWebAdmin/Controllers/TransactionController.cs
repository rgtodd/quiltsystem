//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.Web.Paging;
using RichTodd.QuiltSystem.WebAdmin.Models.Transaction;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class TransactionController : ApplicationController<TransactionModelFactory>
    {
        private ITransactionAdminService TransactionAdminService { get; }

        public TransactionController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            ITransactionAdminService transactionAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            TransactionAdminService = transactionAdminService ?? throw new ArgumentNullException(nameof(transactionAdminService));
        }

        public async Task<ActionResult> Index(string id, string unitOfWork, string source)
        {
            if (!string.IsNullOrEmpty(id))
            {
                throw new NotImplementedException();
            }
            else
            {
                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(unitOfWork, source));

                var model = await GetFulfillmentTransactionListAsync(this.GetPagingState(0));

                return View("List", model);
            }
        }

        public async Task<ActionResult> ListSubmit()
        {
            return await Index(null, null, null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(TransactionList model)
        {
            var pagingStateFilter = ModelFactory.CreatePagingStateFilter(model.Filter.UnitOfWork, model.Filter.Source);
            this.SetPagingState(pagingStateFilter);

            model = await GetFulfillmentTransactionListAsync(this.GetPagingState(0));

            return View("List", model);
        }

        private async Task<TransactionList> GetFulfillmentTransactionListAsync(PagingState pagingState)
        {
            var (unitOfWork, source) = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var aTrnsactionList = await TransactionAdminService.GetTransactionsAsync(
                unitOfWork,
                source != TransactionModelFactory.AnySource ? source : null);

            var model = ModelFactory.CreateTransactionList(aTrnsactionList, pagingState);

            return model;
        }
    }
}