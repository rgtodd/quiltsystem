//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class MicroService
    {
        public MicroService(
            IApplicationLocale locale,
            ILogger logger,
            IQuiltContextFactory quiltContextFactory)
        {
            Locale = locale ?? throw new ArgumentNullException(nameof(locale));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            QuiltContextFactory = quiltContextFactory ?? throw new ArgumentNullException(nameof(quiltContextFactory));
        }

        protected ILogger Logger { get; }

        protected IApplicationLocale Locale { get; }

        protected IQuiltContextFactory QuiltContextFactory { get; }

        protected IFunctionContext BeginFunction(string className, string functionName, params object[] args)
        {
            return Function.BeginFunction(Logger, className, functionName, args);
        }

        //protected void EndFunction()
        //{
        //    Logger.LogEndFunction();
        //}

        //protected void LogException(Exception ex)
        //{
        //    Logger.log.LogException(ex);
        //}

        //protected void LogMessage(string message)
        //{
        //    Logger.LogMessage(message);
        //}

        //protected void log.LogResult(object result)
        //{
        //    Logger.log.LogResult(result);
        //}

        protected DateTime GetLocalNow()
        {
            return Locale.GetLocalNow();
        }

        protected DateTime GetUtcNow()
        {
            return Locale.GetUtcNow();
        }

        protected QuiltContext CreateQuiltContext()
        {
            return QuiltContextFactory.Create();
        }

    }
}
