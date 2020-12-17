//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web.Bootstrap;

namespace RichTodd.QuiltSystem.Web
{
    public static class BootstrapAddressWrapper
    {
        public static IBootstrapAddress Wrap(MCommon_Address mAddress)
        {
            return mAddress != null
                ? new MCommon_Address_Wrapper()
                {
                    MAddress = mAddress
                }
                : null;
        }

        private class MCommon_Address_Wrapper : IBootstrapAddress
        {
            public MCommon_Address MAddress { get; set; }

            public string Name => MAddress.Name;

            public string AddressLine1 => MAddress.AddressLine1;

            public string AddressLine2 => MAddress.AddressLine2;

            public string City => MAddress.City;

            public string StateCode => MAddress.StateCode;

            public string PostalCode => MAddress.PostalCode;

            public string CountryCode => MAddress.CountryCode;
        }
    }
}
