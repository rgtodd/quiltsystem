//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//

//namespace RichTodd.QuiltSystem.Business.NodeFactories
//{
//    class FolderNodeFactory : INodeFactory
//    {
//        private string m_folderName;

//        public FolderNodeFactory(string folderName)
//        {
//            if (folderName == null) throw new ArgumentNullException("folderName");

//            m_folderName = folderName;
//        }

//        public Node Create(string name, FabricStyleList fabricStyles, ComponentParameterCollection parameters, NodeList childNodes)
//        {
//            if (name == null) throw new ArgumentNullException(nameof(name));
//            if (fabricStyles == null) throw new ArgumentNullException(nameof(fabricStyles));
//            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

//            var palette = new Palette("temp");
//            foreach (var fabricStyle in fabricStyles)
//            {
//                palette.Entries.Add(new PaletteEntry(fabricStyle));
//            }

//            var theme = new Theme("temp");
//            theme.Entries.Add(new ThemeEntry(palette));

//            var fileName = name + ".txt";
//            var path = System.IO.Path.Combine(m_folderName, fileName);
//            if (!File.Exists(path))
//            {
//                return null;
//            }

//            var fileLines = File.ReadAllLines(path);

//            var pattern = TextPatternParser.Parse(fileLines);
//            var node = pattern;

//            node.Style = "0";
//            theme.Apply(node);

//            return node;
//        }
//    }
//}
