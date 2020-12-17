//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Component;
using RichTodd.QuiltSystem.Design.Component.Standard;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class Design
    {
        public const string WidthParameter = "Width";
        public const string HeightParameter = "Height";

        private LayoutComponent m_layoutComponent;
        private readonly DesignParameterCollection m_parameters;

        public Design()
        {
            m_parameters = new DesignParameterCollection();
        }

        public Design(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            var jsonLayoutComponent = json[JsonNames.LayoutComponent];
            if (jsonLayoutComponent != null)
            {
                m_layoutComponent = (LayoutComponent)ComponentFactory.Singleton.Create(jsonLayoutComponent);
            }

            var jsonBlockComponents = json[JsonNames.BlockComponents];
            if (jsonBlockComponents != null)
            {
                var blockComponents = new ComponentList(jsonBlockComponents);
                foreach (var blockComponent in blockComponents)
                {
                    m_layoutComponent.Children.Add(blockComponent);
                }
            }

            var jsonParameters = json[JsonNames.Parameters];
            m_parameters = jsonParameters != null
                ? new DesignParameterCollection(jsonParameters)
                : new DesignParameterCollection();
        }

        protected Design(Design prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            if (prototype.m_layoutComponent != null)
            {
                m_layoutComponent = (LayoutComponent)prototype.m_layoutComponent.Clone();
            }

            m_parameters = prototype.m_parameters.Clone();
        }

        public Design Clone()
        {
            return new Design(this);
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.Parameters, m_parameters.JsonSave())
            };

            if (m_layoutComponent != null)
            {
                result[JsonNames.LayoutComponent] = m_layoutComponent.JsonSave();
            }

            return result;
        }

        public LayoutComponent LayoutComponent
        {
            get
            {
                return m_layoutComponent;
            }

            set
            {
                m_layoutComponent = value;
            }
        }

        public DesignParameterCollection Parameters
        {
            get
            {
                return m_parameters;
            }
        }

        public Dimension? Width
        {
            get
            {
                var value = Parameters[WidthParameter];
                return !string.IsNullOrEmpty(value)
                    ? (Dimension?)Dimension.Parse(value)
                    : null;
            }
            set
            {
                Parameters[WidthParameter] = value.HasValue
                    ? value.Value.ToString()
                    : null;
            }
        }

        public Dimension? Height
        {
            get
            {
                var value = Parameters[HeightParameter];
                return !string.IsNullOrEmpty(value)
                    ? (Dimension?)Dimension.Parse(value)
                    : null;
            }
            set
            {
                Parameters[HeightParameter] = value.HasValue
                    ? value.Value.ToString()
                    : null;
            }
        }

        public List<DesignSize> GetStandardSizes()
        {
            var result = new List<DesignSize>();

            if (LayoutComponent != null)
            {
                result.Add(GetStandardSize("12"));
                result.Add(GetStandardSize("8"));
                result.Add(GetStandardSize("6"));
            }

            return result;
        }

        public DesignSize GetStandardSize(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            switch (id)
            {
                case "12":
                    {
                        var dimension = new Dimension(12, DimensionUnits.Inch);
                        return new DesignSize("12", dimension * LayoutComponent.ColumnCount, dimension * LayoutComponent.RowCount, "12-Inch Block", true);
                    }

                case "8":
                    {
                        var dimension = new Dimension(8, DimensionUnits.Inch);
                        return new DesignSize("8", dimension * LayoutComponent.ColumnCount, dimension * LayoutComponent.RowCount, "8-Inch Block", false);
                    }

                case "6":
                    {
                        var dimension = new Dimension(6, DimensionUnits.Inch);
                        return new DesignSize("6", dimension * LayoutComponent.ColumnCount, dimension * LayoutComponent.RowCount, "6-Inch Block", false);
                    }
            }

            throw new ArgumentOutOfRangeException(nameof(id), string.Format("Unknown size {0}.", id));
        }
    }
}
