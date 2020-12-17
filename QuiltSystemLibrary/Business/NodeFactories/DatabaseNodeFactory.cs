//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using RichTodd.QuiltSystem.Business.ComponentProviders;
using RichTodd.QuiltSystem.Business.Libraries;
using RichTodd.QuiltSystem.Design.Component;
using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Service.Database.Abstractions;

namespace RichTodd.QuiltSystem.Business.NodeFactories
{
    public class DatabaseNodeFactory : INodeFactory
    {
        private readonly IQuiltContextFactory m_quiltContextFactory;

        private readonly Dictionary<string, IResourceLibrary> m_resourceLibraries = new Dictionary<string, IResourceLibrary>();

        public DatabaseNodeFactory(IQuiltContextFactory quiltContextFactory)
        {
            m_quiltContextFactory = quiltContextFactory;
        }

        public Node Create(Component component, NodeList childNodes)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));

            var resourceLibrary = GetResourceLibrary(component.Category);
            var resourceLibraryEntry = resourceLibrary.GetEntry(component.Name);

            if (resourceLibraryEntry == null || resourceLibraryEntry.Type != DatabaseBlockComponentProvider.ResourceTypePrefix + component.Type)
            {
                return null;
            }

            var node = ResourceNodeFactory.Create(resourceLibraryEntry);

            NodeStyler.Style(node, component.FabricStyles);
            //NodeStyler.Style(node, Themes.Rainbow);

            return node;
        }

        private IResourceLibrary GetResourceLibrary(string category)
        {
            return !string.IsNullOrEmpty(category)
                ? m_resourceLibraries.TryGetValue(category, out IResourceLibrary resourceLibrary)
                    ? resourceLibrary
                    : GetResourceLibrarySync(category)
                : throw new ArgumentNullException(nameof(category));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private IResourceLibrary GetResourceLibrarySync(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (!m_resourceLibraries.TryGetValue(category, out IResourceLibrary resourceLibrary))
            {
                resourceLibrary = DatabaseResourceLibrary.Load(m_quiltContextFactory, category);
                m_resourceLibraries[category] = resourceLibrary;
            }

            return resourceLibrary;
        }
    }
}