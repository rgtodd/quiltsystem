//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Core
{
    public class KitPart
    {
        private readonly AreaSizes m_areaSize;
        private readonly Color m_color;
        private readonly string m_id;
        private int m_quantity;
        private readonly string m_sku;

        public KitPart(string id, string sku, AreaSizes areaSize, int quantity, Color color)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrEmpty(sku)) throw new ArgumentNullException(nameof(sku));
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

            m_id = id;
            m_sku = sku;
            m_areaSize = areaSize;
            m_quantity = quantity;
            m_color = color;
        }

        public KitPart(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_id = json.Value<string>(JsonNames.Id);
            m_sku = json.Value<string>(JsonNames.Sku);
            m_quantity = (int)json[JsonNames.Quantity];
            m_color = Color.FromArgb((int)json[JsonNames.Color]);

            var areaSize = (string)json[JsonNames.AreaSize];
            m_areaSize = !string.IsNullOrEmpty(areaSize)
                ? (AreaSizes)Enum.Parse(typeof(AreaSizes), areaSize)
                : AreaSizes.FatQuarter;
        }

        protected KitPart(KitPart prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_id = prototype.m_id;
            m_sku = prototype.m_sku;
            m_areaSize = prototype.m_areaSize;
            m_quantity = prototype.m_quantity;
            m_color = prototype.m_color;
        }

        public AreaSizes AreaSize
        {
            get
            {
                return m_areaSize;
            }
        }

        public Color Color
        {
            get
            {
                return m_color;
            }
        }

        public string Id
        {
            get
            {
                return m_id;
            }
        }

        public int Quantity
        {
            get
            {
                return m_quantity;
            }
            set
            {
                m_quantity = value;
            }
        }

        public string Sku
        {
            get
            {
                return m_sku;
            }
        }

        public KitPart Clone()
        {
            return new KitPart(this);
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.Id, m_id),
                new JProperty(JsonNames.Sku, m_sku),
                new JProperty(JsonNames.AreaSize, m_areaSize.ToString()),
                new JProperty(JsonNames.Quantity, m_quantity),
                new JProperty(JsonNames.Color, m_color.ToRgb())
            };

            return result;
        }
    }
}