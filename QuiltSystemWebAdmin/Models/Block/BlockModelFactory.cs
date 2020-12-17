//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;

using PagedList;

using RichTodd.QuiltSystem.Resources.Web;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Block
{
    public class BlockModelFactory : ApplicationModelFactory
    {
        public BlockList CreateBlockList(IList<MDesign_Block> mBlocks, IList<string> allTags, PagingState pagingState)
        {
            var summaries = mBlocks.Select(r => CreateBlockListItem(r)).ToList();

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedSummaries = sortFunction != null
                ? pagingState.Descending
                    ? summaries.OrderByDescending(sortFunction).ToList()
                    : summaries.OrderBy(sortFunction).ToList()
                : summaries;

            var pageSize = PagingState.PageSizeHuge;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedSummaries.Count, pageSize);
            var pagedSummaries = sortedSummaries.ToPagedList(pageNumber, pageSize);

            (var selectedTags, var recordCount) = ParsePagingStateFilter(pagingState.Filter);

            var tags = allTags
                .OrderBy(r => r)
                .Select(r => new BlockListFilterTagItem() { TagName = r, Selected = selectedTags.Contains(r) })
                .ToList();

            var model = new BlockList()
            {
                Items = pagedSummaries,
                Filter = new BlockListFilter()
                {
                    RecordCount = recordCount,

                    RecordCountList = CreateRecordCountList(),
                    Tags = tags
                }
            };

            return model;
        }

        public BlockListItem CreateBlockListItem(MDesign_Block mBlock)
        {
            var model = new BlockListItem(mBlock);

            return model;
        }

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.BlockId);
        }

        public string CreatePagingStateFilter(IList<string> tags, int recordCount)
        {
            string tagValue = tags != null && tags.Count > 0
                ? tags.Aggregate((values, value) => string.IsNullOrEmpty(values) ? value : $"{values},{value}")
                : string.Empty;

            return $"{tagValue}|{recordCount}";
        }

        public string CreatePagingStateFilter(BlockListFilter blockListFilter)
        {
            return CreatePagingStateFilter(
                blockListFilter.Tags.Where(r => r.Selected).Select(r => r.TagName).OrderBy(r => r).ToList(),
                blockListFilter.RecordCount);
        }

        public (IList<string> tags, int recordCount) ParsePagingStateFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return (new List<string>(), DefaultRecordCount);
            }

            var fields = filter.Split('|');
            var tags = fields.Length >= 1 && !string.IsNullOrEmpty(fields[0])
                ? fields[0].Split(',').ToList()
                : new List<string>();

            var recordCount = fields.Length >= 2 && int.TryParse(fields[1], out var recordCountField)
                ? recordCountField
                : DefaultRecordCount;

            return (tags, recordCount);
        }

        private ModelMetadata<BlockListItem> m_listItemMetadata;
        private ModelMetadata<BlockListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<BlockListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<BlockListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<BlockListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var heading = new BlockListItem(null);

                    var sortFunctions = new Dictionary<string, Func<BlockListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.BlockId), r => r.BlockId },
                        { ListItemMetadata.GetDisplayName(m => m.BlockName), r => r.BlockName }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<BlockListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort)
                ? SortFunctions[sort]
                : null;
        }
    }
}