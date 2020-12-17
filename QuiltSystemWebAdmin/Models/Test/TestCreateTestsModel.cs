//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Test
{
    public class TestCreateTestsModel
    {
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StartingDateTime { get; set; }

        public int TestCount { get; set; }

        public int MinMinuteInterval { get; set; }

        public int MaxMinuteInterval { get; set; }
    }
}