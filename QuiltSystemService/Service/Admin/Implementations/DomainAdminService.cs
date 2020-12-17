//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;

namespace RichTodd.QuiltSystem.Service.Admin.Implementations
{
    internal class DomainAdminService : BaseService, IDomainAdminService
    {
        private IDomainMicroService DomainMicroService { get; }

        public DomainAdminService(
            IApplicationRequestServices requestServices,
            ILogger<IDomainMicroService> logger,
            IDomainMicroService domainMicroService)
            : base(requestServices, logger)
        {
            DomainMicroService = domainMicroService ?? throw new ArgumentNullException(nameof(domainMicroService));
        }

        #region IAdmin_DomainService

        public async Task<ADomain_LedgerAccountTypeList> GetLedgerAccountTypesAsync()
        {
            using var log = BeginFunction(nameof(DomainAdminService), nameof(GetLedgerAccountTypesAsync));
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mLedgerAccountEntries = await DomainMicroService.GetLedgerAccountTypesAsync();

                var result = new ADomain_LedgerAccountTypeList();

                var entries = new List<ADomain_LedgerAccountType>();
                foreach (var mLedgerAccountEntry in mLedgerAccountEntries)
                {
                    var entry = new ADomain_LedgerAccountType()
                    {
                        LedgerAccountTypeId = mLedgerAccountEntry.LedgerAccountNumber,
                        Name = mLedgerAccountEntry.Name,
                        DebitCreditCode = mLedgerAccountEntry.DebitCreditCode
                    };
                    entries.Add(entry);
                }
                result.LedgerAccountTypes = entries;

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
