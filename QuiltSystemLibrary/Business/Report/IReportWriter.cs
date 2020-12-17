//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Business.Report
{
    public interface IReportWriter
    {

        void DefineColumn(string headingText);

        void WriteCell(string value, int colSpan);

        void WriteCellTotal(string value, int colSpan);

        void WriteEmptyCell(int colSpan);

    }
}