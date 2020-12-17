//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using RichTodd.QuiltSystem.Web.Bootstrap;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Message
{
    public class CreateMessage
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public long? OrderId { get; set; }

        [Display(Name = "Subject")]
        [Required]
        [StringLength(1000)]
        public string Subject { get; set; }

        [Display(Name = "Message")]
        [DataType(DataType.MultilineText)]
        [Required]
        [StringLength(5000)]
        [Multiline(10)]
        public string Text { get; set; }

        public IList<SelectListItem> Subjects { get; set; }
    }
}