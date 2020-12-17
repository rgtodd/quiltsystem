//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Ajax.Abstractions
{
    public interface IColorAjaxService
    {

        Task<XColor_ColorPalette> CreateColorPaletteAsync(string webColor);

    }
}