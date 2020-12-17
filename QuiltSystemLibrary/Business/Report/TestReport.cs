//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.IO;

using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;

namespace RichTodd.QuiltSystem.Business.Report
{
    public class TestReport
    {

        public byte[] CreateReport()
        {
            //const string text =
            //    "Facin exeraessisit la consenim iureet dignibh eu facilluptat vercil dunt autpat. " +
            //    "Ecte magna faccum dolor sequisc iliquat, quat, quipiss equipit accummy niate magna " +
            //    "facil iure eraesequis am velit, quat atis dolore dolent luptat nulla adio odipissectet " +
            //    "lan venis do essequatio conulla facillandrem zzriusci bla ad minim inis nim velit eugait " +
            //    "aut aut lor at ilit ut nulla ate te eugait alit augiamet ad magnim iurem il eu feuissi.\n" +
            //    "Guer sequis duis eu feugait luptat lum adiamet, si tate dolore mod eu facidunt adignisl in " +
            //    "henim dolorem nulla faccum vel inis dolutpatum iusto od min ex euis adio exer sed del " +
            //    "dolor ing enit veniamcon vullutat praestrud molenis ciduisim doloborem ipit nulla consequisi.\n" +
            //    "Nos adit pratetu eriurem delestie del ut lumsandreet nis exerilisit wis nos alit venit praestrud " +
            //    "dolor sum volore facidui blaor erillaortis ad ea augue corem dunt nis  iustinciduis euisi.\n" +
            //    "Ut ulputate volore min ut nulpute dolobor sequism olorperilit autatie modit wisl illuptat dolore " +
            //    "min ut in ute doloboreet ip ex et am dunt at.";

            const double fontSize = 16;
            XFont font = new XFont("carrois gothic regular", fontSize, XFontStyle.Regular);

            var document = new PdfDocument();
            document.Info.Title = "Font Resolver Sample";

            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var tf = new XTextFormatter(gfx);

            for (int x = 0; x < 1000; x += 72)
            {
                for (int y = 0; y < 1000; y += 72)
                {
                    var rect = new XRect(x, y, 72, 72);
                    tf.DrawString("One Two Three", font, XBrushes.Black, rect, XStringFormats.TopLeft);
                }
            }

            using var stream = new MemoryStream();

            document.Save(stream);
            return stream.ToArray();
        }

    }
}