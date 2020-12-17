//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Security;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Core.Abstractions;

namespace RichTodd.QuiltSystem.Service.Base
{
    internal class BaseService
    {
        public BaseService(IApplicationRequestServices requestServices, ILogger logger)
        {
            RequestServices = requestServices ?? throw new ArgumentNullException(nameof(requestServices));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected IApplicationRequestServices RequestServices { get; }

        protected IApplicationLocale Locale
        {
            get { return RequestServices.Locale; }
        }

        protected ILogger Logger { get; }

        protected IApplicationSecurityPolicy SecurityPolicy
        {
            get { return RequestServices.SecurityPolicy; }
        }

        //protected async Task AssertAuthorized(string toUserId)
        //{
        //    await SecurityPolicy.AssertAuthorized(toUserId).ConfigureAwait(false);
        //}

        protected async Task Assert(Func<Task<bool>> assertion)
        {
            var result = await assertion().ConfigureAwait(false);
            if (!result)
            {
                throw new SecurityException($"Security assertion {assertion.Method.Name} failed.");
            }
        }

        protected async Task Assert<TParm>(Func<TParm, Task<bool>> assertion, TParm parmameter)
        {
            var result = await assertion(parmameter).ConfigureAwait(false);
            if (!result)
            {
                throw new SecurityException($"Security assertion {assertion.Method.Name} failed.");
            }
        }

        protected DateTime GetUtcNow()
        {
            return Locale.GetUtcNow();
        }

        protected DateTime GetLocalNow()
        {
            return Locale.GetLocalNow();
        }

        protected IFunctionContext BeginFunction(string className, string functionName, params object[] args)
        {
            return Function.BeginFunction(Logger, className, functionName, args);
        }
    }
}
