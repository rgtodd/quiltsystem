//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Drawing;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Core
{
    public abstract class KitBuildItem
    {
        private static readonly Dimension DefaultDimension = new Dimension(0, DimensionUnits.Inch);

        private readonly Area m_area;
        private readonly string m_buildItemSubtype;
        private readonly string m_buildItemType;
        private readonly FabricStyleList m_fabricStyles;
        private readonly string m_id;
        private string m_partId;
        private readonly int m_quantity;
        private readonly string m_type;

        protected KitBuildItem(string type, string id, int quantity, string buildItemType, string buildItemSubtype, FabricStyleList fabricStyles, Area area)
        {
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrEmpty(buildItemType)) throw new ArgumentNullException(nameof(buildItemType));
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

            m_type = type;
            m_id = id;
            m_buildItemType = buildItemType;
            m_buildItemSubtype = buildItemSubtype;
            m_quantity = quantity;
            m_fabricStyles = fabricStyles != null ? fabricStyles.Clone() : new FabricStyleList();
            m_area = area;
        }

        protected KitBuildItem(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_type = (string)json[JsonNames.TypeName];
            m_id = (string)json[JsonNames.Id];
            m_buildItemType = (string)json[JsonNames.Name];
            m_buildItemSubtype = json.Value<string>(JsonNames.Style);
            m_quantity = (int)json[JsonNames.Quantity];
            m_fabricStyles = new FabricStyleList(json[JsonNames.FabricStyleList]);
            m_partId = json.Value<string>(JsonNames.PartId);

            var width = GetDimension(json[JsonNames.Width], DefaultDimension);
            var height = GetDimension(json[JsonNames.Height], DefaultDimension);
            m_area = new Area(width, height);
        }

        protected KitBuildItem(KitBuildItem prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_type = prototype.m_type;
            m_id = prototype.m_id;
            m_buildItemType = prototype.m_buildItemType;
            m_buildItemSubtype = prototype.m_buildItemSubtype;
            m_quantity = prototype.m_quantity;
            m_fabricStyles = prototype.m_fabricStyles.Clone();
            m_partId = prototype.m_partId;
            m_area = prototype.m_area;
        }

        public Area Area
        {
            get
            {
                return m_area;
            }
        }

        public string BuildItemSubtype
        {
            get
            {
                return m_buildItemSubtype;
            }
        }

        public string BuildItemType
        {
            get
            {
                return m_buildItemType;
            }
        }

        public FabricStyleList FabricStyles
        {
            get
            {
                return m_fabricStyles;
            }
        }

        public string Id
        {
            get
            {
                return m_id;
            }
        }

        public string PartId
        {
            get
            {
                return m_partId;
            }
            set
            {
                m_partId = value;
            }
        }

        public int Quantity
        {
            get
            {
                return m_quantity;
            }
        }

        public string Type
        {
            get
            {
                return m_type;
            }
        }

        public static KitBuildItem Create(JToken json)
        {
            var typeName = (string)json[JsonNames.TypeName];

            if (string.IsNullOrEmpty(typeName))
            {
                typeName = KitBuildItemNode.TypeName;
            }

            switch (typeName)
            {
                case KitBuildItemNode.TypeName:
                    return new KitBuildItemNode(json);

                case KitBuildItemPattern.TypeName:
                    return new KitBuildItemPattern(json);

                default:
                    return new KitBuildItemNode(json); // HACK: Unknown type name.
                    throw new InvalidOperationException(string.Format("Unknown type name {0}.", typeName));
            }
        }

        public abstract KitBuildItem Clone();

        public abstract Image CreateImage(DimensionScale scale);

        public virtual JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.TypeName, m_type),
                new JProperty(JsonNames.Id, m_id),
                new JProperty(JsonNames.Name, m_buildItemType),
                new JProperty(JsonNames.Style, m_buildItemSubtype),
                new JProperty(JsonNames.Quantity, m_quantity),
                new JProperty(JsonNames.FabricStyleList, m_fabricStyles.JsonSave()),
                new JProperty(JsonNames.Width, m_area.Width.ToString()),
                new JProperty(JsonNames.Height, m_area.Height.ToString()),
                new JProperty(JsonNames.PartId, m_partId)
            };

            return result;
        }

        private Dimension GetDimension(JToken token, Dimension defaultValue)
        {
            return token != null
                ? Dimension.Parse((string)token) :
                defaultValue;
        }
    }
}