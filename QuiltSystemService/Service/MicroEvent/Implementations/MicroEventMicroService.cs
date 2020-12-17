//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;

namespace RichTodd.QuiltSystem.Service.MicroEvent.Implementations
{
    internal class MicroEventMicroService
    {
        public MicroEventMicroService(
            IApplicationLocale locale,
            ILogger logger,
            IQuiltContextFactory quiltContextFactory,
            IServiceProvider serviceProvider)
        {
            Locale = locale ?? throw new ArgumentNullException(nameof(locale));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            QuiltContextFactory = quiltContextFactory ?? throw new ArgumentNullException(nameof(quiltContextFactory));
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected ILogger Logger { get; }

        protected IApplicationLocale Locale { get; }

        protected IQuiltContextFactory QuiltContextFactory { get; }

        protected IServiceProvider ServiceProvider { get; }

        protected ICommunicationMicroService CommunicationMicroService
        {
            get
            {
                return ServiceProvider.GetService<ICommunicationMicroService>();
            }
        }

        protected IFulfillmentMicroService FullfillmentMicroService
        {
            get
            {
                return ServiceProvider.GetService<IFulfillmentMicroService>();
            }
        }

        protected IFundingMicroService FundingMicroService
        {
            get
            {
                return ServiceProvider.GetService<IFundingMicroService>();
            }
        }

        protected IInventoryMicroService InventoryMicroService
        {
            get
            {
                return ServiceProvider.GetService<IInventoryMicroService>();
            }
        }

        protected IOrderMicroService OrderMicroService
        {
            get
            {
                return ServiceProvider.GetService<IOrderMicroService>();
            }
        }

        protected IProjectMicroService ProjectMicroService
        {
            get
            {
                return ServiceProvider.GetService<IProjectMicroService>();
            }
        }

        protected ISquareMicroService SquareMicroService
        {
            get
            {
                return ServiceProvider.GetService<ISquareMicroService>();
            }
        }

        protected IUserMicroService UserMicroService
        {
            get
            {
                return ServiceProvider.GetService<IUserMicroService>();
            }
        }

        protected IFunctionContext BeginFunction(string className, string functionName, params object[] args)
        {
            return Function.BeginFunction(Logger, className, functionName, args);
        }

        //protected void BeginFunction(string className, string functionName, params object[] args)
        //{
        //    Logger.LogBeginFunction(className, functionName, args);
        //}

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

        //protected void LogResult(object result)
        //{
        //    Logger.LogResult(result);
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
