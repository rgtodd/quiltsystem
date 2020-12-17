//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Web.Mvc.Models
{
    public class KitDetailVcModel
    {
        public static KitDetailVcModel Create(MKit_Kit kit)
        {
            return Factory.CreateKitDetailModel(kit);
        }

        public Guid? ProjectId { get; set; }

        public Guid? DesignId { get; set; }

        public string KitName { get; set; }

        public byte[] KitImage { get; set; }

        public KitDetailSpecificationVcModel Specification { get; set; }

        public List<KitDetailPartVcModel> Parts { get; set; }

        public List<KitDetailBuildStepVcModel> BuildSteps { get; set; }

        private static class Factory
        {
            public static KitDetailVcModel CreateKitDetailModel(MKit_Kit kit)
            {
                var parts = CreateKitDetailPartModels(kit);
                var skuNames = CreateSkuNames(parts);
                var buildSteps = CreateKitDetailBuildStepModels(kit, skuNames);
                var specification = CreateKitDetailSpecificationModel(kit, skuNames);

                var result = new KitDetailVcModel()
                {
                    ProjectId = kit.ProjectId,
                    DesignId = kit.DesignId,
                    KitName = kit.ProjectName,
                    KitImage = kit.Image,
                    Specification = specification,
                    Parts = parts,
                    BuildSteps = buildSteps
                };

                return result;
            }

            private static List<KitDetailBuildStepVcModel> CreateKitDetailBuildStepModels(MKit_Kit kit, Dictionary<string, string> skuNames)
            {
                var buildSteps = new List<KitDetailBuildStepVcModel>();
                {
                    foreach (var serviceBuildStep in kit.BuildSteps)
                    {
                        var consumes = new List<KitDetailBuildItemVcModel>();
                        foreach (var input in serviceBuildStep.Consumes)
                        {
                            string skuName1 = null;
                            if (input.Sku1 != null && skuNames.ContainsKey(input.Sku1)) skuName1 = skuNames[input.Sku1];

                            string skuName2 = null;
                            if (input.Sku2 != null && skuNames.ContainsKey(input.Sku2)) skuName2 = skuNames[input.Sku2];

                            consumes.Add(new KitDetailBuildItemVcModel()
                            {
                                Id = input.Id,
                                Name = input.Name,
                                Quantity = input.Quantity,
                                Width = input.Width,
                                Height = input.Height,
                                Sku1 = input.Sku1,
                                Sku2 = input.Sku2,
                                SkuName1 = skuName1,
                                SkuName2 = skuName2,
                                WebColor1 = input.WebColor1,
                                WebColor2 = input.WebColor2,
                                PartId = input.PartId,
                                Image = input.Image?.Image,
                                ImageWidth = input.Image?.Width ?? 0,
                                ImageHeight = input.Image?.Height ?? 0
                            });
                        }

                        var produces = new List<KitDetailBuildItemVcModel>();
                        foreach (var output in serviceBuildStep.Produces)
                        {
                            string skuName1 = null;
                            if (output.Sku1 != null && skuNames.ContainsKey(output.Sku1)) skuName1 = skuNames[output.Sku1];

                            string skuName2 = null;
                            if (output.Sku2 != null && skuNames.ContainsKey(output.Sku2)) skuName2 = skuNames[output.Sku2];

                            produces.Add(new KitDetailBuildItemVcModel()
                            {
                                Id = output.Id,
                                Name = output.Name,
                                Quantity = output.Quantity,
                                Width = output.Width,
                                Height = output.Height,
                                Sku1 = output.Sku1,
                                Sku2 = output.Sku2,
                                SkuName1 = skuName1,
                                SkuName2 = skuName2,
                                WebColor1 = output.WebColor1,
                                WebColor2 = output.WebColor2,
                                Image = output.Image?.Image,
                                ImageWidth = output.Image?.Width ?? 0,
                                ImageHeight = output.Image?.Height ?? 0
                            });
                        }

                        var buildStep = new KitDetailBuildStepVcModel()
                        {
                            Description = serviceBuildStep.Description,
                            Consumes = consumes,
                            Produces = produces
                        };
                        buildSteps.Add(buildStep);
                    }
                }

                return buildSteps;
            }

            private static List<KitDetailPartVcModel> CreateKitDetailPartModels(MKit_Kit kit)
            {
                var parts = new List<KitDetailPartVcModel>();
                {
                    foreach (var servicePart in kit.Parts)
                    {
                        var part = new KitDetailPartVcModel()
                        {
                            Id = servicePart.Id,
                            Sku = servicePart.Sku,
                            Size = GetUnitOfMeasureText(servicePart.UnitOfMeasure),
                            Quantity = servicePart.Quantity,
                            WebColor = servicePart.WebColor,
                            Description = servicePart.Description,
                            Manufacturer = servicePart.Manufacturer,
                            Collection = servicePart.Collection
                        };
                        parts.Add(part);
                    }
                }

                return parts;
            }

            private static KitDetailSpecificationVcModel CreateKitDetailSpecificationModel(MKit_Kit kit, Dictionary<string, string> skuNames)
            {
                string border = kit.Specification.BorderWidth != @"0"""
                    ? skuNames[kit.Specification.BorderFabricStyle.Sku] + " (" + kit.Specification.BorderWidth + ")"
                    : "(None)";

                string binding = kit.Specification.BindingWidth != @"0"""
                    ? skuNames[kit.Specification.BindingFabricStyle.Sku] + " (" + kit.Specification.BindingWidth + ")"
                    : "(None)";

                string backing = kit.Specification.HasBacking
                    ? skuNames[kit.Specification.BackingFabricStyle.Sku]
                    : "(None)";

                var result = new KitDetailSpecificationVcModel()
                {
                    Size = kit.Specification.TotalWidth + " X " + kit.Specification.TotalHeight,
                    Border = border,
                    Binding = binding,
                    Backing = backing,
                    TrimTriangles = kit.Specification.TrimTriangles
                };

                return result;
            }

            private static Dictionary<string, string> CreateSkuNames(List<KitDetailPartVcModel> parts)
            {
                var skuNames = new Dictionary<string, string>();

                foreach (var part in parts)
                {
                    if (!skuNames.ContainsKey(part.Sku))
                    {
                        skuNames.Add(part.Sku, part.Description);
                    }
                }

                return skuNames;
            }

            private static string GetUnitOfMeasureText(MKit_KitPartUnitOfMeasures unitOfMeasure)
            {
                return unitOfMeasure switch
                {
                    MKit_KitPartUnitOfMeasures.FatQuarter => "Fat Quarter",
                    MKit_KitPartUnitOfMeasures.HalfYard => "Half Yard",
                    MKit_KitPartUnitOfMeasures.ThreeYards => "Three Yard",
                    MKit_KitPartUnitOfMeasures.TwoYards => "Two Yard",
                    MKit_KitPartUnitOfMeasures.Yard => "Yard",
                    _ => "?",
                };
            }

        }
    }
}