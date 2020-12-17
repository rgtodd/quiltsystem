//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace RichTodd.QuiltSystem.Web.Bootstrap
{
    public class BootstrapForm : IDisposable
    {
        private const bool DefaultAlignLabelRight = false;
        private const bool DefaultAlignRight = false;
        private const bool DefaultCompact = true;
        private const int DefaultLabelWidth = 250;
        private const int DefaultDisplayFieldWidth = 500;
        private const int DefaultInputFieldWidth = 500;
        private const int DefaultDescriptionWidth = 100;

        private readonly ViewContext m_viewContext;

        private bool m_disposed = false;

        public BootstrapForm(ViewContext viewContext)
        {
            m_viewContext = viewContext;

            LabelWidth = DefaultLabelWidth;
            FieldWidth = DefaultDisplayFieldWidth;
            DescriptionWidth = DefaultDescriptionWidth;
            AlignRight = DefaultAlignRight;
            AlignLabelRight = DefaultAlignLabelRight;
            Compact = DefaultCompact;

            WriteOpeningHtml();
        }

        public bool AlignRight { get; set; }

        public bool AlignLabelRight { get; set; }

        public bool Compact { get; set; }

        public bool ContainsFields { get; set; }

        public int DescriptionWidth { get; set; }

        public int FieldWidth { get; set; }

        public int LabelWidth { get; set; }

        public bool SuppressNullValues { get; set; }

        public void Dispose()
        {
            Dispose(true);
        }

        public BootstrapForm ForDisplay()
        {
            return SetFieldWidth(DefaultDisplayFieldWidth);
        }

        public BootstrapForm ForInput()
        {
            return SetFieldWidth(DefaultInputFieldWidth).SetCompact(false);
        }

        public BootstrapForm SetAlignRight(bool alignRight)
        {
            AlignRight = alignRight;
            return this;
        }

        public BootstrapForm SetAlignLabelRight(bool alignLabelRight)
        {
            AlignLabelRight = alignLabelRight;
            return this;
        }

        public BootstrapForm SetCompact(bool compact)
        {
            Compact = compact;
            return this;
        }

        public BootstrapForm SetDescriptionWidth(int descriptionWidth)
        {
            DescriptionWidth = descriptionWidth;
            return this;
        }

        public BootstrapForm SetFieldWidth(int fieldWidth)
        {
            FieldWidth = fieldWidth;
            return this;
        }

        public BootstrapForm SetLabelWidth(int labelWidth)
        {
            LabelWidth = labelWidth;
            return this;
        }

        public BootstrapForm SetSuppressNullValues(bool suppressNullValues)
        {
            SuppressNullValues = suppressNullValues;
            return this;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    WriteClosingHtml();

                    m_viewContext.ViewData[BootstrapHtmlHelperExtensions.BootstrapFormViewKey] = null;
                }

                m_disposed = true;
            }
        }

        private void WriteClosingHtml()
        {
            var form = new TagBuilder("div")
            {
                TagRenderMode = TagRenderMode.EndTag
            };

            form.WriteTo(m_viewContext.Writer, HtmlEncoder.Default);
        }

        private void WriteOpeningHtml()
        {
            var form = new TagBuilder("div")
            {
                TagRenderMode = TagRenderMode.StartTag
            };

            form.AddCssClass("mt-2");

            form.WriteTo(m_viewContext.Writer, HtmlEncoder.Default);
        }
    }
}