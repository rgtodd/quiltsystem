//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Business.ComponentProviders;
using RichTodd.QuiltSystem.Design.Component;
using RichTodd.QuiltSystem.Design.Component.Standard;
using RichTodd.QuiltSystem.Design.Core;
using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Nodes.Standard;
using RichTodd.QuiltSystem.Design.Path;
using RichTodd.QuiltSystem.Design.Primitives;
using RichTodd.QuiltSystem.Service.Ajax.Abstractions;
using RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Ajax.Implementations
{
    internal class DesignAjaxService : BaseService, IDesignAjaxService
    {
        private static XDesign_Block[] s_blocks;

        private IDesignMicroService DesignMicroService { get; }

        public DesignAjaxService(
            IApplicationRequestServices requestServices,
            ILogger<DesignAjaxService> logger,
            IDesignMicroService designMicroService)
            : base(requestServices, logger)
        {
            DesignMicroService = designMicroService ?? throw new ArgumentNullException(nameof(designMicroService));
        }

        public async Task<XDesign_Block[]> GetBlocksAsync(int size)
        {
            using var log = BeginFunction(nameof(DesignAjaxService), nameof(GetBlocksAsync));
            try
            {
                var result = await GetCachedBlocks(size);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<XDesign_Design> GetDesignAsync(string userId, string designId)
        {
            using var log = BeginFunction(nameof(DesignAjaxService), nameof(GetDesignAsync), userId, designId);
            try
            {
                if (string.IsNullOrEmpty(designId))
                {
                    var design = DesignFactory.CreateEmptyDesign();

                    var result = Create.XDesign_Design(design);

                    log.Result(result);
                    return result;
                }
                else
                {
                    var mDesign = await DesignMicroService.GetDesignAsync(Guid.Parse(designId)).ConfigureAwait(false);
                    if (mDesign == null)
                    {
                        log.Result(null);
                        return null;
                    }

                    var result = Create.XDesign_Design(mDesign);

                    log.Result(result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<XDesign_DesignInfo> GetDesignInfo(XDesign_Design design, int designSize, int layoutSize, int blockSize)
        {
            using var log = BeginFunction(nameof(DesignAjaxService), nameof(GetDesignInfo), design);
            try
            {
                await Task.CompletedTask.ConfigureAwait(false);

                var projectObject = BusinessDataFactory.CreateDesign(design);

                var result = Create.XDesign_DesignInfo(projectObject, designSize, layoutSize, blockSize);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<XDesign_FabricStyleCatalog> GetFabricStyleCatalogAsync()
        {
            using var log = BeginFunction(nameof(DesignAjaxService), nameof(GetFabricStyleCatalogAsync));
            try
            {
                var svcFabricStyleCatalogData = await DesignMicroService.GetFabricStyles().ConfigureAwait(false);

                //using var ctx = QuiltContextFactory.Create();

                //var inventoryItems = await (from ii in ctx.InventoryItems
                //                            where ii.InventoryItemTypeCode == InventoryItemTypes.Fabric && (ii.Quantity - ii.ReservedQuantity) > 0
                //                            orderby ii.Hue, ii.Saturation, ii.Value
                //                            select ii).ToListAsync().ConfigureAwait(false)

                //var tags = await (from iit in ctx.InventoryItemTags
                //                  join t in ctx.Tags on iit.TagId equals t.TagId
                //                  select new { iit.InventoryItemId, t.TagTypeCode, t.Value }).ToListAsync().ConfigureAwait(false)

                //var manufacturerNames = new Dictionary<long, string>();
                //var collectionNameSets = new Dictionary<long, List<string>>();
                //foreach (var tag in tags)
                //{
                //    var inventoryItem = inventoryItems.Where(ii => ii.InventoryItemId == tag.InventoryItemId).SingleOrDefault();
                //    if (inventoryItem != null)
                //    {
                //        // Note: assumes an inventory item is associated with only one manufacturer.
                //        //
                //        if (tag.TagTypeCode == TagTypes.Manufacturer)
                //        {
                //            manufacturerNames[tag.InventoryItemId] = tag.Value;
                //        }

                //        if (tag.TagTypeCode == TagTypes.Collection)
                //        {
                //            if (!collectionNameSets.TryGetValue(tag.InventoryItemId, out var collectionNameSet))
                //            {
                //                collectionNameSet = new List<string>();
                //                collectionNameSets[tag.InventoryItemId] = collectionNameSet;
                //            }
                //            collectionNameSet.Add(tag.Value);
                //        }
                //    }
                //}

                //var manufacturers = new Dictionary<string, Design_FabricStyleManufacturerData>();
                //var collectionSets = new Dictionary<Design_FabricStyleManufacturerData, List<Design_FabricStyleCollectionData>>();
                //var fabricSets = new Dictionary<Design_FabricStyleCollectionData, List<Design_FabricStyleData>>();

                //foreach (var inventoryItem in inventoryItems)
                //{
                //    var manufacturerName = manufacturerNames[inventoryItem.InventoryItemId];
                //    var collectionNameSet = collectionNameSets[inventoryItem.InventoryItemId];

                //    if (!manufacturers.TryGetValue(manufacturerName, out var manufacturer))
                //    {
                //        manufacturer = new Design_FabricStyleManufacturerData()
                //        {
                //            manufacturerName = manufacturerName
                //        };
                //        manufacturers[manufacturerName] = manufacturer;

                //        collectionSets[manufacturer] = new List<Design_FabricStyleCollectionData>();
                //    }

                //    var collectionSet = collectionSets[manufacturer];
                //    foreach (var collectionName in collectionNameSet)
                //    {
                //        var collection = collectionSet.Where(r => r.collectionName == collectionName).SingleOrDefault();
                //        if (collection == null)
                //        {
                //            collection = new Design_FabricStyleCollectionData()
                //            {
                //                collectionName = collectionName
                //            };
                //            collectionSet.Add(collection);

                //            fabricSets[collection] = new List<Design_FabricStyleData>();
                //        }

                //        var fabricSet = fabricSets[collection];
                //        var color = Color.FromAhsb(255, inventoryItem.Hue, inventoryItem.Saturation / 100.0, inventoryItem.Value / 100.0);
                //        var fabricStyle = new FabricStyle(inventoryItem.Sku, color);
                //        fabricSet.Add(Implementations.ServiceDataFactory.CreateFabricStyleData(fabricStyle));
                //    }
                //}

                //foreach (var collection in fabricSets.Keys)
                //{
                //    collection.fabricStyles = fabricSets[collection].ToArray();
                //}
                //foreach (var manufacturer in collectionSets.Keys)
                //{
                //    manufacturer.collections = collectionSets[manufacturer].ToArray();
                //}

                var manufacturers = new List<XDesign_FabricStyleManufacturer>();
                foreach (var svcManufacturer in svcFabricStyleCatalogData.Manufacturers)
                {
                    var collections = new List<XDesign_FabricStyleCollection>();
                    foreach (var svcCollection in svcManufacturer.Collections)
                    {
                        var fabricStyles = new List<XDesign_FabricStyle>();
                        foreach (var svcFabricStyle in svcCollection.FabricStyles)
                        {
                            var fabricStyle = new XDesign_FabricStyle()
                            {
                                color = new XDesign_Color()
                                {
                                    webColor = svcFabricStyle.Color.WebColor
                                },
                                sku = svcFabricStyle.Sku
                            };
                            fabricStyles.Add(fabricStyle);
                        }

                        var collection = new XDesign_FabricStyleCollection()
                        {
                            collectionName = svcCollection.CollectionName,
                            fabricStyles = fabricStyles.ToArray()
                        };
                        collections.Add(collection);
                    }

                    var manufacturer = new XDesign_FabricStyleManufacturer()
                    {
                        manufacturerName = svcManufacturer.ManufacturerName,
                        collections = collections.ToArray()
                    };
                    manufacturers.Add(manufacturer);
                }

                var result = new XDesign_FabricStyleCatalog()
                {
                    manufacturers = manufacturers.ToArray()
                };

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<XDesign_Layout[]> GetLayoutsAsync(int rowCount, int columnCount, int size)
        {
            using var log = BeginFunction(nameof(DesignAjaxService), nameof(GetLayoutsAsync));
            try
            {
                await Task.CompletedTask.ConfigureAwait(false);

                var layoutList = new List<XDesign_Layout>();

                var provider = new BuiltInQuiltLayoutComponenProvider();
                foreach (var entry in provider.GetComponents(LayoutComponent.TypeName, Constants.DefaultComponentCategory))
                {
                    var fabricStyles = new FabricStyleList
                    {
                        new FabricStyle(FabricStyle.UNKNOWN_SKU, Color.Red),
                        new FabricStyle(FabricStyle.UNKNOWN_SKU, Color.Green),
                        new FabricStyle(FabricStyle.UNKNOWN_SKU, Color.Blue)
                    };

                    var component = LayoutComponent.Create(entry.Category, entry.Name, fabricStyles, rowCount, columnCount, entry.BlockCount);

                    var data = Create.XDesign_Layout(component, size);
                    layoutList.Add(data);
                }

                var result = layoutList.ToArray();

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<Guid> SaveDesignAsync(string userId, XDesign_Design design)
        {
            using var log = BeginFunction(nameof(DesignAjaxService), nameof(SaveDesignAsync), userId, design);
            try
            {
                var ownerReference = CreateOwnerReference.FromUserId(userId);
                var ownerId = await DesignMicroService.AllocateOwnerAsync(ownerReference).ConfigureAwait(false);

                var result = string.IsNullOrEmpty(design.designId)
                    ? await DesignMicroService.CreateDesignAsync(ownerId, design.designName, BusinessDataFactory.Create_MDesign_DesignSpecification(design), GetUtcNow()).ConfigureAwait(false)
                    : await DesignMicroService.UpdateDesignAsync(Guid.Parse(design.designId), BusinessDataFactory.Create_MDesign_DesignSpecification(design), GetUtcNow()).ConfigureAwait(false);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private async Task<XDesign_Block[]> GetCachedBlocks(int size)
        {
            if (s_blocks == null)
            {
                s_blocks = await GetCachedBlocksSync(size);
            }

            return s_blocks;
        }

        private async Task<XDesign_Block[]> GetCachedBlocksSync(int size)
        {
            var blockList = new List<XDesign_Block>();

            var mBlockCollection = await DesignMicroService.GetBlockCollectionAsync(size);
            foreach (var mBlock in mBlockCollection.Blocks)
            {
                var data = Create.XDesign_Block(mBlock);
                blockList.Add(data);
            }

            return blockList.ToArray();
        }

        //public async Task<ColorData[]> GetColors()
        //{
        //    using var log = BeginFunction(nameof(DesignWebService), nameof(GetColors));
        //    try
        //    {
        //        var colors = new List<ColorData>();

        //        using (var ctx = QuiltContext.Create())
        //        {
        //            var inventoryItems = await (from ii in ctx.InventoryItems
        //                                        where ii.InventoryItemTypeCode == InventoryItemTypes.Fabric && (ii.Quantity - ii.ReservedQuantity) > 0
        //                                        orderby ii.Hue, ii.Saturation, ii.Value
        //                                        select ii).ToListAsync().ConfigureAwait(false)

        //            foreach (var inventoryItem in inventoryItems)
        //            {
        //                var color = ColorUtility.ColorFromAhsb(255, inventoryItem.Hue, inventoryItem.Saturation / 100.0, inventoryItem.Value / 100.0);

        //                colors.Add(ServiceDataFactory.CreateColorData(color));
        //            }
        //        }

        //        var result = colors.ToArray();

        //        log.LogResult(result);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        EndFunction(ex);
        //        throw;
        //    }
        //            //}
        //public async Task<BlockPreviewData> GetBlockPreview(DesignBlockData block)
        //{
        //    using var log = BeginFunction(nameof(DesignWebService), nameof(GetBlockPreview), block);
        //    try
        //    {
        //        var task = new Task<BlockPreviewData>(() =>
        //        {
        //            var blockComponent = BlockComponent.Create(
        //                block.blockCategory,
        //                block.blockName,
        //                BusinessDataFactory.CreateFabricStyleList(block.fabricStyles));

        //            return ServiceDataFactory.CreateBlockPreviewData(blockComponent);
        //        });

        //        task.Start();

        //        var result = await task;

        //        log.LogResult(result);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        EndFunction(ex);
        //        throw;
        //    }
        //            //}

        //public async Task<LayoutPreviewData> GetLayoutPreview(DesignLayoutData layout)
        //{
        //    using var log = BeginFunction(nameof(DesignWebService), nameof(GetLayoutPreview), layout);
        //    try
        //    {
        //        var task = new Task<LayoutPreviewData>(() =>
        //        {
        //            var layoutComponent = LayoutComponent.Create(
        //                layout.layoutCategory,
        //                layout.layoutName,
        //                BusinessDataFactory.CreateFabricStyleList(layout.fabricStyles),
        //                layout.rowCount,
        //                layout.columnCount,
        //                layout.blockCount);

        //            var layoutPreview = ServiceDataFactory.CreateLayoutPreviewData(layoutComponent);

        //            return layoutPreview;
        //        });

        //        task.Start();

        //        var result = await task;

        //        log.LogResult(result);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        EndFunction(ex);
        //        throw;
        //    }
        //            //}

        private static class Create
        {
            public static XDesign_Layout XDesign_Layout(LayoutComponent layoutComponent, int size)
            {
                if (layoutComponent == null)
                {
                    return null;
                }

                var id = layoutComponent.Name.Replace(" ", "").Replace("/", "");

                var result = new XDesign_Layout()
                {
                    id = id,
                    layoutCategory = layoutComponent.Category,
                    layoutName = layoutComponent.Name,
                    fabricStyles = XDesign_FabricStyles(layoutComponent.FabricStyles),
                    rowCount = layoutComponent.RowCount,
                    columnCount = layoutComponent.ColumnCount,
                    blockCount = layoutComponent.BlockCount,
                    preview = XDesign_LayoutPreview(layoutComponent, size)
                };

                return result;
            }

            public static XDesign_Block XDesign_Block(MDesign_Block mBlock)
            {
                var result = new XDesign_Block()
                {
                    id = mBlock.Id,
                    blockCategory = mBlock.Category,
                    blockGroup = mBlock.Group,
                    blockName = mBlock.BlockName,
                    fabricStyles = XDesign_FabricStyles(mBlock.FabricStyles),
                    preview = XDesign_BlockPreview(mBlock.Preview)
                };

                return result;
            }

            public static XDesign_Design XDesign_Design(MDesign_Design mDesign)
            {
                var design = new Design.Core.Design(JToken.Parse(mDesign.DesignArtifactValue));

                var result = new XDesign_Design()
                {
                    designId = mDesign.DesignId.ToString(),
                    designName = mDesign.Name,
                    width = design.Width.ToString(),
                    height = design.Height.ToString(),
                    layout = XDesign_DesignLayout(design.LayoutComponent),
                    blocks = XDesign_DesignBlocks(design.LayoutComponent.Children)
                };

                return result;
            }

            public static XDesign_Design XDesign_Design(Design.Core.Design design)
            {
                var result = new XDesign_Design()
                {
                    designId = "",
                    designName = "New",
                    width = design.Width.ToString(),
                    height = design.Height.ToString(),
                    layout = XDesign_DesignLayout(design.LayoutComponent),
                    blocks = XDesign_DesignBlocks(design.LayoutComponent.Children)
                };

                return result;
            }

            public static XDesign_DesignInfo XDesign_DesignInfo(Design.Core.Design design, int designSize, int layoutSize, int blockSize)
            {
                var result = new XDesign_DesignInfo()
                {
                    preview = XDesign_DesignPreview(design, designSize),
                    layout = XDesign_Layout(design.LayoutComponent, layoutSize),
                    blocks = XDesign_Blocks(design.LayoutComponent.Children, blockSize)
                };

                return result;
            }

            private static XDesign_Block XDesign_Block(BlockComponent blockComponent, int size)
            {
                if (blockComponent == null) throw new ArgumentNullException(nameof(blockComponent));

                var idxLastSlash = blockComponent.Name.LastIndexOf('/');
                var group = idxLastSlash != -1
                    ? blockComponent.Name.Substring(0, idxLastSlash)
                    : "Default";

                var id = blockComponent.Name.Replace(" ", "");

                var result = new XDesign_Block()
                {
                    id = id,
                    blockCategory = blockComponent.Category,
                    blockGroup = group,
                    blockName = blockComponent.Name,
                    fabricStyles = XDesign_FabricStyles(blockComponent.FabricStyles),
                    preview = XDesign_BlockPreview(blockComponent, size)
                };

                return result;
            }

            private static XDesign_FabricStyle XDesign_FabricStyle(FabricStyle fabricStyle)
            {
                var result = new XDesign_FabricStyle()
                {
                    sku = fabricStyle.Sku,
                    color = XDesign_Color(fabricStyle.Color)
                };

                return result;
            }

            private static XDesign_FabricStyle XDesign_FabricStyle(MDesign_FabricStyle mFabricStyle)
            {
                var result = new XDesign_FabricStyle()
                {
                    sku = mFabricStyle.Sku,
                    color = XDesign_Color(mFabricStyle.Color)
                };

                return result;
            }

            private static XDesign_Block[] XDesign_Blocks(IEnumerable<Component> blockComponents, int size)
            {
                if (blockComponents == null) throw new ArgumentNullException(nameof(blockComponents));

                var result = new List<XDesign_Block>();

                foreach (var component in blockComponents)
                {
                    var blockComponent = (BlockComponent)component;
                    result.Add(XDesign_Block(blockComponent, size));
                }

                return result.ToArray();
            }

            private static XDesign_BlockPreview XDesign_BlockPreview(BlockComponent blockComponent, int size)
            {
                if (blockComponent == null) throw new ArgumentNullException(nameof(blockComponent));

                var blockSize = new Dimension(12, DimensionUnits.Inch);

                var scale = new DimensionScale(blockSize.Value, blockSize.Unit, size, DimensionUnits.Pixel);

                var pageLayout = new PageLayoutNode(blockSize * scale, blockSize * scale);
                pageLayout.LayoutSites[0].Node = blockComponent.Expand(false);
                pageLayout.UpdateBounds(PathOrientation.CreateDefault(), scale);

                var result = new XDesign_BlockPreview()
                {
                    width = (int)(blockSize * scale).Value,
                    height = (int)(blockSize * scale).Value,
                    shapes = XDesign_Shapes(pageLayout)
                };

                return result;
            }

            private static XDesign_BlockPreview XDesign_BlockPreview(MDesign_BlockPreview mBlockPreview)
            {
                var result = new XDesign_BlockPreview()
                {
                    width = mBlockPreview.Width,
                    height = mBlockPreview.Height,
                    shapes = XDesign_Shapes(mBlockPreview.Shapes)
                };

                return result;
            }

            private static XDesign_Color XDesign_Color(Color color)
            {
                var result = new XDesign_Color()
                {
                    webColor = color.WebColor
                };

                return result;
            }

            private static XDesign_Color XDesign_Color(MDesign_Color mColor)
            {
                var result = new XDesign_Color()
                {
                    webColor = mColor.WebColor
                };

                return result;
            }

            private static XDesign_DesignBlock XDesign_DesignBlock(BlockComponent blockComponent)
            {
                if (blockComponent == null) throw new ArgumentNullException(nameof(blockComponent));

                var result = new XDesign_DesignBlock()
                {
                    blockName = blockComponent.Name,
                    fabricStyles = XDesign_FabricStyles(blockComponent.FabricStyles)
                };

                return result;
            }

            private static XDesign_DesignBlock[] XDesign_DesignBlocks(IEnumerable<Component> blockComponents)
            {
                if (blockComponents == null) throw new ArgumentNullException(nameof(blockComponents));

                var result = new List<XDesign_DesignBlock>();

                foreach (var component in blockComponents)
                {
                    var blockComponent = (BlockComponent)component;
                    result.Add(XDesign_DesignBlock(blockComponent));
                }

                return result.ToArray();
            }

            private static XDesign_DesignLayout XDesign_DesignLayout(LayoutComponent layoutComponent)
            {
                if (layoutComponent == null)
                {
                    return null;
                }

                var result = new XDesign_DesignLayout()
                {
                    layoutName = layoutComponent.Name,
                    fabricStyles = XDesign_FabricStyles(layoutComponent.FabricStyles),
                    rowCount = layoutComponent.RowCount,
                    columnCount = layoutComponent.ColumnCount,
                    blockCount = layoutComponent.BlockCount
                };

                return result;
            }

            private static XDesign_DesignPreview XDesign_DesignPreview(Design.Core.Design design, int size)
            {
                if (design == null) throw new ArgumentNullException(nameof(design));

                var result = new XDesign_DesignPreview();

                if (design.LayoutComponent != null)
                {
                    var designSize = design.GetStandardSizes().Where(r => r.Preferred).Single();

                    var maxDimension = designSize.Width > designSize.Height ? designSize.Width : designSize.Height;

                    var scale = new DimensionScale(maxDimension.Value, maxDimension.Unit, size, DimensionUnits.Pixel);

                    var pageLayoutNode = new PageLayoutNode(designSize.Width * scale, designSize.Height * scale);
                    pageLayoutNode.LayoutSites[0].Node = design.LayoutComponent.Expand(true);
                    pageLayoutNode.UpdateBounds(PathOrientation.CreateDefault(), scale);

                    //result.layoutSites = CreateLayoutSiteDataArray(pageLayoutNode.LayoutSites, Palettes.Rainbow);
                    result.shapes = XDesign_Shapes(pageLayoutNode);
                }

                return result;
            }

            private static XDesign_FabricStyle[] XDesign_FabricStyles(FabricStyleList fabricStyles)
            {
                var result = new List<XDesign_FabricStyle>();

                foreach (var fabricStyle in fabricStyles)
                {
                    result.Add(XDesign_FabricStyle(fabricStyle));
                }

                return result.ToArray();
            }

            private static XDesign_FabricStyle[] XDesign_FabricStyles(IList<MDesign_FabricStyle> mFabricStyles)
            {
                var result = new List<XDesign_FabricStyle>();

                foreach (var mFabricStyle in mFabricStyles)
                {
                    result.Add(XDesign_FabricStyle(mFabricStyle));
                }

                return result.ToArray();
            }

            private static XDesign_LayoutPreview XDesign_LayoutPreview(LayoutComponent layoutComponent, int size)
            {
                var blockSize = new Dimension(12, DimensionUnits.Inch);

                var layoutWidth = blockSize * layoutComponent.ColumnCount;
                var layoutHeight = blockSize * layoutComponent.RowCount;

                var maxDimension = layoutWidth > layoutHeight ? layoutWidth : layoutHeight;

                var scale = new DimensionScale(maxDimension.Value, maxDimension.Unit, size, DimensionUnits.Pixel);

                var pageLayoutNode = new PageLayoutNode(layoutWidth * scale, layoutHeight * scale);
                pageLayoutNode.LayoutSites[0].Node = layoutComponent.Expand(false);
                pageLayoutNode.UpdateBounds(PathOrientation.CreateDefault(), scale);

                var result = new XDesign_LayoutPreview()
                {
                    width = (int)(layoutWidth * scale).Value,
                    height = (int)(layoutHeight * scale).Value,
                    layoutSites = XDesign_LayoutSites(pageLayoutNode, Palettes.Rainbow)
                };

                return result;
            }

            private static XDesign_LayoutSite XDesign_LayoutSite(LayoutSite layoutSite, FabricStyle fabricStyle)
            {
                var result = new XDesign_LayoutSite()
                {
                    path = XDesign_Path(layoutSite.Path)
                };

                if (fabricStyle != null)
                {
                    result.color = XDesign_Color(fabricStyle.Color);
                }

                return result;
            }

            private static XDesign_LayoutSite[] XDesign_LayoutSites(Node node, IPalette palette)
            {
                return XDesign_LayoutSites(GetLayoutSites(node), palette);
            }

            private static XDesign_LayoutSite[] XDesign_LayoutSites(IEnumerable<LayoutSite> layoutSites, IPalette palette)
            {
                var result = new List<XDesign_LayoutSite>();

                foreach (var layoutSite in layoutSites)
                {
                    FabricStyle fabricStyle = null;
                    if (layoutSite.Style != null)
                    {
                        fabricStyle = palette.GetFabricStyle(int.Parse(layoutSite.Style));
                    }

                    result.Add(XDesign_LayoutSite(layoutSite, fabricStyle));
                }

                return result.ToArray();
            }

            private static XDesign_Path XDesign_Path(IPath path)
            {
                var result = new XDesign_Path()
                {
                    points = XDesign_Points(path)
                };

                return result;
            }

            private static XDesign_Path XDesign_Path(MDesign_Path mPath)
            {
                var result = new XDesign_Path()
                {
                    points = XDesign_Points(mPath)
                };

                return result;
            }

            private static XDesign_Point[] XDesign_Points(IPath path)
            {
                var result = new XDesign_Point[path.SegmentCount];

                for (var idx = 0; idx < path.SegmentCount; ++idx)
                {
                    var segment = path.GetSegment(idx);

                    var pointWebData = new XDesign_Point();
                    result[idx] = pointWebData;

                    pointWebData.x = (int)segment.Origin.X.Value;
                    pointWebData.y = (int)segment.Origin.Y.Value;
                }

                return result;
            }

            private static XDesign_Point[] XDesign_Points(MDesign_Path mPath)
            {
                var result = new XDesign_Point[mPath.Points.Count];

                for (var idx = 0; idx < mPath.Points.Count; ++idx)
                {
                    var pointWebData = new XDesign_Point();
                    result[idx] = pointWebData;

                    pointWebData.x = mPath.Points[idx].X;
                    pointWebData.y = mPath.Points[idx].Y;
                }

                return result;
            }

            private static XDesign_Shape XDesign_Shape(ShapeNode shapeNode)
            {
                var result = new XDesign_Shape()
                {
                    path = XDesign_Path(shapeNode.Path),
                    color = XDesign_Color(shapeNode.FabricStyle.Color)
                };

                return result;
            }

            private static XDesign_Shape XDesign_Shape(MDesign_Shape mShape)
            {
                var result = new XDesign_Shape()
                {
                    path = XDesign_Path(mShape.Path),
                    color = XDesign_Color(mShape.Color)
                };

                return result;
            }

            private static XDesign_Shape[] XDesign_Shapes(Node node)
            {
                return XDesign_Shapes(GetShapeNodes(node));
            }

            private static XDesign_Shape[] XDesign_Shapes(IList<ShapeNode> shapeNodes)
            {
                var result = new XDesign_Shape[shapeNodes.Count];

                for (var idx = 0; idx < shapeNodes.Count; ++idx)
                {
                    var shapeNode = shapeNodes[idx];

                    result[idx] = XDesign_Shape(shapeNode);
                }

                return result;
            }

            private static XDesign_Shape[] XDesign_Shapes(IList<MDesign_Shape> mShapes)
            {
                var result = new XDesign_Shape[mShapes.Count];

                for (var idx = 0; idx < mShapes.Count; ++idx)
                {
                    var mShape = mShapes[idx];

                    result[idx] = XDesign_Shape(mShape);
                }

                return result;
            }

            private static void AddLayoutSites(List<LayoutSite> layoutSites, Node node)
            {
                switch (node)
                {
                    case ShapeNode _:
                        break;

                    case LayoutNode layoutNode:
                        AddLayoutSites(layoutSites, layoutNode);
                        break;

                    default:
                        throw new InvalidOperationException(string.Format("Unknown node type {0}", node.GetType().Name));
                        // Ignore
                }
            }

            private static void AddLayoutSites(List<LayoutSite> layoutSites, LayoutNode layoutNode)
            {
                foreach (var layoutSite in layoutNode.LayoutSites)
                {
                    if (layoutSite.Node == null || layoutSite.Node is ShapeNode)
                    {
                        layoutSites.Add(layoutSite);
                    }
                    else
                    {
                        AddLayoutSites(layoutSites, layoutSite.Node);
                    }
                }
            }

            private static void AddShapeNodes(List<ShapeNode> shapeNodes, Node node)
            {
                switch (node)
                {
                    case ShapeNode shapeNode:
                        shapeNodes.Add(shapeNode);
                        break;

                    case LayoutNode layoutNode:
                        AddShapeNodes(shapeNodes, layoutNode);
                        break;

                    default:
                        throw new InvalidOperationException(string.Format("Unknown node type {0}", node.GetType().Name));
                }
            }

            private static void AddShapeNodes(List<ShapeNode> shapeNodes, LayoutNode layoutNode)
            {
                foreach (var layoutSite in layoutNode.LayoutSites)
                {
                    if (layoutSite.Node != null)
                    {
                        AddShapeNodes(shapeNodes, layoutSite.Node);
                    }
                }
            }

            private static IList<LayoutSite> GetLayoutSites(Node node)
            {
                var layoutSites = new List<LayoutSite>();

                AddLayoutSites(layoutSites, node);

                return layoutSites;
            }

            private static IList<ShapeNode> GetShapeNodes(Node node)
            {
                var shapeNodes = new List<ShapeNode>();

                AddShapeNodes(shapeNodes, node);

                return shapeNodes;
            }
        }
    }
}