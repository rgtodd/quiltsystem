//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using RichTodd.QuiltSystem.Design.Component;

namespace RichTodd.QuiltSystem.Business.ComponentProviders
{
    class CompositeComponentProvider : IComponentProvider
    {
        private readonly List<IComponentProvider> m_providers;

        public CompositeComponentProvider()
        {
            m_providers = new List<IComponentProvider>();
        }

        public void Add(IComponentProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            m_providers.Add(provider);
        }

        public List<ComponentProviderEntry> GetComponents(string type, string category)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (category == null) throw new ArgumentNullException(nameof(category));

            var result = new List<ComponentProviderEntry>();

            foreach (var provider in m_providers)
            {
                result.AddRange(provider.GetComponents(type, category));
            }

            return result;
        }

        public ComponentProviderEntry GetComponent(string type, string category, string name)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (category == null) throw new ArgumentNullException(nameof(category));
            if (name == null) throw new ArgumentNullException(nameof(name));

            foreach (var provider in m_providers)
            {
                var result = provider.GetComponent(type, category, name);
                if (result != null) return result;
            }

            return null;
        }
    }
}
