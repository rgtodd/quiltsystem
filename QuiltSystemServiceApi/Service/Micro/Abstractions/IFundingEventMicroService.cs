//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions
{
    public interface IFundingEventMicroService
    {
        Task HandleFunderEventAsync(MFunding_FunderEvent eventData);

        Task HandleFundableEventAsync(MFunding_FundableEvent eventData);
    }
}
