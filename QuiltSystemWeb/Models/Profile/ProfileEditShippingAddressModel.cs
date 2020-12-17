//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace RichTodd.QuiltSystem.Web.Models.Profile
{
    public class ProfileEditShippingAddressModel
    {
        [Display(Name = "Address")]
        [Required]
        [StringLength(100)]
        public string AddressLine1 { get; set; }

        [StringLength(100)]
        public string AddressLine2 { get; set; }

        [Display(Name = "City")]
        [Required]
        [StringLength(50)]
        public string City { get; set; }

        [Display(Name = "State")]
        [Required]
        [StringLength(2)]
        public string StateCode { get; set; }

        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        [Required]
        [StringLength(10)]
        public string PostalCode { get; set; }

        [Display(Name = "Country")]
        [StringLength(2)]
        public string CountryCode { get; set; }

        public IList<SelectListItem> StateCodes { get; set; }
    }
}