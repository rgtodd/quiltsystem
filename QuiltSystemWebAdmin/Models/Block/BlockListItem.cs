//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Block
{
    public class BlockListItem
    {
        public MDesign_Block MBlock { get; }

        public BlockListItem(
            MDesign_Block mBlock)
        {
            MBlock = mBlock;
        }

        [Display(Name = "Block ID")]
        public string BlockId => MBlock.Id;

        [Display(Name = "Block Name")]
        public string BlockName => MBlock.BlockName;

        [Display(Name = "Tags")]
        public string[] Tags => MBlock.Tags;
    }
}
