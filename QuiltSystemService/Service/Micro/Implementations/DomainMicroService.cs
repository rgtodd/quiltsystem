//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class DomainMicroService : MicroService, IDomainMicroService
    {
        private const string s_shippingVendorDomain = "ShippingVendor";
        private const string s_stateDomain = "State";

        public DomainMicroService(
            IApplicationLocale locale,
            ILogger<DesignMicroService> logger,
            IQuiltContextFactory quiltContextFactory)
            : base(
                  locale,
                  logger,
                  quiltContextFactory)
        { }

        public string ShippingVendorDomain
        {
            get
            {
                return s_shippingVendorDomain;
            }
        }

        public string StateDomain
        {
            get
            {
                return s_stateDomain;
            }
        }

        public IEnumerable<MDomain_ValueData> GetDomainValues(string domain)
        {
            using var log = BeginFunction(nameof(DomainMicroService), nameof(GetDomainValues), domain);
            try
            {
                switch (domain)
                {
                    case s_shippingVendorDomain:
                        {
                            var result = new List<MDomain_ValueData>();

                            using (var ctx = QuiltContextFactory.Create())
                            {
                                foreach (var dbShippingVendor in ctx.ShippingVendors.OrderBy(r => r.Name))
                                {
                                    result.Add(new MDomain_ValueData()
                                    {
                                        Id = dbShippingVendor.ShippingVendorId.ToString(),
                                        Value = dbShippingVendor.Name
                                    });
                                }
                            }

                            log.Result(result);
                            return result;
                        }

                    case s_stateDomain:
                        {
                            var result = new List<MDomain_ValueData>();

                            using (var ctx = QuiltContextFactory.Create())
                            {
                                foreach (var dbState in ctx.States.OrderBy(r => r.Name))
                                {
                                    result.Add(new MDomain_ValueData()
                                    {
                                        Id = dbState.StateCode,
                                        Value = dbState.Name
                                    });
                                }
                            }

                            log.Result(result);
                            return result;
                        }
                }

                throw new InvalidOperationException(string.Format("Unknown domain value {0}", domain));
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<IReadOnlyList<MDomain_LedgerAccountType>> GetLedgerAccountTypesAsync()
        {
            using var log = BeginFunction(nameof(DomainMicroService), nameof(GetLedgerAccountTypesAsync));
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var entries = new List<MDomain_LedgerAccountType>();

                foreach (var dbLedgerAccount in await ctx.LedgerAccounts.ToListAsync())
                {
                    var entry = new MDomain_LedgerAccountType()
                    {
                        LedgerAccountNumber = dbLedgerAccount.LedgerAccountNumber,
                        Name = dbLedgerAccount.Name,
                        DebitCreditCode = dbLedgerAccount.DebitCreditCode
                    };
                    entries.Add(entry);
                }

                var result = entries;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public IEnumerable<MDomain_TimeZone> GetTimeZoneInfoList()
        {
            using var log = BeginFunction(nameof(DomainMicroService), nameof(GetTimeZoneInfoList));
            try
            {
                var timeZoneInfoList = new List<MDomain_TimeZone>();

                foreach (var timeZone in TimeZoneInfo.GetSystemTimeZones())
                {
                    var timeZoneInfo = new MDomain_TimeZone()
                    {
                        TimeZoneId = timeZone.Id,
                        Name = timeZone.DisplayName
                    };
                    timeZoneInfoList.Add(timeZoneInfo);
                }

                log.Result(timeZoneInfoList);
                return timeZoneInfoList;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public IEnumerable<MDomain_WebsiteValue> GetWebsiteValues()
        {
            using var log = BeginFunction(nameof(DomainMicroService), nameof(GetWebsiteValues));
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var result = new List<MDomain_WebsiteValue>();
                foreach (var dbWebsiteProperty in ctx.WebsiteProperties)
                {
                    result.Add(new MDomain_WebsiteValue()
                    {
                        PropertyName = dbWebsiteProperty.PropertyName,
                        PropertyValue = dbWebsiteProperty.PropertyValue
                    });
                }

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }
    }
}
