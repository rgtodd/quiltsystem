//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ResourceLibrary
    {
        public ResourceLibrary()
        {
            Resources = new HashSet<Resource>();
        }

        public long ResourceLibraryId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Resource> Resources { get; set; }
    }
}
