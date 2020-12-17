//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Nodes.Standard;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class KitSpecification
    {
        private static readonly Dimension DefaultWidth = new Dimension(48, DimensionUnits.Inch);
        private static readonly Dimension DefaultHeight = new Dimension(48, DimensionUnits.Inch);
        private static readonly Dimension DefaultBorderWidth = new Dimension(4, DimensionUnits.Inch);
        private static readonly Dimension DefaultBindingWidth = new Dimension(2, DimensionUnits.Inch);
        private const bool DefaultHasBacking = false;
        private const bool DefaultTrimTriangles = false;
        private static readonly FabricStyle DefaultBorderFabricStyle = new FabricStyle("K001-1019", Color.FromAhsb(255, 0.0, 0.0, 0.08));
        private static readonly FabricStyle DefaultBindingFabricStyle = new FabricStyle("K001-1019", Color.FromAhsb(255, 0.0, 0.0, 0.08));
        private static readonly FabricStyle DefaultBackingFabricStyle = new FabricStyle("K001-1019", Color.FromAhsb(255, 0.0, 0.0, 0.08));

        private Dimension m_width;
        private Dimension m_height;

        private Dimension m_borderWidth;
        private FabricStyle m_borderFabricStyle;

        private Dimension m_bindingWidth;
        private FabricStyle m_bindingFabricStyle;

        private bool m_hasBacking;
        private FabricStyle m_backingFabricStyle;

        private bool m_trimTriangles;

        public KitSpecification()
        {
            m_width = DefaultWidth;
            m_height = DefaultHeight;
            m_borderWidth = DefaultBorderWidth;
            m_bindingWidth = DefaultBindingWidth;
            m_hasBacking = DefaultHasBacking;
            m_trimTriangles = DefaultTrimTriangles;
            m_borderFabricStyle = DefaultBorderFabricStyle;
            m_bindingFabricStyle = DefaultBindingFabricStyle;
            m_backingFabricStyle = DefaultBackingFabricStyle;
        }

        public KitSpecification(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_width = GetDimension(json[JsonNames.Width], DefaultWidth);
            m_height = GetDimension(json[JsonNames.Height], DefaultHeight);
            m_borderWidth = GetDimension(json[JsonNames.BorderWidth], DefaultBorderWidth);
            m_bindingWidth = GetDimension(json[JsonNames.BindingWidth], DefaultBindingWidth);
            m_hasBacking = GetBoolean(json[JsonNames.HasBacking], DefaultHasBacking);
            m_trimTriangles = GetBoolean(json[JsonNames.TrimTriangles], DefaultTrimTriangles);
            m_borderFabricStyle = GetFabricStyle(json[JsonNames.BorderFabricStyle], DefaultBorderFabricStyle);
            m_bindingFabricStyle = GetFabricStyle(json[JsonNames.BindingFabricStyle], DefaultBindingFabricStyle);
            m_backingFabricStyle = GetFabricStyle(json[JsonNames.BackingFabricStyle], DefaultBackingFabricStyle);
        }

        protected KitSpecification(KitSpecification prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_width = prototype.m_width;
            m_height = prototype.m_height;
            m_borderWidth = prototype.m_borderWidth;
            m_bindingWidth = prototype.m_bindingWidth;
            m_hasBacking = prototype.m_hasBacking;
            m_trimTriangles = prototype.m_trimTriangles;
            m_borderFabricStyle = prototype.m_borderFabricStyle.Clone();
            m_bindingFabricStyle = prototype.m_bindingFabricStyle.Clone();
            m_backingFabricStyle = prototype.m_backingFabricStyle.Clone();
        }

        public KitSpecification Clone()
        {
            return new KitSpecification(this);
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.Width, m_width.ToString()),
                new JProperty(JsonNames.Height, m_height.ToString()),
                new JProperty(JsonNames.BorderWidth, m_borderWidth.ToString()),
                new JProperty(JsonNames.BindingWidth, m_bindingWidth.ToString()),
                new JProperty(JsonNames.HasBacking, m_hasBacking),
                new JProperty(JsonNames.TrimTriangles, m_trimTriangles),
                new JProperty(JsonNames.BorderFabricStyle, m_borderFabricStyle.JsonSave()),
                new JProperty(JsonNames.BindingFabricStyle, m_bindingFabricStyle.JsonSave()),
                new JProperty(JsonNames.BackingFabricStyle, m_backingFabricStyle.JsonSave())
            };

            return result;
        }

        public Dimension Width
        {
            get
            {
                return m_width;
            }
            set
            {
                m_width = value;
            }
        }

        public Dimension Height
        {
            get
            {
                return m_height;
            }
            set
            {
                m_height = value;
            }
        }

        public Dimension BorderWidth
        {
            get
            {
                return m_borderWidth;
            }
            set
            {
                m_borderWidth = value;
            }
        }

        public Dimension BindingWidth
        {
            get
            {
                return m_bindingWidth;
            }
            set
            {
                m_bindingWidth = value;
            }
        }

        public bool HasBacking
        {
            get
            {
                return m_hasBacking;
            }
            set
            {
                m_hasBacking = value;
            }
        }

        public bool TrimTriangles
        {
            get
            {
                return m_trimTriangles;
            }
            set
            {
                m_trimTriangles = value;
            }
        }

        public FabricStyle BorderFabricStyle
        {
            get
            {
                return m_borderFabricStyle;
            }
            set
            {
                m_borderFabricStyle = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public FabricStyle BindingFabricStyle
        {
            get
            {
                return m_bindingFabricStyle;
            }
            set
            {
                m_bindingFabricStyle = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public FabricStyle BackingFabricStyle
        {
            get
            {
                return m_backingFabricStyle;
            }
            set
            {
                m_backingFabricStyle = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public Node Expand(Design design)
        {
            var node = design.LayoutComponent.Expand(true);

            if (BorderWidth.Value == 0)
            {
                return node;
            }
            else
            {
                var borderLayoutNode = new BorderLayoutNode(BorderFabricStyle, BorderWidth);
                borderLayoutNode.LayoutSites[0].Node = node;

                return borderLayoutNode;
            }
        }

        private Dimension GetDimension(JToken token, Dimension defaultValue)
        {
            return token != null
                ? Dimension.Parse((string)token) 
                : defaultValue;
        }

        private FabricStyle GetFabricStyle(JToken token, FabricStyle defaultValue)
        {
            return token != null
                ? new FabricStyle(token) 
                : defaultValue;
        }

        private bool GetBoolean(JToken token, bool defaultValue)
        {
            return token != null
                ? (bool)token 
                : defaultValue;
        }
    }
}
