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

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Business.Libraries;
using RichTodd.QuiltSystem.Design.Build;
using RichTodd.QuiltSystem.Design.Core;
using RichTodd.QuiltSystem.Design.Primitives;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class KitMicroService : BaseService, IKitMicroService
    {
        private IDesignMicroService DesignMicroService { get; }
        private IProjectMicroService ProjectMicroService { get; }
        private IInventoryMicroService InventoryMicroService { get; }

        public KitMicroService(
            IApplicationRequestServices requestServices,
            ILogger<KitMicroService> logger,
            IDesignMicroService designMicroService,
            IInventoryMicroService inventoryMicroService,
            IProjectMicroService projectMicroService)
            : base(requestServices, logger)
        {
            DesignMicroService = designMicroService ?? throw new ArgumentNullException(nameof(designMicroService));
            InventoryMicroService = inventoryMicroService ?? throw new ArgumentNullException(nameof(inventoryMicroService));
            ProjectMicroService = projectMicroService ?? throw new ArgumentNullException(nameof(projectMicroService));
        }

        public async Task<MKit_Kit> GetKitDetailAsync(string userId, Guid projectId, int thumbnailSize)
        {
            using var log = BeginFunction(nameof(KitMicroService), nameof(GetKitDetailAsync), userId, projectId, thumbnailSize);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var entry = await ProjectMicroService.GetProjectAsync(projectId).ConfigureAwait(false);
                if (entry == null)
                {
                    log.Result(null);
                    return null;
                }

                log.Message("Project entry retrieved.");

                var kit = ProjectLibraryKitUtility.CreateKit(entry);

                log.Message("Kit created.");

                var result = Create.MKit_Kit(kit, entry.Name, entry.ProjectId, null, thumbnailSize, InventoryMicroService);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MKit_Kit> GetKitDetailAsync(string userId, long projectSnapshotId, int thumbnailSize)
        {
            using var log = BeginFunction(nameof(KitMicroService), nameof(GetKitDetailAsync), userId, projectSnapshotId, thumbnailSize);
            try
            {
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var entry = await ProjectMicroService.GetProjectAsync(projectSnapshotId).ConfigureAwait(false);
                if (entry == null)
                {
                    log.Result(null);
                    return null;
                }

                log.Message("Project entry retrieved.");

                var kit = ProjectLibraryKitUtility.CreateKit(entry);

                log.Message("Kit created.");

                var result = Create.MKit_Kit(kit, entry.Name, entry.ProjectId, null, thumbnailSize, InventoryMicroService);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MKit_Kit> GetKitDetailFromDesignAsync(string userId, Guid designId, int thumbnailSize)
        {
            using var log = BeginFunction(nameof(KitMicroService), nameof(GetKitDetailFromDesignAsync), userId, designId, thumbnailSize);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var mDesign = await DesignMicroService.GetDesignAsync(designId).ConfigureAwait(false);
                if (mDesign == null)
                {
                    log.Result(null);
                    return null;
                }

                var result = Create.MKit_Kit(mDesign, thumbnailSize, InventoryMicroService);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MKit_Kit> GetKitDetailPreviewAsync(string userId, Guid projectId, int thumbnailSize, MKit_KitSpecificationUpdate specificationUpdate)
        {
            using var log = BeginFunction(nameof(KitMicroService), nameof(GetKitDetailPreviewAsync), userId, projectId, thumbnailSize, specificationUpdate);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var entry = await ProjectMicroService.GetProjectAsync(projectId).ConfigureAwait(false);
                if (entry == null)
                {
                    log.Result(null);
                    return null;
                }

                var kit = ProjectLibraryKitUtility.CreateKit(entry);
                var specification = kit.KitSpecification.Clone();
                var standardSizes = kit.Design.GetStandardSizes();

                var serviceErrorBuilder = new ServiceErrorBuilder();
                UpdateSpecification(specification, specificationUpdate, standardSizes, serviceErrorBuilder);

                kit.KitSpecification = specification;

                var result = Create.MKit_Kit(kit, entry.Name, entry.ProjectId, null, thumbnailSize, InventoryMicroService);
                if (serviceErrorBuilder.ServiceError != null)
                {
                    serviceErrorBuilder.AddPageError("One or more input values are incorrect.");
                    result.ServiceError = serviceErrorBuilder.ServiceError;
                }

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MKit_Kit> GetKitDetailPreviewFromDesignAsync(string userId, Guid designId, int thumbnailSize, MKit_KitSpecificationUpdate specificationUpdate)
        {
            using var log = BeginFunction(nameof(KitMicroService), nameof(GetKitDetailPreviewFromDesignAsync), userId, designId, thumbnailSize, specificationUpdate);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var mDesign = await DesignMicroService.GetDesignAsync(designId).ConfigureAwait(false);
                if (mDesign == null)
                {
                    log.Result(null);
                    return null;
                }

                var design = new Design.Core.Design(JToken.Parse(mDesign.DesignArtifactValue));

                var kit = new Kit(design, new KitSpecification());
                var specification = kit.KitSpecification.Clone();
                var standardSizes = kit.Design.GetStandardSizes();

                var serviceErrorBuilder = new ServiceErrorBuilder();
                UpdateSpecification(specification, specificationUpdate, standardSizes, serviceErrorBuilder);

                kit.KitSpecification = specification;

                var result = Create.MKit_Kit(kit, "New", null, designId, thumbnailSize, InventoryMicroService);
                if (serviceErrorBuilder.ServiceError != null)
                {
                    serviceErrorBuilder.AddPageError("One or more input values are incorrect.");
                    result.ServiceError = serviceErrorBuilder.ServiceError;
                }

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> SaveKitAsync(string userId, MKit_Kit kit)
        {
            using var log = BeginFunction(nameof(KitMicroService), nameof(SaveKitAsync), userId, kit);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MKit_UpdateSpecificationResponse> UpdateKitSpecificationAsync(string userId, Guid projectId, MKit_KitSpecificationUpdate specificationUpdate)
        {
            using var log = BeginFunction(nameof(KitMicroService), nameof(UpdateKitSpecificationAsync), userId, projectId, specificationUpdate);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var entry = await ProjectMicroService.GetProjectAsync(projectId).ConfigureAwait(false);

                var kit = ProjectLibraryKitUtility.CreateKit(entry);
                var specification = kit.KitSpecification.Clone();
                var standardSizes = kit.Design.GetStandardSizes();

                var serviceErrorBuilder = new ServiceErrorBuilder();
                UpdateSpecification(specification, specificationUpdate, standardSizes, serviceErrorBuilder);

                kit.KitSpecification = specification;

                if (serviceErrorBuilder.ServiceError != null)
                {
                    serviceErrorBuilder.AddPageError("Kit not updated.");
                    var result = new MKit_UpdateSpecificationResponse()
                    {
                        ServiceError = serviceErrorBuilder.ServiceError
                    };
                    log.Result(result);
                    return result;
                }
                else
                {
                    var data = ProjectLibraryKitUtility.CreateProjectSpecification(kit);

                    _ = await ProjectMicroService.UpdateProjectAsync(projectId, data, GetUtcNow()).ConfigureAwait(false);

                    var result = new MKit_UpdateSpecificationResponse();
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

        private void UpdateSpecification(KitSpecification specification, MKit_KitSpecificationUpdate specificationUpdate, List<DesignSize> standardSizes, ServiceErrorBuilder serviceErrorBuilder)
        {
            switch (specificationUpdate.Size)
            {
                case "CUSTOM":
                    try
                    {
                        specification.Width = Dimension.Parse(specificationUpdate.CustomWidth);
                    }
                    catch (Exception)
                    {
                        serviceErrorBuilder.AddFieldError("CustomWidth", "Invalid format.");
                    }
                    try
                    {
                        specification.Height = Dimension.Parse(specificationUpdate.CustomHeight);
                    }
                    catch (Exception)
                    {
                        serviceErrorBuilder.AddFieldError("CustomHeight", "Invalid format.");
                    }
                    break;

                default:
                    var size = standardSizes.Where(r => r.Id == specificationUpdate.Size).Single();
                    specification.Width = size.Width;
                    specification.Height = size.Height;
                    break;
            }

            switch (specificationUpdate.BorderWidth)
            {
                case "CUSTOM":
                    try
                    {
                        specification.BorderWidth = Dimension.Parse(specificationUpdate.CustomBorderWidth);
                    }
                    catch (Exception)
                    {
                        serviceErrorBuilder.AddFieldError("CustomBorderWidth", "Invalid format.");
                    }
                    break;

                case "NONE":
                    specification.BorderWidth = new Dimension(0, DimensionUnits.Inch);
                    break;

                default:
                    try
                    {
                        specification.BorderWidth = Dimension.Parse(specificationUpdate.BorderWidth + @"""");
                    }
                    catch (Exception)
                    {
                        serviceErrorBuilder.AddFieldError("BorderWidth", "Invalid format.");
                    }
                    break;
            }
            if (specificationUpdate.BorderFabricStyle != null)
            {
                specification.BorderFabricStyle = Create.CreateFabricStyle(specificationUpdate.BorderFabricStyle, InventoryMicroService);
            }

            switch (specificationUpdate.BindingWidth)
            {
                case "CUSTOM":
                    try
                    {
                        specification.BindingWidth = Dimension.Parse(specificationUpdate.CustomBindingWidth);
                    }
                    catch (Exception)
                    {
                        serviceErrorBuilder.AddFieldError("CustomBindingWidth", "Invalid format.");
                    }
                    break;

                case "NONE":
                    specification.BindingWidth = new Dimension(0, DimensionUnits.Inch);
                    break;

                default:
                    try
                    {
                        specification.BindingWidth = Dimension.Parse(specificationUpdate.BindingWidth + @"""");
                    }
                    catch (Exception)
                    {
                        serviceErrorBuilder.AddFieldError("BindingWidth", "Invalid format.");
                    }
                    break;
            }

            if (specificationUpdate.BindingFabricStyle != null)
            {
                specification.BindingFabricStyle = Create.CreateFabricStyle(specificationUpdate.BindingFabricStyle, InventoryMicroService);
            }

            if (specificationUpdate.HasBacking != null)
            {
                specification.HasBacking = specificationUpdate.HasBacking.Value;
            }
            if (specificationUpdate.BackingFabricStyle != null)
            {
                specification.BackingFabricStyle = Create.CreateFabricStyle(specificationUpdate.BackingFabricStyle, InventoryMicroService);
            }

            if (specificationUpdate.TrimTriangles != null)
            {
                specification.TrimTriangles = specificationUpdate.TrimTriangles.Value;
            }
        }

        private static class Create
        {
            private static MKit_KitBuildItem MKit_KitBuildItem(KitBuildItem item, DimensionScale scale)
            {
                var name = item.BuildItemType == BuildComponentTypes.Yardage || item.BuildItemType == BuildComponentTypes.Piece || item.BuildItemType == BuildComponentTypes.Component
                    ? item.BuildItemSubtype
                    : item.BuildItemType == BuildComponentTypes.Block
                        ? item.BuildItemSubtype == "Quilt Layout"
                            ? "Quilt Top"
                            : "Block"
                        : item.BuildItemType == BuildComponentTypes.Quilt
                            ? "Quilt"
                            : "?";

                var itemData = new MKit_KitBuildItem()
                {
                    Id = item.Id,
                    Name = name,
                    Quantity = item.Quantity,
                    Width = item.Area.Width.ToString(),
                    Height = item.Area.Height.ToString(),
                    PartId = item.PartId,
                    Image = MKit_KitBuildItemImage(item, scale)
                };

                if (item.FabricStyles.Count >= 1)
                {
                    itemData.Sku1 = item.FabricStyles[0].Sku;
                    itemData.WebColor1 = item.FabricStyles[0].Color.WebColor;
                }
                if (item.FabricStyles.Count >= 2)
                {
                    itemData.Sku2 = item.FabricStyles[1].Sku;
                    itemData.WebColor2 = item.FabricStyles[1].Color.WebColor;
                }

                return itemData;
            }

            private static IList<MKit_KitBuildStep> MKit_KitBuildSteps(Kit kit)
            {
                var kitBuildSteps = new List<MKit_KitBuildStep>();

                foreach (var buildStep in kit.BuildSteps)
                {
                    DimensionScale scale;
                    var maximumWidth = buildStep.GetMaximumBuildItemWidth();
                    scale = maximumWidth.Value > 45
                        ? new DimensionScale(80, DimensionUnits.Inch, 500, DimensionUnits.Pixel)
                        : maximumWidth.Value > 25
                            ? new DimensionScale(45, DimensionUnits.Inch, 500, DimensionUnits.Pixel)
                            : new DimensionScale(25, DimensionUnits.Inch, 500, DimensionUnits.Pixel);

                    var consumes = new List<MKit_KitBuildItem>();
                    foreach (var input in buildStep.Consumes)
                    {
                        var consume = MKit_KitBuildItem(input, scale);
                        consumes.Add(consume);
                    }

                    var produces = new List<MKit_KitBuildItem>();
                    foreach (var output in buildStep.Produces)
                    {
                        var produce = MKit_KitBuildItem(output, scale);
                        produces.Add(produce);
                    }

                    var buildStepData = new MKit_KitBuildStep()
                    {
                        Description = "Step " + buildStep.StepNumber + " - " + buildStep.Description,
                        Consumes = consumes,
                        Produces = produces
                    };

                    kitBuildSteps.Add(buildStepData);
                }

                return kitBuildSteps;
            }

            private static IList<MKit_KitPart> MKit_KitParts(Kit kit, IInventoryMicroService InventoryMicroService)
            {
                var kitParts = new List<MKit_KitPart>();

                foreach (var part in kit.Parts)
                {
                    var inventoryItem = InventoryMicroService.GetEntry(part.Sku);

                    kitParts.Add(new MKit_KitPart()
                    {
                        Id = part.Id,
                        Sku = part.Sku,
                        Quantity = part.Quantity,
                        UnitOfMeasure = MKit_KitPartUnitOfMeasures(part.AreaSize),
                        WebColor = part.Color.WebColor,
                        Description = inventoryItem.Name,
                        Manufacturer = inventoryItem.Manufacturer,
                        Collection = inventoryItem.Collection
                    });
                }

                return kitParts;
            }

            private static MKit_KitPartUnitOfMeasures MKit_KitPartUnitOfMeasures(AreaSizes areaSize)
            {
                return areaSize switch
                {
                    AreaSizes.FatQuarter => Abstractions.Data.MKit_KitPartUnitOfMeasures.FatQuarter,
                    AreaSizes.HalfYard => Abstractions.Data.MKit_KitPartUnitOfMeasures.HalfYard,
                    AreaSizes.ThreeYards => Abstractions.Data.MKit_KitPartUnitOfMeasures.ThreeYards,
                    AreaSizes.TwoYards => Abstractions.Data.MKit_KitPartUnitOfMeasures.TwoYards,
                    AreaSizes.Yard => Abstractions.Data.MKit_KitPartUnitOfMeasures.Yard,
                    _ => throw new InvalidOperationException(string.Format("Unknown area size {0}.", areaSize)),
                };
            }

            private static MKit_Size MKit_Size(DesignSize designSize)
            {
                return new MKit_Size()
                {
                    Id = designSize.Id,
                    Width = designSize.Width.ToString(),
                    Height = designSize.Height.ToString(),
                    Description = designSize.Description,
                    Preferred = designSize.Preferred
                };
            }

            private static IList<MKit_Size> MKit_Size(IEnumerable<DesignSize> designSizes)
            {
                var result = new List<MKit_Size>();

                foreach (var designSize in designSizes)
                {
                    result.Add(MKit_Size(designSize));
                }

                return result;
            }

            private static MKit_KitSpecification MKit_KitSpecification(Kit kit, IInventoryMicroService InventoryMicroService)
            {
                var totalWidth = kit.KitSpecification.Width + (kit.KitSpecification.BorderWidth * 2);
                var totalHeight = kit.KitSpecification.Height + (kit.KitSpecification.BorderWidth * 2);

                var specification = new MKit_KitSpecification()
                {
                    Width = kit.KitSpecification.Width.ToString(),
                    Height = kit.KitSpecification.Height.ToString(),
                    TotalWidth = totalWidth.ToString(),
                    TotalHeight = totalHeight.ToString(),
                    TrimTriangles = kit.KitSpecification.TrimTriangles,
                    BorderWidth = kit.KitSpecification.BorderWidth.ToString(),
                    BorderFabricStyle = MCommon_FabricStyle(kit.KitSpecification.BorderFabricStyle, InventoryMicroService),
                    BindingWidth = kit.KitSpecification.BindingWidth.ToString(),
                    BindingFabricStyle = MCommon_FabricStyle(kit.KitSpecification.BindingFabricStyle, InventoryMicroService),
                    HasBacking = kit.KitSpecification.HasBacking,
                    BackingFabricStyle = MCommon_FabricStyle(kit.KitSpecification.BackingFabricStyle, InventoryMicroService)
                };

                return specification;
            }

            private static MCommon_Color MCommon_Color(int hue, int saturation, int value)
            {
                var color = Color.FromAhsb(255, hue, saturation / 100.0, value / 100.0);

                var result = new MCommon_Color()
                {
                    Hue = hue,
                    Saturation = saturation,
                    Value = value,
                    WebColor = color.WebColor
                };

                return result;
            }

            private static MCommon_Color MCommon_Color(Color color)
            {
                var result = new MCommon_Color()
                {
                    Hue = (int)color.Hue,
                    Saturation = (int)(color.Saturation * 100),
                    Value = (int)(color.Brightness * 100),
                    WebColor = color.WebColor
                };

                return result;
            }

            private static MCommon_FabricStyle MCommon_FabricStyle(FabricStyle fabricStyle, IInventoryMicroService InventoryMicroService)
            {
                var result = new MCommon_FabricStyle();

                var entry = InventoryMicroService.GetEntry(fabricStyle.Sku);

                result.Sku = fabricStyle.Sku;
                result.Color = MCommon_Color(fabricStyle.Color);
                result.Name = entry.Name;

                return result;
            }

            private static MKit_KitSpecificationOptions MKit_KitSpecificationOptions(Kit kit)
            {
                var standardBorderWidths = new MKit_Dimension[]
                {
                    new MKit_Dimension()
                    {
                        Id = "2", Length = @"2""", Preferred = true
                    },
                    new MKit_Dimension()
                    {
                        Id = "4", Length = @"4""", Preferred = false
                    },
                    new MKit_Dimension()
                    {
                        Id = "6", Length = @"6""", Preferred = false
                    }
                };

                var standardBindingWidths = new MKit_Dimension[]
                {
                    new MKit_Dimension()
                    {
                        Id = "2.5", Length = @"2.5""", Preferred = true
                    },
                    new MKit_Dimension()
                    {
                        Id = "2.25", Length = @"2.25""", Preferred = false
                    }
                };

                return new MKit_KitSpecificationOptions()
                {
                    StandardSizes = MKit_Size(kit.Design.GetStandardSizes()),
                    StandardBorderWidths = standardBorderWidths,
                    StandardBindingWidths = standardBindingWidths
                };
            }

            private static MKit_KitBuildItemImage MKit_KitBuildItemImage(KitBuildItem kitBuildItem, DimensionScale scale)
            {
                var image = kitBuildItem.CreateImage(scale);
                if (image != null)
                {
                    try
                    {
                        using var ms = new MemoryStream();

                        image.Save(ms, ImageFormat.Png);

                        var result = new MKit_KitBuildItemImage()
                        {
                            Width = image.Width,
                            Height = image.Height,
                            Image = ms.ToArray()
                        };

                        return result;
                    }
                    finally
                    {
                        image.Dispose();
                    }
                }

                return null;
            }

            private static Color CreateColor(MCommon_Color color)
            {
                return Color.FromWebColor(color.WebColor);
            }

            public static FabricStyle CreateFabricStyle(MCommon_FabricStyle fabricStyleData, IInventoryMicroService InventoryMicroService)
            {
                FabricStyle result;

                if (fabricStyleData.Color != null)
                {
                    result = new FabricStyle(fabricStyleData.Sku, CreateColor(fabricStyleData.Color));
                }
                else
                {
                    var item = InventoryMicroService.GetEntry(fabricStyleData.Sku);
                    var color = MCommon_Color(item.Hue, item.Saturation, item.Value);
                    result = new FabricStyle(fabricStyleData.Sku, CreateColor(color));
                }

                return result;
            }

            public static MKit_Kit MKit_Kit(MDesign_Design mDesign, int thumbnailSize, IInventoryMicroService InventoryMicroService)
            {
                var design = new Design.Core.Design(JToken.Parse(mDesign.DesignArtifactValue));

                var kit = new Kit(design, new KitSpecification());

                return MKit_Kit(kit, "New", null, mDesign.DesignId, thumbnailSize, InventoryMicroService);
            }

            public static MKit_Kit MKit_Kit(Kit kit, string kitName, Guid? kitId, Guid? designId, int thumbnailSize, IInventoryMicroService InventoryMicroService)
            {
                var specification = MKit_KitSpecification(kit, InventoryMicroService);

                var specificationOptions = MKit_KitSpecificationOptions(kit);

                var kitParts = MKit_KitParts(kit, InventoryMicroService);

                //Logger.LogMessage("ServiceDataFactory::Create_Kit_DetailData - Kit parts created.");

                var kitBuildSteps = MKit_KitBuildSteps(kit);

                //Logger.LogMessage("ServiceDataFactory::Create_Kit_DetailData - Kit build steps created.");

                byte[] image;
                var renderer = new DesignRenderer();
                using (var bitmap = renderer.CreateBitmap(kit, thumbnailSize))
                {
                    using var ms = new MemoryStream();

                    bitmap.Save(ms, ImageFormat.Png);
                    image = ms.ToArray();
                }

                //Logger.LogMessage("ServiceDataFactory::Create_Kit_DetailData - Kit rendered.");

                var result = new MKit_Kit()
                {
                    ProjectId = kitId,
                    DesignId = designId,
                    ProjectName = kitName,
                    Specification = specification,
                    SpecificationOptions = specificationOptions,
                    Parts = kitParts,
                    BuildSteps = kitBuildSteps,
                    Image = image
                };

                return result;
            }
        }
    }
}