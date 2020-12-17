//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Design.Component
{
    public class ComponentProviderEntry
    {
        public string Type { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string[] Tags { get; set; }
        public Component Component { get; set; }
        public int BlockCount { get; set; }
    }
}
