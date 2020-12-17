﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Web.Mvc.Models
{
    public class ErrorFieldVcModel
    {
#pragma warning disable IDE1006 // Naming Styles
        public string className
        {
            get { return nameof(ErrorFieldVcModel); }
            set { if (value != nameof(ErrorFieldVcModel)) throw new ArgumentException("Value does not match class name."); }
        }

        public string fieldName { get; set; }
        public string message { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
