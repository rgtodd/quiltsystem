//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Design.Core;
using RichTodd.QuiltSystem.Design.Nodes;
using RichTodd.QuiltSystem.Design.Nodes.Standard;
using RichTodd.QuiltSystem.Design.Primitives;

namespace RichTodd.QuiltSystem.Design.Build
{
    internal class BuildComponentFactory
    {
        private int m_flyingGooseId;
        private int m_halfSquareTriangleId;
        private int m_layoutId;
        private int m_quiltId;
        private int m_rectangleId;
        private int m_yardageId;

        public BuildComponentFlyingGoose CreateBuildComponentFlyingGooose(FabricStyle fabricStyleBody, FabricStyle fabricStyleCorner, Area area, bool trim)
        {
            m_flyingGooseId += 1;
            var id = "FG." + m_flyingGooseId;

            return new BuildComponentFlyingGoose(id, fabricStyleBody, fabricStyleCorner, area, trim);
        }

        public BuildComponentHalfSquareTriangle CreateBuildComponentHalfSquareTriangle(HalfSquareTriangleLayoutNode layoutNode, bool trim)
        {
            m_halfSquareTriangleId += 1;
            var id = "HSQ." + m_halfSquareTriangleId;

            return new BuildComponentHalfSquareTriangle(id, layoutNode, trim);
        }

        public BuildComponentLayout CreateBuildComponentLayout(LayoutNode layoutNode, bool trimTriangles)
        {
            m_layoutId += 1;
            var id = "B." + m_layoutId;

            return new BuildComponentLayout(id, layoutNode, trimTriangles);
        }

        public BuildComponentQuilt CreateBuildComponentQuilt(KitSpecification kitSpecification, Core.Design design)
        {
            m_quiltId += 1;
            var id = "Q." + m_quiltId;

            return new BuildComponentQuilt(id, kitSpecification, design);
        }

        public BuildComponentRectangle CreateBuildComponentRectangle(FabricStyle fabricStyle, Area area)
        {
            m_rectangleId += 1;
            var id = "R." + m_rectangleId;

            return new BuildComponentRectangle(id, fabricStyle, area);
        }

        public BuildComponentYardage CreateBuildComponentYardage(FabricStyle fabricStyle, AreaSizes areaSize)
        {
            m_yardageId += 1;
            var id = "Y." + m_yardageId;

            return new BuildComponentYardage(id, fabricStyle, areaSize);
        }
    }
}