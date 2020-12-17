//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Domain
{
    public class DomainValueModel
    {
        [Display(Name = "ID")]
        public string Id { get; set; }

        [Display(Name = "Value")]
        public string Value { get; set; }
    }
}