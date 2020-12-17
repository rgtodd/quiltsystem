//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MDesign_Block
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public string Group { get; set; }
        public string BlockName { get; set; }
        public IList<MDesign_FabricStyle> FabricStyles { get; set; }
        public string[] Tags { get; set; }
        public MDesign_BlockPreview Preview { get; set; }
    }
}
