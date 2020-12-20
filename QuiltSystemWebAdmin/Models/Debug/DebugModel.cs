//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Service.Core.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Database.Abstractions.Data;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Debug
{
    public class DebugModel
    {
        [Display(Name = "Square Webhook Transaction ID")]
        public long? SquareWebookTransactionId { get; set; }

        public ApplicationOptions ApplicationOptions { get; set; }

        public DatabaseOptions DatabaseOptions { get; set; }
    }
}
