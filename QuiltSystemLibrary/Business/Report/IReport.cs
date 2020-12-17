//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.Database.Abstractions;

namespace RichTodd.QuiltSystem.Business.Report
{
    public interface IReport
    {

        void Run(IReportWriter wtr, IQuiltContextFactory quiltContextFactory);

    }
}