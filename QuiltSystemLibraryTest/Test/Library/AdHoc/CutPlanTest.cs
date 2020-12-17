//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

//using System.Collections.Generic;
//using System.Diagnostics;

//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using RichTodd.QuiltSystem.Build;
//using RichTodd.QuiltSystem.Design.Primitives;

//namespace QuiltAGoGoLibraryTest
//{
//    [TestClass]
//    public class CutPlanTest
//    {
//        

//        [TestMethod]
//        [Ignore]
//        public void TestCutPlan1()
//        {
//            var cutShapes = new List<ICutShape>();
//            for (int idx = 0; idx < 10; ++idx)
//            {
//                cutShapes.Add(new CutShapeMockup(new Area(new Dimension(3, DimensionUnits.Inch), new Dimension(3, DimensionUnits.Inch))));
//            }

//            var plan = CutPlanner.Plan(cutShapes);

//            foreach (var cutRegion in plan.CutRegions)
//            {
//                Trace.WriteLine(string.Format("Cut Region @ ({0},{1}) - {2} X {3}", cutRegion.Left, cutRegion.Top, cutRegion.Width, cutRegion.Height));
//            }
//        }

//        [TestInitialize]
//        public void TestInitialize()
//        {
//            //Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
//        }

//        

//        #region Private Classes

//        private class CutShapeMockup : ICutShape
//        {
//            

//            private readonly Area m_area;

//            

//            

//            public CutShapeMockup(Area area)
//            {
//                m_area = area;
//            }

//            

//            

//            public Area Area
//            {
//                get
//                {
//                    return m_area;
//                }
//            }

//            
//        }

//        #endregion Private Classes
//    }
//}