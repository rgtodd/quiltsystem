//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Web.Mvc.Models;

namespace RichTodd.QuiltSystem.Web.Models.Kit
{
    public class KitEditModel
    {
        public KitDetailVcModel Detail { get; set; }

        public KitSpecificationEditModel Specification { get; set; }

        //
        // Unbound Properties
        //

        public string JsonError { get; set; }

        public string ErrorMessage { get; set; }
    }
}