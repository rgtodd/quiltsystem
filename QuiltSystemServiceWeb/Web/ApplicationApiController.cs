//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RichTodd.QuiltSystem.Web
{
    [ApplicationRequireHttps]
    public class ApplicationApiController : ControllerBase, IApplicationController
    {

        private readonly UserManager<IdentityUser> m_userManager;
        private readonly SignInManager<IdentityUser> m_signInManager;
        private readonly ILogger m_applicationLogger;

        public ApplicationApiController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger applicationLogger)
        {
            m_userManager = userManager;
            m_signInManager = signInManager;
            m_applicationLogger = applicationLogger;
        }

        protected UserManager<IdentityUser> UserManager
        {
            get { return m_userManager; }
        }

        protected SignInManager<IdentityUser> SignInManager
        {
            get { return m_signInManager; }
        }

        protected ILogger Logger
        {
            get { return m_applicationLogger; }
        }

        //public IServiceEnvironment Environment
        //{
        //    get
        //    {
        //        if (m_environment == null)
        //        {
        //            m_environment = OwinServiceEnvironment.Create(
        //                ApplicationDependencies.AppConfiguration,
        //                ApplicationDependencies.QuiltContextFactory,
        //                HttpContext,
        //                m_userManager,
        //                m_signInManager,
        //                m_applicationLogger);

        //            HttpContext.Response.RegisterForDispose(m_environment);
        //        }

        //        return m_environment;
        //    }
        //}

        //public ServicePool ServicePool
        //{
        //    get
        //    {
        //        if (m_servicePool == null)
        //        {
        //            m_servicePool = new ServicePool(Environment);

        //            HttpContext.Response.RegisterForDispose(m_servicePool);
        //        }

        //        return m_servicePool;
        //    }
        //}

    }
}