//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Design.Core;
using RichTodd.QuiltSystem.Design.Primitives;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Business.Libraries
{
    public static class ProjectLibraryKitUtility
    {
        public static Kit CreateKit(MProject_Project mProject)
        {
            var kit = new Kit(JToken.Parse(mProject.Specification.ProjectArtifactValue));
            if (kit.Design == null)
            {
                kit.Design = new Design.Core.Design(JToken.Parse(mProject.Specification.DesignArtifactValue));
            }
            if (kit.KitSpecification == null)
            {
                kit.KitSpecification = new KitSpecification();
            }

            return kit;
        }

        public static MProject_ProjectSpecification CreateProjectSpecification(Design.Core.Design design)
        {
            var kit = new Kit(design, new KitSpecification());

            var projectComponents = new List<MProject_ProjectSpecificationComponent>();
            foreach (var kitPart in kit.Parts)
            {
                projectComponents.Add(
                    new MProject_ProjectSpecificationComponent(kitPart.Sku, GetUnitOfMeasureCode(kitPart.AreaSize), kitPart.Quantity));
            }

            var projectSpecification = new MProject_ProjectSpecification(
                design.JsonSave().ToString(),
                ArtifactTypeCodes.Kit,
                ArtifactValueTypeCodes.Json,
                kit.JsonSave().ToString(),
                projectComponents);

            return projectSpecification;
        }

        public static MProject_ProjectSpecification CreateProjectSpecification(Kit kit)
        {
            var projectComponents = new List<MProject_ProjectSpecificationComponent>();
            foreach (var kitPart in kit.Parts)
            {
                projectComponents.Add(
                    new MProject_ProjectSpecificationComponent(kitPart.Sku, GetUnitOfMeasureCode(kitPart.AreaSize), kitPart.Quantity));
            }

            var projectSpecification = new MProject_ProjectSpecification(
                null,
                ArtifactTypeCodes.Kit,
                ArtifactValueTypeCodes.Json,
                kit.JsonSave().ToString(),
                projectComponents);

            return projectSpecification;
        }

        private static string GetUnitOfMeasureCode(AreaSizes areaSize)
        {
            return areaSize switch
            {
                AreaSizes.FatQuarter => UnitOfMeasureCodes.FatQuarter,
                AreaSizes.HalfYard => UnitOfMeasureCodes.HalfYardage,
                AreaSizes.Yard => UnitOfMeasureCodes.Yardage,
                AreaSizes.TwoYards => UnitOfMeasureCodes.TwoYards,
                AreaSizes.ThreeYards => UnitOfMeasureCodes.ThreeYards,
                _ => throw new InvalidOperationException(string.Format("Unknown area size {0}", areaSize)),
            };
        }
    }
}