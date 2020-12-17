//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class UserProfile
    {
        public UserProfile()
        {
            UserAddresses = new HashSet<UserAddress>();
            UserProperties = new HashSet<UserProperty>();
        }

        public long UserProfileId { get; set; }
        public string UserProfileReference { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public string WebsiteUrl { get; set; }
        public string TimeZoneId { get; set; }

        public virtual UserProfileAspNetUser UserProfileAspNetUser { get; set; }
        public virtual ICollection<UserAddress> UserAddresses { get; set; }
        public virtual ICollection<UserProperty> UserProperties { get; set; }
    }
}
