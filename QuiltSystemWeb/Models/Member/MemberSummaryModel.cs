//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Web.Models.Member
{
    public class MemberSummaryModel
    {

        public IList<MemberDesignSummaryModel> DesignSummaries { get; set; }
        public IList<MemberProjectSummaryModel> ProjectSummaries { get; set; }

    }
}