//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Web.Bootstrap
{
    public interface IBootstrapAddress
    {
        string Name { get; }
        string AddressLine1 { get; }
        string AddressLine2 { get; }
        string City { get; }
        string StateCode { get; }
        string PostalCode { get; }
        string CountryCode { get; }
    }
}
