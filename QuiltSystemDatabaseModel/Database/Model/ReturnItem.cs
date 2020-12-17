//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ReturnItem
    {
        public long ReturnItemId { get; set; }
        public long ReturnId { get; set; }
        public long ReturnRequestItemId { get; set; }
        public int Quantity { get; set; }

        public virtual Return Return { get; set; }
        public virtual ReturnRequestItem ReturnRequestItem { get; set; }
    }
}
