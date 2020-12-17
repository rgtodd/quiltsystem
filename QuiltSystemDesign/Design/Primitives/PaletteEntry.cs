//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Primitives
{
    public class PaletteEntry
    {
        private FabricStyle m_fabricStyle;

        public PaletteEntry(FabricStyle fabricStyle)
        {
            m_fabricStyle = fabricStyle ?? throw new ArgumentNullException(nameof(fabricStyle));
        }

        public PaletteEntry(JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            m_fabricStyle = new FabricStyle(json[JsonNames.FabricStyle]);
        }

        protected PaletteEntry(PaletteEntry prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_fabricStyle = prototype.m_fabricStyle?.Clone();
        }

        public FabricStyle FabricStyle
        {
            get
            {
                return m_fabricStyle;
            }

            set
            {
                m_fabricStyle = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public PaletteEntry Clone()
        {
            return new PaletteEntry(this);
        }

        public JToken JsonSave()
        {
            var result = new JObject()
            {
                new JProperty(JsonNames.FabricStyle, m_fabricStyle.JsonSave())
            };

            return result;
        }
    }
}