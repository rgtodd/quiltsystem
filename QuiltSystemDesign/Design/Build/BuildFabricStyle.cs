//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    // Immutable class
    //
    internal class BuildFabricStyle
    {
        private readonly Color m_color;
        private readonly string m_sku;

        public BuildFabricStyle(FabricStyle fabricStyle)
        {
            if (fabricStyle == null) throw new ArgumentNullException(nameof(fabricStyle));

            m_sku = fabricStyle.Sku;
            m_color = fabricStyle.Color;
        }

        public Color Color
        {
            get
            {
                return m_color;
            }
        }

        public string Key
        {
            get
            {
                return Sku;
            }
        }

        public string Sku
        {
            get
            {
                return m_sku;
            }
        }
    }
}