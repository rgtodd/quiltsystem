//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Database.Readers
{
    public partial class ReportTypeTableSummary
    {
        public string FieldName { get; set; }
        public string Name { get; set; }
        public int? RecordCount { get; set; }
        public string TableName { get; set; }
    }
}