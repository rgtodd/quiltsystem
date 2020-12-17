//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;

using RichTodd.QuiltSystem.Business.Libraries;
using RichTodd.QuiltSystem.Design.Component;
using RichTodd.QuiltSystem.Design.Component.Standard;
using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Primitives;
using RichTodd.QuiltSystem.Service.Database.Abstractions;

namespace RichTodd.QuiltSystem.Business.ComponentProviders
{
    public class DatabaseBlockComponentProvider : IComponentProvider
    {
        public const string ResourceTypePrefix = "Component/";

        private readonly IQuiltContextFactory m_quiltContextFactory;

        public DatabaseBlockComponentProvider(IQuiltContextFactory quiltContextFactory)
        {
            m_quiltContextFactory = quiltContextFactory;
        }

        public List<ComponentProviderEntry> GetComponents(string type, string category)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (category == null) throw new ArgumentNullException(nameof(category));

            var result = new List<ComponentProviderEntry>();

            var resourceLibrary = DatabaseResourceLibrary.Load(m_quiltContextFactory, category);
            foreach (var resourceLibraryEntry in resourceLibrary.GetEntries().Where(r => r.Type == ResourceTypePrefix + type))
            {
                var entry = CreateEntry(category, resourceLibraryEntry);
                result.Add(entry);
            }

            return result;
        }

        public ComponentProviderEntry GetComponent(string type, string category, string name)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (category == null) throw new ArgumentNullException(nameof(category));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var resourceLibrary = DatabaseResourceLibrary.Load(m_quiltContextFactory, category);
            var resourceLibraryEntry = resourceLibrary.GetEntry(name);

            if (resourceLibraryEntry == null || resourceLibraryEntry.Type != ResourceTypePrefix + type)
            {
                return null;
            }

            var entry = CreateEntry(category, resourceLibraryEntry);

            return entry;
        }

        private static ComponentProviderEntry CreateEntry(string category, ResourceLibraryEntry resourceLibraryEntry)
        {
            var type = resourceLibraryEntry.Type[ResourceTypePrefix.Length..];

            var node = ResourceNodeFactory.Create(resourceLibraryEntry);

            NodeStyler.Style(node, Themes.Rainbow);
            var fabricStyles = node.GetFabricStyles();
            fabricStyles.Sort();

            var component = type switch
            {
                BlockComponent.TypeName => BlockComponent.Create(category, resourceLibraryEntry.Name, fabricStyles),
                _ => throw new InvalidOperationException(string.Format("Unsupported resource type {0}.", type)),
            };

            var entry = new ComponentProviderEntry()
            {
                Type = type,
                Category = category,
                Name = resourceLibraryEntry.Name,
                Tags = resourceLibraryEntry.Tags,
                Component = component
            };

            return entry;
        }
    }
}