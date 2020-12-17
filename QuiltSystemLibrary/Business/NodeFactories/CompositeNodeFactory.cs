//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using RichTodd.QuiltSystem.Design.Component;
using RichTodd.QuiltSystem.Design.Nodes;

namespace RichTodd.QuiltSystem.Business.NodeFactories
{
    public class CompositeNodeFactory : INodeFactory
    {
        private readonly List<INodeFactory> m_factories = new List<INodeFactory>();

        public CompositeNodeFactory()
        { }

        public void Add(INodeFactory factory)
        {
            m_factories.Add(factory);
        }

        public Node Create(Component component, NodeList childNodes)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));

            foreach (var factory in m_factories)
            {
                var node = factory.Create(component, childNodes);
                if (node != null)
                {
                    return node;
                }
            }

            return null;
        }
    }
}
