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
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Service.User.Abstractions.Data;
using RichTodd.QuiltSystem.Web.Models.Common;
using RichTodd.QuiltSystem.Web.Mvc.Models;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.Web.Models.Kit
{
    public class KitModelFactory : ApplicationModelFactory
    {

        private const string VALUE_CUSTOM = "CUSTOM";
        private const string VALUE_NONE = "NONE";
        private const string ZERO_INCHES = @"0""";
        //private static readonly List<CommonColorModel> s_colorModels;
        private static IDictionary<string, Func<KitSummaryModel, object>> s_sortFunctions;
        //private readonly Mvc.Models.Kit.KitModelFactory m_mvcKitModelFactory = new Mvc.Models.Kit.KitModelFactory();

        private ModelMetadata<KitSummaryModel> m_kitSummaryModelMetadata;

        private ModelMetadata<KitSummaryModel> KitSummaryModelMetadata
        {
            get
            {
                if (m_kitSummaryModelMetadata == null)
                {
                    m_kitSummaryModelMetadata = ModelMetadata<KitSummaryModel>.Create(HttpContext);
                }

                return m_kitSummaryModelMetadata;
            }
        }

        private IDictionary<string, Func<KitSummaryModel, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var sortFunctions = new Dictionary<string, Func<KitSummaryModel, object>>
                    {
                        { KitSummaryModelMetadata.GetDisplayName(m => m.KitName), r => r.KitName }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        public KitEditModel CreateKitEditModel(MKit_Kit mKit)
        {
            var specification = CreateKitSpecificationEditModel(mKit);

            var detail = new KitEditModel()
            {
                Detail = KitDetailVcModel.Create(mKit),
                Specification = specification
            };
            return detail;
        }

        public KitSummaryListModel CreateKitSummaryListModel(UProject_ProjectSummaryList svcKits, PagingState pagingState)
        {
            var kitSummaries = new List<KitSummaryModel>();
            foreach (var svcKitSummary in svcKits.ProjectSummaries)
            {
                var summary = new KitSummaryModel()
                {
                    KitId = svcKitSummary.ProjectId,
                    KitName = svcKitSummary.ProjectName
                };

                kitSummaries.Add(summary);
            }

            IReadOnlyList<KitSummaryModel> sortedKitSummaries;
            var sortFunction = GetSortFunction(pagingState.Sort);
            sortedKitSummaries = sortFunction != null
                ? pagingState.Descending
                    ? kitSummaries.OrderByDescending(sortFunction).ToList()
                    : kitSummaries.OrderBy(sortFunction).ToList()
                : kitSummaries;

            int pageSize = 11;
            int pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedKitSummaries.Count, pageSize);
            var pagedKitSummaries = sortedKitSummaries.ToPagedList(pageNumber, pageSize);

            var model = new KitSummaryListModel()
            {
                KitSummaries = pagedKitSummaries,
                HasDeletedKits = svcKits.HasDeletedProjects,
                Filter = pagingState.Filter,
                Filters = new List<SelectListItem>
                {
                    new SelectListItem() { Text = "All", Value= "All" }
                },
                RenameKit = new KitRenameModel()
            };

            return model;
        }

        //private async Task<List<CommonColorModel>> CreateColorModelsAsync(IInventoryService inventoryService)
        //{
        //    if (s_colorModels == null)
        //    {
        //        var colorModels = new List<CommonColorModel>();
        //        {
        //            var inventoryItems = await inventoryService.GetInventoryItemsAsync();
        //            foreach (var inventoryItem in inventoryItems)
        //            {
        //                var colorModel = new CommonColorModel()
        //                {
        //                    Sku = inventoryItem.Sku,
        //                    Name = inventoryItem.Name,
        //                    WebColor = inventoryItem.Color.WebColor
        //                };
        //                colorModels.Add(colorModel);
        //            }
        //        }

        //        s_colorModels = colorModels;
        //    }

        //    return s_colorModels;
        //}

        private CommonColorModel CreateCommonColorModel(MCommon_FabricStyle svcFabricStyle)
        {
            return new CommonColorModel()
            {
                Sku = svcFabricStyle.Sku,
                Name = svcFabricStyle.Name,
                WebColor = svcFabricStyle.Color.WebColor
            };
        }

        private KitSpecificationEditModel CreateKitSpecificationEditModel(MKit_Kit kit)
        {
            var sizeListItems = GetSizeListItems(kit);

            // Determine selected list item.
            //
            var sizeListItemValue = VALUE_CUSTOM; // Assume custom value.
            foreach (var svcStandardSize in kit.SpecificationOptions.StandardSizes)
            {
                if (svcStandardSize.Width == kit.Specification.Width && svcStandardSize.Height == kit.Specification.Height)
                {
                    sizeListItemValue = svcStandardSize.Id;
                    break;
                }
            }

            var borderWidthListItems = GetBorderWidthListItems(kit);

            // Determine selected list item.
            //
            string borderWidthListItemValue;
            if (kit.Specification.BorderWidth == ZERO_INCHES)
            {
                borderWidthListItemValue = VALUE_NONE;
            }
            else
            {
                borderWidthListItemValue = VALUE_CUSTOM; // Assume custom value.
                foreach (var svcStandardDimension in kit.SpecificationOptions.StandardBorderWidths)
                {
                    if (svcStandardDimension.Length == kit.Specification.BorderWidth)
                    {
                        borderWidthListItemValue = svcStandardDimension.Id;
                        break;
                    }
                }
            }

            var bindingWidthListItems = GetBindingWidthListItems(kit);

            // Determine selected list item.
            //
            string bindingWidthListItemValue;
            if (kit.Specification.BindingWidth == ZERO_INCHES)
            {
                bindingWidthListItemValue = VALUE_NONE;
            }
            else
            {
                bindingWidthListItemValue = VALUE_CUSTOM; // Assume custom value.
                foreach (var svcStandardDimension in kit.SpecificationOptions.StandardBindingWidths)
                {
                    if (svcStandardDimension.Length == kit.Specification.BindingWidth)
                    {
                        bindingWidthListItemValue = svcStandardDimension.Id;
                        break;
                    }
                }
            }

            var specification = new KitSpecificationEditModel()
            {
                Sizes = sizeListItems,
                Size = sizeListItemValue,
                //UseCustomSize = sizeListItemValue == VALUE_CUSTOM,
                CustomSizeWidth = kit.Specification.Width,
                CustomSizeHeight = kit.Specification.Height,

                BorderFabricStyle = CreateCommonColorModel(kit.Specification.BorderFabricStyle),
                BorderWidths = borderWidthListItems,
                BorderWidth = borderWidthListItemValue,
                //UseCustomBorderWidth = borderWidthListItemValue == VALUE_CUSTOM,
                CustomBorderWidth = kit.Specification.BorderWidth,
                //HasBorder = kit.Specification.BorderWidth != ZERO_INCHES,

                BindingFabricStyle = CreateCommonColorModel(kit.Specification.BindingFabricStyle),
                BindingWidths = bindingWidthListItems,
                BindingWidth = bindingWidthListItemValue,
                //UseCustomBindingWidth = bindingWidthListItemValue == VALUE_CUSTOM,
                CustomBindingWidth = kit.Specification.BindingWidth,
                //HasBinding = kit.Specification.BindingWidth != ZERO_INCHES,

                BackingFabricStyle = CreateCommonColorModel(kit.Specification.BackingFabricStyle),
                HasBacking = kit.Specification.HasBacking,

                TrimTriangles = kit.Specification.TrimTriangles
            };

            return specification;
        }

        private List<SelectListItem> GetBindingWidthListItems(MKit_Kit kit)
        {
            var standardBindingWidths = new List<SelectListItem>();
            {
                standardBindingWidths.Add(new SelectListItem()
                {
                    Text = "No Border",
                    Value = VALUE_NONE
                });

                foreach (var svcStandardDimension in kit.SpecificationOptions.StandardBindingWidths)
                {
                    standardBindingWidths.Add(new SelectListItem()
                    {
                        Text = svcStandardDimension.Length,
                        Value = svcStandardDimension.Id
                    });
                }

                standardBindingWidths.Add(new SelectListItem()
                {
                    Text = "Custom Size",
                    Value = VALUE_CUSTOM
                });
            }

            return standardBindingWidths;
        }

        private List<SelectListItem> GetBorderWidthListItems(MKit_Kit kit)
        {
            var standardBorderWidths = new List<SelectListItem>();
            {
                standardBorderWidths.Add(new SelectListItem()
                {
                    Text = "No Border",
                    Value = VALUE_NONE
                });

                foreach (var svcStandardDimension in kit.SpecificationOptions.StandardBorderWidths)
                {
                    standardBorderWidths.Add(new SelectListItem()
                    {
                        Text = svcStandardDimension.Length,
                        Value = svcStandardDimension.Id
                    });
                }

                standardBorderWidths.Add(new SelectListItem()
                {
                    Text = "Custom Size",
                    Value = VALUE_CUSTOM
                });
            }

            return standardBorderWidths;
        }

        private List<SelectListItem> GetSizeListItems(MKit_Kit kit)
        {
            var standardSizes = new List<SelectListItem>();
            {
                foreach (var svcStandardSize in kit.SpecificationOptions.StandardSizes)
                {
                    standardSizes.Add(new SelectListItem()
                    {
                        Text = svcStandardSize.Width + " x " + svcStandardSize.Height + " (" + svcStandardSize.Description + ")",
                        Value = svcStandardSize.Id
                    });
                }

                standardSizes.Add(new SelectListItem()
                {
                    Text = "Custom Size",
                    Value = VALUE_CUSTOM
                });
            }

            return standardSizes;
        }

        private Func<KitSummaryModel, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort) ? SortFunctions[sort] : null;
        }

    }
}