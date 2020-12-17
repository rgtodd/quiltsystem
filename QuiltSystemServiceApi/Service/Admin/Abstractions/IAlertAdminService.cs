//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions
{
    public interface IAlertAdminService
    {
        Task<AAlert_AlertList> GetAlertsAsync(bool? acknowledged, int recordCount);
        Task<AAlert_Alert> GetAlertAsync(long alertId);
        Task AcknowledgeAlertsAsync();
        Task AcknowledgeAlertAsync(long alertId);
    }
}
