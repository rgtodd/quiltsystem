//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

//using System;
//using System.Diagnostics;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using RichTodd.QuiltSystem.Business.Libraries;

//namespace RichTodd.QuiltSystem.Test.UnitTest
//{
//    [TestClass]
//    public class LibraryDataTest
//    {
//        [TestInitialize]
//        public void TestInitialize()
//        {
//            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
//        }

//        [TestMethod]
//        public void TestDatabaseResourceLibrary()
//        {
//            var library = DatabaseResourceLibrary.Create("Standard");
//            foreach (var entry in library.GetEntries())
//            {
//                Trace.WriteLine(entry.JsonSave());
//            }

//            //IBlockLibrary blockLibrary = new FolderBlockLibrary(
//            //    System.IO.Path.Combine(
//            //        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
//            //        "QuiltAGoGo",
//            //        "Library",
//            //        "Blocks"));

//            //foreach (var entry in blockLibrary.GetEntriesAsync().Result)
//            //{
//            //    Trace.WriteLine(entry.Name);
//            //}
//        }
//    }
//}