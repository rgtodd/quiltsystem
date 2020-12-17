//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Build;
using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Path;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class Kit
    {
        private KitBuildStepList m_buildSteps; // derived -- see RefreshOutputs
        private Design m_design;
        private KitSpecification m_kitSpecification;
        private KitPartList m_parts; // derived -- see RefreshOuputs

        public Kit(Design design, KitSpecification kitSpecification)
        {
            m_design = design ?? throw new ArgumentNullException(nameof(design));
            m_kitSpecification = kitSpecification ?? throw new ArgumentNullException(nameof(kitSpecification));
        }

        public Kit(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            var jsonDesign = json[JsonNames.Design];
            if (jsonDesign != null)
            {
                m_design = new Design(jsonDesign);
            }

            var jsonKitSpecification = json[JsonNames.KitSpecification];
            if (jsonKitSpecification != null)
            {
                m_kitSpecification = new KitSpecification(jsonKitSpecification);
            }
        }

        protected Kit(Kit prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_design = prototype.m_design.Clone();
            m_kitSpecification = prototype.m_kitSpecification.Clone();
        }

        public KitBuildStepList BuildSteps
        {
            get
            {
                if (m_buildSteps == null)
                {
                    RefreshOutputs();
                }
                return m_buildSteps;
            }
        }

        public Design Design
        {
            get
            {
                return m_design;
            }
            set
            {
                m_design = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public KitSpecification KitSpecification
        {
            get
            {
                return m_kitSpecification;
            }
            set
            {
                m_kitSpecification = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public KitPartList Parts
        {
            get
            {
                if (m_parts == null)
                {
                    RefreshOutputs();
                }
                return m_parts;
            }
        }

        public Kit Clone()
        {
            return new Kit(this);
        }

        public Node Expand()
        {
            return m_kitSpecification.Expand(m_design);
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.Design, m_design.JsonSave()),
                new JProperty(JsonNames.KitSpecification, m_kitSpecification.JsonSave())
            };

            return result;
        }

        private KitBuildItem CreateKitBuildItem(IBuildComponent buildComponent)
        {
            return buildComponent is BuildComponentYardage buildComponentYardage
                ? CreateKitBuildItemPattern(buildComponentYardage)
                : CreateKitBuildItemNode(buildComponent);
        }

        private KitBuildItem CreateKitBuildItemNode(IBuildComponent buildComponent)
        {
            var fabricStyles = new FabricStyleList();
            foreach (var style in buildComponent.FabricStyles)
            {
                fabricStyles.Add(new FabricStyle(style.Sku, style.Color));
            }

            return new KitBuildItemNode(buildComponent.Id, buildComponent.Quantity, buildComponent.ComponentType, buildComponent.ComponentSubtype, fabricStyles, buildComponent.Area, buildComponent.Node);
        }

        private KitBuildItem CreateKitBuildItemPattern(BuildComponentYardage buildComponent)
        {
            var fabricStyles = new FabricStyleList();
            foreach (var style in buildComponent.FabricStyles)
            {
                fabricStyles.Add(new FabricStyle(style.Sku, style.Color));
            }

            var pattern = new Pattern(buildComponent.Area);
            foreach (var region in buildComponent.Regions)
            {
                var patternElement = new PatternElement(
                    PatternElementTypes.Shape,
                    new PathPoint(region.Left, region.Top),
                    new PathPoint(region.Left + region.Width, region.Top + region.Height),
                    region.BuildComponentRectangle.Id
                    );

                pattern.PatternElements.Add(patternElement);
            }

            return new KitBuildItemPattern(buildComponent.Id, buildComponent.Quantity, buildComponent.ComponentType, buildComponent.ComponentSubtype, fabricStyles, buildComponent.Area, pattern);
        }

        private void RefreshOutputs()
        {
            var builder = new Builder(KitSpecification, Design);
            var buildPlan = builder.Create();

            m_buildSteps = new KitBuildStepList();
            m_parts = new KitPartList();

            var yardages = new List<KitBuildItem>();

            foreach (var buildStep in buildPlan.BuildSteps)
            {
                var kitBuildStep = new KitBuildStep(buildStep.StepNumber, buildStep.Description);
                m_buildSteps.Add(kitBuildStep);

                foreach (var input in buildStep.Consumes)
                {
                    var kitBuildItem = CreateKitBuildItem(input);
                    kitBuildStep.Consumes.Add(kitBuildItem);

                    if (kitBuildItem.BuildItemType == BuildComponentTypes.Yardage)
                    {
                        yardages.Add(kitBuildItem);
                    }
                }

                foreach (var output in buildStep.Produces)
                {
                    kitBuildStep.Produces.Add(CreateKitBuildItem(output));
                }
            }

            var partIndex = 0;
            foreach (var yardage in yardages)
            {
                var areaSize = Area.GetSmallestContainingStandardArea(yardage.Area);
                var part = m_parts.Where(r => r.Sku == yardage.FabricStyles[0].Sku && r.AreaSize == areaSize).SingleOrDefault();
                if (part == null)
                {
                    partIndex += 1;

                    part = new KitPart(
                        "P." + partIndex,
                        yardage.FabricStyles[0].Sku,
                        areaSize,
                        1,
                        yardage.FabricStyles[0].Color);
                    m_parts.Add(part);
                }
                else
                {
                    part.Quantity += 1;
                }

                yardage.PartId = part.Id;
            }
        }
    }
}