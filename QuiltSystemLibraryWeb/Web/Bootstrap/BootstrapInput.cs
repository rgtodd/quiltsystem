//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.IO;

namespace RichTodd.QuiltSystem.Web.Bootstrap
{
    public class BootstrapInput : IDisposable
    {
        private bool m_disposed = false;
#pragma warning disable IDE0069 // Disposable fields should be disposed
#pragma warning disable CA2213 // Disposable fields should be disposed
        private readonly TextWriter m_writer;
#pragma warning restore CA2213 // Disposable fields should be disposed
#pragma warning restore IDE0069 // Disposable fields should be disposed

        public BootstrapInput(TextWriter writer)
        {
            m_writer = writer;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    WriteClosingHtml();
                }

                m_disposed = true;
            }
        }

        private void WriteClosingHtml()
        {
            m_writer.Write("</div></div>");
        }
    }
}