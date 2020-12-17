//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class OrderNumber
    {
        public DateTime OrderDateUtc { get; set; }
        public int Number { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
