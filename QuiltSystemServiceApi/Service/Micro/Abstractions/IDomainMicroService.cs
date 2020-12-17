//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions
{
    public interface IDomainMicroService
    {
        string ShippingVendorDomain { get; }
        string StateDomain { get; }

        IEnumerable<MDomain_ValueData> GetDomainValues(string domain);
        IEnumerable<MDomain_TimeZone> GetTimeZoneInfoList();
        IEnumerable<MDomain_WebsiteValue> GetWebsiteValues();
        Task<IReadOnlyList<MDomain_LedgerAccountType>> GetLedgerAccountTypesAsync();
    }
}
