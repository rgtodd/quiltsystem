//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Web.Mvc.Models
{
    public class ErrorVcModel
    {
#pragma warning disable IDE1006 // Naming Styles
        public string className
        {
            get { return nameof(ErrorVcModel); }
            set { if (value != nameof(ErrorVcModel)) throw new ArgumentException("Value does not match class name."); }
        }

        public ErrorPageVcModel[] pageErrors { get; set; }
        public ErrorFieldVcModel[] fieldErrors { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
