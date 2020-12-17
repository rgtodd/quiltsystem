//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Domain
{
    public class DomainModelFactory : ApplicationModelFactory
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Avoid future breaking change.")]
        public DomainModel CreateShippingVendorDomainModel(IDomainMicroService domainMicroService)
        {
            var values = new List<DomainValueModel>();
            foreach (var svcValue in domainMicroService.GetDomainValues(domainMicroService.ShippingVendorDomain))
            {
                values.Add(new DomainValueModel()
                {
                    Id = svcValue.Id,
                    Value = svcValue.Value
                });
            }

            return new DomainModel()
            {
                Values = values
            };
        }

    }
}