//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Profile
{
    public class ProfileDetailModel
    {
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Nickname")]
        public string NickName { get; set; }

        [Display(Name = "Website")]
        public string WebsiteUrl { get; set; }

        [Display(Name = "Shipping Address")]
        public IReadOnlyList<string> ShippingAddressLines { get; set; }

        [Display(Name = "Time Zone")]
        public string TimeZoneName { get; set; }

        public string TimeZoneId { get; set; }
    }
}