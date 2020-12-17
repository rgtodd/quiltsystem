//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Text;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Web.Models.Profile
{
    public class ProfileModelFactory : ApplicationModelFactory
    {

        public ProfileDetailModel CreateProfileDetailModel(MUser_User svcProfileDetailData)
        {
            var result = new ProfileDetailModel()
            {
                UserId = svcProfileDetailData.UserId,
                Password = "(Set)",
                Email =
                    FormatOptional(
                        FormatEmail(
                            svcProfileDetailData.Email,
                            svcProfileDetailData.EmailConfirmed)),
                Name =
                    FormatOptional(
                        FormatName(
                            svcProfileDetailData.FirstName,
                            svcProfileDetailData.LastName)),
                NickName = FormatOptional(svcProfileDetailData.NickName),
                WebsiteUrl = FormatOptional(svcProfileDetailData.WebsiteUrl),
                TimeZoneId = svcProfileDetailData.TimeZoneId,
                TimeZoneName = svcProfileDetailData.TimeZoneName,
                ShippingAddressLines =
                    FormatOptional(
                        FormatAddress(
                            svcProfileDetailData.ShippingAddressLine1,
                            svcProfileDetailData.ShippingAddressLine2,
                            svcProfileDetailData.ShippingCity,
                            svcProfileDetailData.ShippingStateCode,
                            FormatPostalCode(svcProfileDetailData.ShippingPostalCode),
                            svcProfileDetailData.ShippingCountryCode))
            };

            return result;
        }

        public ProfileEditShippingAddressModel CreateProfileEditShippingAddressModel(MUser_User svcProfileDetailData)
        {
            var result = new ProfileEditShippingAddressModel()
            {
                AddressLine1 = svcProfileDetailData.ShippingAddressLine1,
                AddressLine2 = svcProfileDetailData.ShippingAddressLine2,
                City = svcProfileDetailData.ShippingCity,
                StateCode = svcProfileDetailData.ShippingStateCode,
                PostalCode = FormatPostalCode(svcProfileDetailData.ShippingPostalCode),
                CountryCode = svcProfileDetailData.ShippingCountryCode
            };

            return result;
        }

        public string FormatPostalCode(string value)
        {
            return !string.IsNullOrEmpty(value)
                ? value.Length == 9
                    ? value.Substring(0, 5) + "-" + value.Substring(5, 4)
                    : value
                : value;
        }

        public string ParsePostalCode(string value)
        {
            return !string.IsNullOrEmpty(value)
                ? value.Length == 10
                    ? value.Substring(0, 5) + value.Substring(6, 4)
                    : value
                : value;
        }

        private string FormatEmail(string email, bool emailConfirmed)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(email))
            {
                _ = sb.Append(email);

                _ = emailConfirmed
                    ? sb.Append(" (Confirmed)")
                    : sb.Append(" (Unconfirmed)");
            }

            return sb.ToString();
        }

    }
}