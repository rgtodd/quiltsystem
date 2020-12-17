//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Design.Component
{
    public interface IComponentProvider
    {
        List<ComponentProviderEntry> GetComponents(string type, string category);
        ComponentProviderEntry GetComponent(string type, string category, string name);
    }
}
