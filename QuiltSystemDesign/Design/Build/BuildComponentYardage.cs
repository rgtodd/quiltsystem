//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Text;

using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class BuildComponentYardage : BuildComponent
    {
        private readonly Area m_area;
        private readonly AreaSizes m_areaSize;
        private readonly FabricStyle[] m_fabricStyles;
        private readonly IList<BuildComponentYardageRegion> m_regions;
        private readonly string m_styleKey;

        public BuildComponentYardage(string id, FabricStyle fabricStyle, AreaSizes areaSize)
            : base(id)
        {
            if (fabricStyle == null) throw new ArgumentNullException(nameof(fabricStyle));

            m_fabricStyles = new FabricStyle[] { fabricStyle };

            m_areaSize = areaSize;
            m_area = Area.Create(areaSize);

            m_styleKey = CreateStyleKey(fabricStyle, areaSize);

            m_regions = new List<BuildComponentYardageRegion>();
        }

        public override Area Area
        {
            get
            {
                return m_area;
            }
        }

        public AreaSizes AreaSize
        {
            get
            {
                return m_areaSize;
            }
        }

        public override string ComponentSubtype
        {
            get
            {
                return m_areaSize switch
                {
                    AreaSizes.FatQuarter => "Fat Quarter",
                    AreaSizes.HalfYard => "Half Yard",
                    AreaSizes.Yard => "Yard",
                    AreaSizes.TwoYards => "Two Yards",
                    AreaSizes.ThreeYards => "Three Yards",
                    _ => throw new InvalidOperationException(string.Format("Unknown area size {0}.", m_areaSize)),
                };
            }
        }

        public override string ComponentType
        {
            get
            {
                return BuildComponentTypes.Yardage;
            }
        }

        public FabricStyle FabricStyle
        {
            get
            {
                return m_fabricStyles[0];
            }
        }

        public override IReadOnlyList<FabricStyle> FabricStyles
        {
            get
            {
                return m_fabricStyles;
            }
        }

        public override Node Node
        {
            get
            {
                return null;
            }
        }

        public IList<BuildComponentYardageRegion> Regions
        {
            get
            {
                return m_regions;
            }
        }

        public override string StyleKey
        {
            get
            {
                return m_styleKey;
            }
        }

        public static string CreateStyleKey(FabricStyle fabricStyle, AreaSizes areaSize)
        {
            var sb = new StringBuilder();

            sb.Append(typeof(BuildComponentYardage).Name);
            sb.Append(StyleKeyDelimiter);
            sb.Append(fabricStyle.Sku);
            sb.Append(StyleKeyDelimiter);
            sb.Append(areaSize.ToString());

            return sb.ToString();
        }

        protected override IBuildComponent Clone(BuildComponentFactory factory)
        {
            return factory.CreateBuildComponentYardage(FabricStyle, AreaSize);
        }
    }
}