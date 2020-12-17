//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class UserProfileAspNetUser
    {
        public long UserProfileId { get; set; }
        public string AspNetUserId { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
