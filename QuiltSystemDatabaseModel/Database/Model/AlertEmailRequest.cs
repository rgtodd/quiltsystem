//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class AlertEmailRequest
    {
        public long AlertEmailRequestId { get; set; }
        public long AlertId { get; set; }
        public long EmailRequestId { get; set; }

        public virtual Alert Alert { get; set; }
        public virtual EmailRequest EmailRequest { get; set; }
    }
}
