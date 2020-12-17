//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Resource
    {
        public Resource()
        {
            ResourceTags = new HashSet<ResourceTag>();
        }

        public long ResourceId { get; set; }
        public long ResourceLibraryId { get; set; }
        public int ResourceTypeId { get; set; }
        public string Name { get; set; }
        public string ResourceData { get; set; }

        public virtual ResourceLibrary ResourceLibrary { get; set; }
        public virtual ResourceType ResourceType { get; set; }
        public virtual ICollection<ResourceTag> ResourceTags { get; set; }
    }
}
