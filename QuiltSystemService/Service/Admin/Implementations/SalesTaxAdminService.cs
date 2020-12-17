//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;

namespace RichTodd.QuiltSystem.Service.Admin.Implementations
{
    internal class SalesTaxAdminService : BaseService, ISalesTaxAdminService
    {
        public SalesTaxAdminService(
            IApplicationRequestServices requestServices,
            ILogger<SalesTaxAdminService> logger)
            : base(requestServices, logger)
        { }

        #region IAdmin_SalesTaxService

        public async Task<ASalesTax_SalesTax> LookupSalesTaxAsync(ASalesTax_LookupSalesTax request)
        {
            using var log = BeginFunction(nameof(SalesTaxAdminService), nameof(LookupSalesTaxAsync), request);
            try
            {
                // HACK: Migrate
                await Task.CompletedTask.ConfigureAwait(false);
                throw new NotSupportedException();
                //var op = new KansasSalesTaxTableLookupOperation(Environment);
                //var opResult = await op.ExecuteAsync(request.City, request.PostalCode, request.PaymentDate);

                //var result = new Admin_SalesTax_ResponseData()
                //{
                //    SalesTax = opResult.SalesTaxRate,
                //    SalesTaxJurisdiction = opResult.SalesTaxJurisdiction
                //};

                //log.LogResult(result);
                //return result;
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
