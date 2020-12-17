//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Owner
    {
        public Owner()
        {
            Designs = new HashSet<Design>();
            Projects = new HashSet<Project>();
        }

        public long OwnerId { get; set; }
        public string OwnerReference { get; set; }
        public string OwnerTypeCode { get; set; }

        public virtual ICollection<Design> Designs { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}
