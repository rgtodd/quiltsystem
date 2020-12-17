//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class TopicField
    {
        public long TopicId { get; set; }
        public string FieldCode { get; set; }
        public string FieldValue { get; set; }

        public virtual Topic Topic { get; set; }
    }
}
