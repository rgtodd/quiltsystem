//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace RichTodd.QuiltSystem.Design.Nodes
{
    internal class LayoutSiteList : List<LayoutSite>
    {
        private readonly LayoutNode m_layout;

        public LayoutSiteList(LayoutNode layout)
        {
            m_layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public LayoutSiteList(LayoutNode layout, JToken json)
        {
            m_layout = layout ?? throw new ArgumentNullException(nameof(layout));

            foreach (var jsonLayoutSite in json ?? throw new ArgumentNullException(nameof(json)))
            {
                Add(new LayoutSite(m_layout, jsonLayoutSite));
            }
        }

        protected LayoutSiteList(LayoutNode layout, IList<LayoutSite> prototype)
        {
            if (prototype == null) throw new ArgumentNullException(nameof(prototype));

            m_layout = layout ?? throw new ArgumentNullException(nameof(layout));

            foreach (var layoutSite in prototype)
            {
                Add(layoutSite.Clone(m_layout));
            }
        }

        public LayoutSiteList Clone(LayoutNode layout)
        {
            if (layout == null) throw new ArgumentNullException(nameof(layout));

            return new LayoutSiteList(layout, this);
        }

        public JToken JsonSave()
        {
            var jsonLayoutSites = new JArray();
            foreach (var layoutSite in this)
            {
                jsonLayoutSites.Add(layoutSite.JsonSave());
            }

            return jsonLayoutSites;
        }
    }
}