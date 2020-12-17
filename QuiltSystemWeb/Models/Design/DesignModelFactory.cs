//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Resources.Web;
using RichTodd.QuiltSystem.Service.User.Abstractions.Data;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.Web.Models.Design
{
    public class DesignModelFactory : ApplicationModelFactory
    {

        private static IDictionary<string, Func<DesignSummaryModel, object>> s_sortFunctions;

        private ModelMetadata<DesignSummaryModel> m_designSummaryModelMetadata;

        private ModelMetadata<DesignSummaryModel> DesignSummaryModelMetadata
        {
            get
            {
                if (m_designSummaryModelMetadata == null)
                {
                    m_designSummaryModelMetadata = ModelMetadata<DesignSummaryModel>.Create(HttpContext);
                }

                return m_designSummaryModelMetadata;
            }
        }

        private IDictionary<string, Func<DesignSummaryModel, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var sortFunctions = new Dictionary<string, Func<DesignSummaryModel, object>>
                    {
                        { DesignSummaryModelMetadata.GetDisplayName(m => m.DesignName), r => r.DesignName }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        public DesignDetailModel CreateDesignDetailModel(Guid? designid)
        {
            var model = new DesignDetailModel()
            {
                DesignId = designid == Guid.Empty ? "" : designid.Value.ToString()
            };

            return model;
        }

        public DesignSummaryListModel CreateDesignSummaryListModel(UDesign_DesignSummaryList svcDesigns, PagingState pagingState)
        {
            var designSummaries = new List<DesignSummaryModel>();
            foreach (var svcDesignSummary in svcDesigns.Summaries)
            {
                var designSummary = new DesignSummaryModel()
                {
                    DesignId = svcDesignSummary.DesignId,
                    DesignName = svcDesignSummary.DesignName
                };

                if (string.IsNullOrEmpty(designSummary.DesignName))
                {
                    designSummary.DesignName = "-";
                }

                designSummaries.Add(designSummary);
            }

            IList<DesignSummaryModel> sortedDesignSummaries;
            var sortFunction = GetSortFunction(pagingState.Sort);
            sortedDesignSummaries = sortFunction != null
                ? pagingState.Descending
                    ? designSummaries.OrderByDescending(sortFunction).ToList()
                    : designSummaries.OrderBy(sortFunction).ToList()
                : designSummaries;

            int pageSize = 11;
            int pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedDesignSummaries.Count, pageSize);
            var pagedDesignSummaries = sortedDesignSummaries.ToPagedList(pageNumber, pageSize);

            var model = new DesignSummaryListModel()
            {
                DesignSummaries = pagedDesignSummaries,
                HasDeletedDesigns = svcDesigns.HasDeletedDesigns,
                Filter = pagingState.Filter,
                Filters = new List<SelectListItem>
                {
                    new SelectListItem() { Text = "All", Value= "All" }
                },
                CreateDesign = new DesignCreateModel(),
                RenameDesign = new DesignRenameModel()
            };

            return model;
        }

        private Func<DesignSummaryModel, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort) ? SortFunctions[sort] : null;
        }

    }
}