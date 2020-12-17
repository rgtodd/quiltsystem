//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Net;
//using System.Threading.Tasks;

//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using RichTodd.QuiltSystem.Database;
//using RichTodd.QuiltSystem.Service.Admin.Data;
//using RichTodd.QuiltSystem.Service.Core;

//namespace RichTodd.QuiltSystem.Test.UnitTest
//{
//    [TestClass]
//    public class InventoryTest
//    {
//        private IServiceEnvironment m_environment;
//        private ServicePool m_servicePool;

//        [TestInitialize]
//        public void Initialize()
//        {
//            m_environment = DirectServiceEnvironment.Create(DateTimeAccessors.UtcNow, DateTimeAccessors.LocalNow, TimeZoneInfoAccessors.CentralStandardTime, BuiltInUsers.ServiceUser);
//            m_servicePool = new ServicePool(m_environment);
//        }

//        public void DownloadInventoryItems()
//        {
//            var webClient = new WebClient();

//            var lines = File.ReadAllLines(@"C:\Users\Rich\Documents\QuiltAGoGo\Fabrics\Import.csv");
//            foreach (var line in lines)
//            {
//                string[] fields = line.Split(new char[] { ',' });

//                string sku = fields[0];
//                string name = fields[1];
//                string imageUrl = fields[2];

//                string localFileName = @"C:\Users\Rich\Documents\QuiltAGoGo\Fabrics\Import\" + sku + ".jpg";
//                webClient.DownloadFile(imageUrl, localFileName);

//                Console.WriteLine(sku);
//            }
//        }

//        [TestMethod]
//        public async Task ImportInventoryItems()
//        {
//            var unitOfMeasureIds = new List<int>();
//            unitOfMeasureIds.Add(UnitsOfMeasure.FatQuarter);
//            unitOfMeasureIds.Add(UnitsOfMeasure.HalfYardage);
//            unitOfMeasureIds.Add(UnitsOfMeasure.Yardage);
//            unitOfMeasureIds.Add(UnitsOfMeasure.TwoYards);
//            unitOfMeasureIds.Add(UnitsOfMeasure.ThreeYards);

//            var lines = File.ReadAllLines(@"C:\Users\Rich\Documents\QuiltAGoGo\Fabrics\Import.csv");
//            foreach (var line in lines)
//            {
//                string[] fields = line.Split(new char[] { ',' });

//                string sku = fields[0];
//                string name = fields[1];

//                string localFileName = @"C:\Users\Rich\Documents\QuiltAGoGo\Fabrics\Import\" + sku + ".jpg";

//                var image = (Bitmap)Image.FromFile(localFileName);

//                var averageColor = getAverageColor(image);

//                var request = new Admin_Inventory_AddItemRequestData()
//                {
//                    InventoryItemTypeCode = InventoryItemTypes.Fabric,
//                    Quantity = 1000,
//                    ReservedQuantity = 0,
//                    Name = name,
//                    Sku = sku,
//                    Hue = (int)averageColor.GetHue(),
//                    Saturation = (int)(averageColor.GetSaturation() * 100),
//                    Value = (int)(averageColor.GetBrightness() * 100),
//                    UnitOfMeasureIdList = unitOfMeasureIds,
//                    PricingScheduleId = 1 // Default
//                };

//                var response = await m_servicePool.Admin_InventoryService.AddItem(request);

//                Console.WriteLine("{0} = {1}", sku, response.InventoryItemId);
//            }
//        }

//        [TestCleanup]
//        public void TestCleanup()
//        {
//            m_servicePool.Dispose();
//            m_environment.Dispose();
//        }

//        public Color getAverageColor(Bitmap bmp)
//        {

//            //Used for tally
//            int r = 0;
//            int g = 0;
//            int b = 0;

//            int total = 0;

//            for (int x = 0; x < bmp.Width; x++)
//            {
//                for (int y = 0; y < bmp.Height; y++)
//                {
//                    Color clr = bmp.GetPixel(x, y);

//                    r += clr.R;
//                    g += clr.G;
//                    b += clr.B;

//                    total++;
//                }
//            }

//            //Calculate average
//            r /= total;
//            g /= total;
//            b /= total;

//            return Color.FromArgb(r, g, b);
//        }
//    }
//}