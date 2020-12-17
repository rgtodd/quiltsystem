//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Business.ComponentProviders;
using RichTodd.QuiltSystem.Database;
using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Design.Component.Standard;
using RichTodd.QuiltSystem.Design.Core;
using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Nodes.Standard;
using RichTodd.QuiltSystem.Design.Path;
using RichTodd.QuiltSystem.Design.Primitives;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class DesignMicroService : MicroService, IDesignMicroService
    {
        public DesignMicroService(
            IApplicationLocale locale,
            ILogger<DesignMicroService> logger,
            IQuiltContextFactory quiltContextFactory)
            : base(
                  locale,
                  logger,
                  quiltContextFactory)
        { }

        public async Task<long> AllocateOwnerAsync(string ownerReference)
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(AllocateOwnerAsync), ownerReference);
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var dbOwner = await ctx.Owners.Where(r => r.OwnerReference == ownerReference).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbOwner == null)
                {
                    dbOwner = new Owner()
                    {
                        OwnerReference = ownerReference,
                        OwnerTypeCode = "A"
                    };
                    _ = ctx.Owners.Add(dbOwner);

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                var result = dbOwner.OwnerId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MDesign_Dashboard> GetDashboardAsync()
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(GetDashboardAsync));
            try
            {
                using var ctx = CreateQuiltContext();

                var dashboard = new MDesign_Dashboard()
                {
                    TotalDesigns = await ctx.Designs.CountAsync()
                };

                var result = dashboard;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task GenerateStandardBlocks()
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(GenerateStandardBlocks));
            try
            {
                await Task.CompletedTask;

                ModelSetup.CreateStandardResources(QuiltContextFactory);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<IReadOnlyList<MDesign_Design>> GetDesignsAsync(long? ownerId, int? skip, int? take)
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(GetDesignsAsync), ownerId, skip, take);
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var dbDesigns = ctx.Designs.Where(r => r.DeleteDateTimeUtc == null);
                if (ownerId.HasValue)
                {
                    dbDesigns = dbDesigns.Where(r => r.OwnerId == ownerId.Value);
                }
                dbDesigns = dbDesigns.OrderByDescending(r => r.UpdateDateTimeUtc);
                if (skip.HasValue)
                {
                    dbDesigns = dbDesigns.Skip(skip.Value);
                }
                if (take.HasValue)
                {
                    dbDesigns = dbDesigns.Take(take.Value);
                }

                var dbDesignList = await dbDesigns.ToListAsync().ConfigureAwait(false);

                var designs = new List<MDesign_Design>();
                foreach (var dbDesign in dbDesignList)
                {
                    var dbDesignSnapshot = dbDesign.DesignSnapshots.Where(r => r.DesignSnapshotSequence == dbDesign.CurrentDesignSnapshotSequence).Single();
                    var design = Create.MDesign_Design(dbDesignSnapshot);
                    designs.Add(design);
                }

                var result = designs;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MDesign_Design> GetDesignAsync(Guid designId)
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(GetDesignAsync), designId);
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var dbDesign = await ctx.Designs.Where(r => r.DesignId == designId).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbDesign == null)
                {
                    return null;
                }

                //var ownerUserId = ParseUserId.FromOwnerReference(dbDesign.Owner.OwnerReference);
                //if (!SecurityPolicy.IsAuthorized(userId, ownerUserId))
                //{
                //    return null;
                //}

                var dbDesignSnapshot = dbDesign.DesignSnapshots.Where(r => r.DesignSnapshotSequence == dbDesign.CurrentDesignSnapshotSequence).Single();

                var result = Create.MDesign_Design(dbDesignSnapshot);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MDesign_Design> GetDesignAsync(int designSnapshotId)
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(GetDesignAsync), designSnapshotId);
            try
            {
                using var ctx = QuiltContextFactory.Create();

                //ctx.Database.Log = message => Trace.WriteLine(message);

                var dbDesignSnapshot = await ctx.DesignSnapshots.Where(r => r.DesignSnapshotId == designSnapshotId).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbDesignSnapshot == null)
                {
                    return null;
                }

                //var ownerUserId = ParseUserId.FromOwnerReference(dbDesignSnapshot.Design.Owner.OwnerReference);
                //if (!SecurityPolicy.IsAuthorized(userId, ownerUserId))
                //{
                //    return null;
                //}

                //Logger.LogMessage("dbDesignSnapshot retrieved.");

                var result = Create.MDesign_Design(dbDesignSnapshot);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<Guid> CreateDesignAsync(long ownerId, string name, MDesign_DesignSpecification designSpecification, DateTime utcNow)
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(CreateDesignAsync), ownerId, name, designSpecification, utcNow);
            try
            {
                //if (SecurityPolicy.IsBuiltInUser(userId))
                //{
                //    throw new InvalidOperationException("Built-in user ID not supported.");
                //}

                //var artifactValue = design.JsonSave().ToString();

                using var ctx = QuiltContextFactory.Create();

                //var ownerReference = CreateOwnerReference.FromUserId(userId);
                //var dbOwner = await ctx.Owners.Where(r => r.OwnerReference == ownerReference).SingleOrDefaultAsync().ConfigureAwait(false);
                //if (dbOwner == null)
                //{
                //    dbOwner = new Owner()
                //    {
                //        OwnerReference = ownerReference,
                //        OwnerTypeCode = "A"
                //    };
                //    _ = ctx.Owners.Add(dbOwner);
                //}

                //ctx.Database.Log = message => Trace.WriteLine(message);

                var dbDesign = new QuiltSystem.Database.Model.Design()
                {
                    DesignId = Guid.NewGuid(),
                    OwnerId = ownerId,
                    Name = name ?? "New Project",
                    CurrentDesignSnapshotSequence = 0,
                    CreateDateTimeUtc = utcNow,
                    UpdateDateTimeUtc = utcNow
                };
                _ = ctx.Designs.Add(dbDesign);

                var dbArtifact = new Artifact()
                {
                    ArtifactTypeCode = ArtifactTypeCodes.Design,
                    ArtifactValueTypeCode = ArtifactValueTypeCodes.Json,
                    Value = designSpecification.ArtifactValue
                };
                _ = ctx.Artifacts.Add(dbArtifact);

                var dbDesignSnapshot = new DesignSnapshot()
                {
                    Design = dbDesign,
                    DesignSnapshotSequence = 0,
                    Artifact = dbArtifact,
                    Name = name ?? "New Project",
                    CreateDateTimeUtc = utcNow,
                    UpdateDateTimeUtc = utcNow
                };
                _ = ctx.DesignSnapshots.Add(dbDesignSnapshot);

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                var result = dbDesign.DesignId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<Guid> UpdateDesignAsync(Guid designId, MDesign_DesignSpecification designSpecification, DateTime utcNow)
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(UpdateDesignAsync), designId, designSpecification, utcNow);
            try
            {
                using var ctx = QuiltContextFactory.Create();

                //ctx.Database.Log = message => Trace.WriteLine(message);

                var dbDesign = await ctx.Designs.SingleAsync(p => p.DesignId == designId).ConfigureAwait(false);

                //var ownerUserId = ParseUserId.FromOwnerReference(dbDesign.Owner.OwnerReference);
                //if (!SecurityPolicy.IsAuthorized(userId, ownerUserId))
                //{
                //    throw new InvalidOperationException("Design has different owner.");
                //}

                dbDesign.CurrentDesignSnapshotSequence += 1;
                dbDesign.UpdateDateTimeUtc = utcNow;

                var dbArtifact = new Artifact()
                {
                    ArtifactTypeCode = ArtifactTypeCodes.Design,
                    ArtifactValueTypeCode = ArtifactValueTypeCodes.Json,
                    Value = designSpecification.ArtifactValue
                };
                _ = ctx.Artifacts.Add(dbArtifact);

                var dbDesignSnapshot = new DesignSnapshot()
                {
                    Design = dbDesign,
                    Artifact = dbArtifact,
                    Name = dbDesign.Name,
                    DesignSnapshotSequence = dbDesign.CurrentDesignSnapshotSequence,
                    CreateDateTimeUtc = utcNow,
                    UpdateDateTimeUtc = utcNow
                };
                _ = ctx.DesignSnapshots.Add(dbDesignSnapshot);

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                var result = designId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> RenameDesignAsync(Guid designId, string name)
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(RenameDesignAsync), designId, name);
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var dbDesign = await ctx.Designs.Where(r => r.DesignId == designId).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbDesign == null)
                {
                    return false;
                }
                //var ownerUserId = ParseUserId.FromOwnerReference(dbDesign.Owner.OwnerReference);
                //if (!SecurityPolicy.IsAuthorized(userId, ownerUserId))
                //{
                //    return false;
                //}

                var dbDesignSnapshot = dbDesign.DesignSnapshots.Where(r => r.DesignSnapshotSequence == dbDesign.CurrentDesignSnapshotSequence).Single();

                dbDesign.Name = name;
                dbDesignSnapshot.Name = name;

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                var result = true;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> DeleteDesignAsync(Guid designId, DateTime utcNow)
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(DeleteDesignAsync), designId, utcNow);
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var dbDesign = await ctx.Designs.Where(r => r.DesignId == designId).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbDesign == null)
                {
                    return false;
                }
                //var ownerUserId = ParseUserId.FromOwnerReference(dbDesign.Owner.OwnerReference);
                //if (!SecurityPolicy.IsAuthorized(userId, ownerUserId))
                //{
                //    return false;
                //}

                dbDesign.DeleteDateTimeUtc = utcNow;

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                var result = true;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> HasDeletedDesignsAsync(long ownerId)
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(HasDeletedDesignsAsync), ownerId);
            try
            {
                //if (SecurityPolicy.IsBuiltInUser(userId))
                //{
                //    throw new InvalidOperationException("Built-in user ID not supported.");
                //}

                using var ctx = QuiltContextFactory.Create();

                //var ownerReference = CreateOwnerReference.FromUserId(userId);
                var result = await ctx.Designs.AnyAsync(r => r.OwnerId == ownerId && r.DeleteDateTimeUtc != null).ConfigureAwait(false);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<Guid?> UndeleteDesignAsync(long ownerId)
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(UndeleteDesignAsync), ownerId);
            try
            {
                //if (SecurityPolicy.IsBuiltInUser(userId))
                //{
                //    throw new InvalidOperationException("Built-in user ID not supported.");
                //}

                using var ctx = QuiltContextFactory.Create();

                //var ownerReference = CreateOwnerReference.FromUserId(userId);
                var dbDesign = await (from p in ctx.Designs
                                      where p.DeleteDateTimeUtc != null && p.OwnerId == ownerId
                                      orderby p.DeleteDateTimeUtc descending
                                      select p).FirstOrDefaultAsync().ConfigureAwait(false);

                Guid? deletedDesignId;
                if (dbDesign != null)
                {
                    dbDesign.DeleteDateTimeUtc = null;

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                    deletedDesignId = dbDesign.DesignId;
                }
                else
                {
                    deletedDesignId = null;
                }

                var result = deletedDesignId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MDesign_FabricStyleCatalog> GetFabricStyles()
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(GetFabricStyles));
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var inventoryItems = await (from ii in ctx.InventoryItems
                                            where ii.InventoryItemTypeCode == InventoryItemTypeCodes.Fabric && (ii.Quantity - ii.ReservedQuantity) > 0
                                            orderby ii.Hue, ii.Saturation, ii.Value
                                            select ii).ToListAsync().ConfigureAwait(false);

                var tags = await (from iit in ctx.InventoryItemTags
                                  join t in ctx.Tags on iit.TagId equals t.TagId
                                  select new { iit.InventoryItemId, t.TagTypeCode, t.Value }).ToListAsync().ConfigureAwait(false);

                var manufacturerNames = new Dictionary<long, string>();
                var collectionNameSets = new Dictionary<long, List<string>>();
                foreach (var tag in tags)
                {
                    var inventoryItem = inventoryItems.Where(ii => ii.InventoryItemId == tag.InventoryItemId).SingleOrDefault();
                    if (inventoryItem != null)
                    {
                        // Note: assumes an inventory item is associated with only one manufacturer.
                        //
                        if (tag.TagTypeCode == TagTypeCodes.Manufacturer)
                        {
                            manufacturerNames[tag.InventoryItemId] = tag.Value;
                        }

                        if (tag.TagTypeCode == TagTypeCodes.Collection)
                        {
                            if (!collectionNameSets.TryGetValue(tag.InventoryItemId, out var collectionNameSet))
                            {
                                collectionNameSet = new List<string>();
                                collectionNameSets[tag.InventoryItemId] = collectionNameSet;
                            }
                            collectionNameSet.Add(tag.Value);
                        }
                    }
                }

                var manufacturers = new Dictionary<string, MDesign_FabricStyleManufacturer>();
                var collectionSets = new Dictionary<MDesign_FabricStyleManufacturer, List<MDesign_FabricStyleCollection>>();
                var fabricSets = new Dictionary<MDesign_FabricStyleCollection, List<MDesign_FabricStyle>>();

                foreach (var inventoryItem in inventoryItems)
                {
                    var manufacturerName = manufacturerNames[inventoryItem.InventoryItemId];
                    var collectionNameSet = collectionNameSets[inventoryItem.InventoryItemId];

                    if (!manufacturers.TryGetValue(manufacturerName, out var manufacturer))
                    {
                        manufacturer = new MDesign_FabricStyleManufacturer()
                        {
                            ManufacturerName = manufacturerName
                        };
                        manufacturers[manufacturerName] = manufacturer;

                        collectionSets[manufacturer] = new List<MDesign_FabricStyleCollection>();
                    }

                    var collectionSet = collectionSets[manufacturer];
                    foreach (var collectionName in collectionNameSet)
                    {
                        var collection = collectionSet.Where(r => r.CollectionName == collectionName).SingleOrDefault();
                        if (collection == null)
                        {
                            collection = new MDesign_FabricStyleCollection()
                            {
                                CollectionName = collectionName
                            };
                            collectionSet.Add(collection);

                            fabricSets[collection] = new List<MDesign_FabricStyle>();
                        }

                        var fabricSet = fabricSets[collection];
                        var color = Color.FromAhsb(255, inventoryItem.Hue, inventoryItem.Saturation / 100.0, inventoryItem.Value / 100.0);
                        var fabricStyle = new FabricStyle(inventoryItem.Sku, color);
                        fabricSet.Add(Create.MDesign_FabricStyle(fabricStyle));
                    }
                }

                foreach (var collection in fabricSets.Keys)
                {
                    collection.FabricStyles = fabricSets[collection].ToArray();
                }
                foreach (var manufacturer in collectionSets.Keys)
                {
                    manufacturer.Collections = collectionSets[manufacturer].ToArray();
                }

                var result = new MDesign_FabricStyleCatalog()
                {
                    Manufacturers = manufacturers.Values.ToArray()
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

        public async Task<MDesign_BlockCollection> GetBlockCollectionAsync(int previewSize)
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(GetBlockCollectionAsync));
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var allTags = await ctx.Tags.Where(r => r.TagTypeCode == TagTypeCodes.Block).OrderBy(r => r.Value).Select(r => r.Value).ToArrayAsync().ConfigureAwait(false);

                var blockList = new List<MDesign_Block>();

                var provider = new DatabaseBlockComponentProvider(QuiltContextFactory);
                foreach (var entry in provider.GetComponents(BlockComponent.TypeName, Constants.DefaultComponentCategory).OrderBy(r => r.Category).ThenBy(r => r.Name))
                {
                    var blockComponent = (BlockComponent)entry.Component;
                    var data = Create.MDesign_Block(blockComponent, entry.Tags, previewSize);
                    blockList.Add(data);
                }

                var result = new MDesign_BlockCollection()
                {
                    Blocks = blockList,
                    AllTags = allTags
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

        public async Task<byte[]> GetBlockThumbnailAsync(string blockId, int thumbnailSize)
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(GetBlockThumbnailAsync), blockId, thumbnailSize);
            try
            {
                await Task.CompletedTask;

                var provider = new DatabaseBlockComponentProvider(QuiltContextFactory);
                var entry = provider.GetComponent(BlockComponent.TypeName, Constants.DefaultComponentCategory, blockId);

                var node = entry.Component.Expand(true);

                var blockDimension = new Dimension(1, DimensionUnits.Inch);
                var thumbnailDimension = new Dimension(thumbnailSize, DimensionUnits.Pixel);
                var scale = new DimensionScale(1, DimensionUnits.Inch, thumbnailSize, DimensionUnits.Pixel);

                var pageLayoutNode = new PageLayoutNode(blockDimension * scale, blockDimension * scale);
                pageLayoutNode.LayoutSites[0].Node = node;
                pageLayoutNode.UpdateBounds(PathOrientation.CreateDefault(), scale);

                var renderer = new DesignRenderer();
                using var image = renderer.CreateBitmap(node, DimensionScale.CreateIdentity(DimensionUnits.Pixel), false);

                using var ms = new MemoryStream();
                image.Save(ms, ImageFormat.Png);

                var result = ms.ToArray();

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<byte[]> GetDesignThumbnailAsync(Guid designId, int thumbnailSize)
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(GetDesignSnapshotThumbnailAsync), designId, thumbnailSize);
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var dbDesign = await ctx.Designs.Where(r => r.DesignId == designId).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbDesign == null)
                {
                    return null;
                }

                var dbDesignSnapshot = dbDesign.DesignSnapshots.Where(r => r.DesignSnapshotSequence == dbDesign.CurrentDesignSnapshotSequence).Single();

                var json = JToken.Parse(dbDesignSnapshot.Artifact.Value);
                var design = new Design.Core.Design(json);

                //Logger.LogMessage("Artifact parsed.");

                var renderer = new DesignRenderer();

                using var image = renderer.CreateBitmap(design, thumbnailSize, false);
                using var ms = new MemoryStream();

                image.Save(ms, ImageFormat.Png);
                var result = ms.ToArray();

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<byte[]> GetDesignSnapshotThumbnailAsync(int designSnapshotId, int thumbnailSize)
        {
            using var log = BeginFunction(nameof(DesignMicroService), nameof(GetDesignSnapshotThumbnailAsync), designSnapshotId, thumbnailSize);
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var dbDesignSnapshot = await ctx.DesignSnapshots.Where(r => r.DesignSnapshotId == designSnapshotId).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbDesignSnapshot == null)
                {
                    return null;
                }

                var json = JToken.Parse(dbDesignSnapshot.Artifact.Value);
                var design = new Design.Core.Design(json);

                //Logger.LogMessage("Artifact parsed.");

                var renderer = new DesignRenderer();

                using var image = renderer.CreateBitmap(design, thumbnailSize, false);
                using var ms = new MemoryStream();

                image.Save(ms, ImageFormat.Png);
                var result = ms.ToArray();

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private static class Create
        {
            public static MDesign_Design MDesign_Design(DesignSnapshot dbDesignSnapshot)
            {
                var design = new MDesign_Design()
                {
                    DesignId = dbDesignSnapshot.DesignId,
                    DesignSnapshotId = dbDesignSnapshot.DesignSnapshotId,
                    Name = dbDesignSnapshot.Name,
                    DesignArtifactValue = dbDesignSnapshot.Artifact.Value,
                    UpdateDateTimeUtc = dbDesignSnapshot.UpdateDateTimeUtc
                };

                return design;
            }

            public static MDesign_Block MDesign_Block(BlockComponent blockComponent, string[] tags, int size)
            {
                if (blockComponent == null) throw new ArgumentNullException(nameof(blockComponent));

                var idxLastSlash = blockComponent.Name.LastIndexOf('/');
                var group = idxLastSlash != -1
                    ? blockComponent.Name.Substring(0, idxLastSlash)
                    : "Default";

                var id = blockComponent.Name.Replace(" ", "").Replace("/", "");

                var result = new MDesign_Block()
                {
                    Id = id,
                    Category = blockComponent.Category,
                    Group = group,
                    BlockName = blockComponent.Name,
                    FabricStyles = MDesign_FabricStyles(blockComponent.FabricStyles),
                    Tags = tags,
                    Preview = MDesign_BlockPreview(blockComponent, size)
                };

                return result;
            }

            public static MDesign_FabricStyle MDesign_FabricStyle(FabricStyle fabricStyle)
            {
                var result = new MDesign_FabricStyle()
                {
                    Sku = fabricStyle.Sku,
                    Color = MDesign_Color(fabricStyle.Color)
                };

                return result;
            }

            private static IList<MDesign_FabricStyle> MDesign_FabricStyles(FabricStyleList fabricStyles)
            {
                var result = new List<MDesign_FabricStyle>();

                foreach (var fabricStyle in fabricStyles)
                {
                    result.Add(MDesign_FabricStyle(fabricStyle));
                }

                return result;
            }

            private static MDesign_Color MDesign_Color(Color color)
            {
                var result = new MDesign_Color()
                {
                    WebColor = color.WebColor
                };

                return result;
            }

            private static MDesign_BlockPreview MDesign_BlockPreview(BlockComponent blockComponent, int size)
            {
                if (blockComponent == null) throw new ArgumentNullException(nameof(blockComponent));

                var blockSize = new Dimension(12, DimensionUnits.Inch);

                var scale = new DimensionScale(blockSize.Value, blockSize.Unit, size, DimensionUnits.Pixel);

                var pageLayout = new PageLayoutNode(blockSize * scale, blockSize * scale);
                pageLayout.LayoutSites[0].Node = blockComponent.Expand(false);
                pageLayout.UpdateBounds(PathOrientation.CreateDefault(), scale);

                var result = new MDesign_BlockPreview()
                {
                    Width = (int)(blockSize * scale).Value,
                    Height = (int)(blockSize * scale).Value,
                    Shapes = MDesign_Shapes(pageLayout)
                };

                return result;
            }

            private static IList<MDesign_Shape> MDesign_Shapes(Node node)
            {
                return MDesign_Shapes(GetShapeNodes(node));
            }

            private static IList<MDesign_Shape> MDesign_Shapes(IList<ShapeNode> shapeNodes)
            {
                var result = new List<MDesign_Shape>();

                for (var idx = 0; idx < shapeNodes.Count; ++idx)
                {
                    var shapeNode = shapeNodes[idx];

                    result.Add(MDesign_Shape(shapeNode));
                }

                return result;
            }

            private static MDesign_Shape MDesign_Shape(ShapeNode shapeNode)
            {
                var result = new MDesign_Shape()
                {
                    Path = MDesign_Path(shapeNode.Path),
                    Color = MDesign_Color(shapeNode.FabricStyle.Color)
                };

                return result;
            }

            private static MDesign_Path MDesign_Path(IPath path)
            {
                var result = new MDesign_Path()
                {
                    Points = MDesign_Points(path)
                };

                return result;
            }

            private static IList<MDesign_Point> MDesign_Points(IPath path)
            {
                var result = new List<MDesign_Point>();

                for (var idx = 0; idx < path.SegmentCount; ++idx)
                {
                    var segment = path.GetSegment(idx);

                    var pointWebData = new MDesign_Point();
                    result.Add(pointWebData);

                    pointWebData.X = (int)segment.Origin.X.Value;
                    pointWebData.Y = (int)segment.Origin.Y.Value;
                }

                return result;
            }

            private static IList<ShapeNode> GetShapeNodes(Node node)
            {
                var shapeNodes = new List<ShapeNode>();

                AddShapeNodes(shapeNodes, node);

                return shapeNodes;
            }

            private static void AddShapeNodes(IList<ShapeNode> shapeNodes, Node node)
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

            private static void AddShapeNodes(IList<ShapeNode> shapeNodes, LayoutNode layoutNode)
            {
                foreach (var layoutSite in layoutNode.LayoutSites)
                {
                    if (layoutSite.Node != null)
                    {
                        AddShapeNodes(shapeNodes, layoutSite.Node);
                    }
                }
            }

        }
    }
}
