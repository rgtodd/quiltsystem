//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class TagType
    {
        public TagType()
        {
            Tags = new HashSet<Tag>();
        }

        public string TagTypeCode { get; set; }
        public string TagCategoryCode { get; set; }
        public string Name { get; set; }

        public virtual TagCategory TagCategoryCodeNavigation { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
