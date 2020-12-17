//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Component
{
    internal class ComponentFactory
    {
        private static readonly ComponentFactory s_singleton = new ComponentFactory();

        private readonly IList<ComponentConstructor> m_constructors;

        private ComponentFactory()
        {
            m_constructors = new List<ComponentConstructor>();
            foreach (Type type in GetTypesWith<ComponentAttribute>(false))
            {
                m_constructors.Add(new ComponentConstructor(type));
            }
        }

        public static ComponentFactory Singleton
        {
            get
            {
                return s_singleton;
            }
        }

        public IList<ComponentConstructor> Constructors
        {
            get
            {
                return m_constructors;
            }
        }

        public Component Create(string typeName, string category, string name, FabricStyleList fabricStyles, ComponentParameterCollection parameters)
        {
            return Lookup(typeName).Create(category, name, fabricStyles, parameters);
        }

        public Component Create(JToken json)
        {
            string typeName = (string)json[JsonNames.TypeName];

            var constructor = Lookup(typeName);

            return constructor.Create(json);
        }

        private static IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit)
            where TAttribute : Attribute
        {
            var selectedTypes = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var assemblyTypes = assembly.GetTypes();
                    foreach (var assemblyType in assemblyTypes)
                    {
                        try
                        {
                            if (assemblyType.IsDefined(typeof(TAttribute), inherit))
                            {
                                selectedTypes.Add(assemblyType);
                            }
                        }
#pragma warning disable CA1031 // Do not catch general exception types
                        catch { }
#pragma warning restore CA1031 // Do not catch general exception types
                    }
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch { }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            return selectedTypes;
        }

        private ComponentConstructor Lookup(string typeName)
        {
            foreach (var factory in m_constructors)
            {
                if (factory.TypeName == typeName)
                {
                    return factory;
                }
            }

            return null;
        }
    }
}