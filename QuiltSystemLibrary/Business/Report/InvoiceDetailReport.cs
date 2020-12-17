//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable IDE0052 // Unnecessary assignment of a value
using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;

using RichTodd.QuiltSystem.Service.Database.Abstractions;

namespace RichTodd.QuiltSystem.Business.Report
{
    public class InvoiceDetailReport
    {
        private readonly IQuiltContextFactory m_quiltContextFactory;

        public InvoiceDetailReport(
            IQuiltContextFactory quiltContextFactory)
        {
            m_quiltContextFactory = quiltContextFactory ?? throw new ArgumentNullException(nameof(quiltContextFactory));
        }

        public async Task<byte[]> Print(string userId)
        {
            const double fontSize = 16;
            var font = new XFont("carrois gothic regular", fontSize, XFontStyle.Regular);

            var document = new PdfDocument();
            document.Info.Title = "Font Resolver Sample";

            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var tf = new XTextFormatter(gfx);

            //int y = 0;
            using (var ctx = m_quiltContextFactory.Create())
            {
                var dbOrders = await ctx.Orders.ToListAsync().ConfigureAwait(false);
                foreach (var dbOrder in dbOrders)
                {
                    //var order = factory.Create_Order_CartData(dbOrder, designLibrary);
                    //foreach (var item in order.Items)
                    //{
                    //    //XImage image;
                    //    //using (var ms = new MemoryStream(item.Image.Image))
                    //    //{
                    //    //    image = XImage.FromStream(ms);
                    //    //}

                    //    //gfx.DrawImage(image, 72, y);
                    //    y += 72 * 2;
                    //}
                }
            }

            using var stream = new MemoryStream();

            document.Save(stream);
            return stream.ToArray();
        }

    }
#pragma warning restore IDE0059 // Unnecessary assignment of a value
}