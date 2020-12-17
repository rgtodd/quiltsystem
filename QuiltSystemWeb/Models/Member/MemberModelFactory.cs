//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.User.Abstractions.Data;

namespace RichTodd.QuiltSystem.Web.Models.Member
{
    public class MemberModelFactory : ApplicationModelFactory
    {

        public MemberSummaryModel CreateMemberSummaryModel(UDesign_DesignSummaryList svcDesigns, UProject_ProjectSummaryList svcProjects)
        {
            var designSummaries = new List<MemberDesignSummaryModel>();
            foreach (var svcDesignSummary in svcDesigns.Summaries)
            {
                var designSummary = new MemberDesignSummaryModel()
                {
                    DesignId = svcDesignSummary.DesignId,
                    DesignName = svcDesignSummary.DesignName
                };

                designSummaries.Add(designSummary);
            }

            var projectSummaries = new List<MemberProjectSummaryModel>();
            foreach (var svcProjectSummary in svcProjects.ProjectSummaries)
            {
                var projectSummary = new MemberProjectSummaryModel()
                {
                    ProjectId = svcProjectSummary.ProjectId,
                    ProjectName = svcProjectSummary.ProjectName
                };

                projectSummaries.Add(projectSummary);
            }

            var model = new MemberSummaryModel()
            {
                DesignSummaries = designSummaries,
                ProjectSummaries = projectSummaries
            };

            return model;
        }

    }
}