//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class UserProperty
    {
        public long UserPropertyId { get; set; }
        public long UserProfileId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }

        public virtual UserProfile UserProfile { get; set; }
    }
}
