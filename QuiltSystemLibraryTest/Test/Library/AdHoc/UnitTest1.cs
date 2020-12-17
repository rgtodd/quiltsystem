//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

//using Microsoft.VisualStudio.TestTools.UnitTesting;

//
//using RichTodd.QuiltSystem.Design.Primitives;

//namespace RichTodd.QuiltSystem.Test.UnitTest
//{
//    [TestClass]
//    public class UnitTest1
//    {
//        [TestInitialize]
//        public void TestInitialize()
//        {
//            //Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
//        }

//        [TestMethod]
//        [Ignore]
//        public void TestMethod1()
//        {
//            FabricStyle fabricStyleBlue = new FabricStyle(FabricStyle.UNKNOWN_SKU, System.Drawing.Color.Blue);
//            FabricStyle fabricStyleGreen = new FabricStyle(FabricStyle.UNKNOWN_SKU, System.Drawing.Color.Green);

//            GridLayoutNode gridLayout = new GridLayoutNode(2, 2);
//            gridLayout.LayoutSites[0].Node = new RectangleShapeNode(fabricStyleBlue);
//            gridLayout.LayoutSites[1].Node = new RectangleShapeNode(fabricStyleGreen);
//            gridLayout.LayoutSites[2].Node = new RectangleShapeNode(fabricStyleGreen);
//            gridLayout.LayoutSites[3].Node = new RectangleShapeNode(fabricStyleBlue);

//            DimensionScale scale = new DimensionScale(1, DimensionUnits.Inch, 1, DimensionUnits.Inch);

//            PageLayoutNode pageLayout = new PageLayoutNode(new Dimension(1000, DimensionUnits.Pixel), new Dimension(1000, DimensionUnits.Pixel));
//            pageLayout.LayoutSites[0].Node = gridLayout;
//            pageLayout.UpdateBounds(PathOrientation.CreateDefault(), scale);
//        }

//        [TestMethod]
//        [Ignore]
//        public void TestMethod2()
//        {
//            //IPath path = PathGeometries.Rectangle.CreatePath(new PathPoint[] {
//            //    new PathPoint() { X = 0, Y = 0 } ,
//            //    new PathPoint() {X = 100, Y = 0 },
//            //    new PathPoint() {X = 100, Y = 100 },
//            //    new PathPoint() {X = 0, Y = 1000 }});
//        }

//        [TestMethod]
//        [Ignore]
//        public void TestMethod3()
//        {
//            //var pattern1 = PatternFactory.CreateTestPattern2(1000, 1000);
//            //var json1 = pattern1.JsonSave();

//            //var pattern2 = pattern1.Clone();
//            //var json2 = pattern2.JsonSave();

//            //Assert.AreEqual(json1.ToString(), json2.ToString());
//        }
//    }
//}