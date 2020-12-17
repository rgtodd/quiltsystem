//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Design.Primitives
{
    // One value should be defined for 0.  This ensures that Dimension's initialized to 0 have a valid
    // DimensionUnit.
    //
    public enum DimensionUnits
    {
        Pixel = 0,
        Inch = 1
    }
}
