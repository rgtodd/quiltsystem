//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RichTodd.QuiltSystem.Security;
using RichTodd.QuiltSystem.Service.Ajax.Abstractions;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions;

namespace RichTodd.QuiltSystem.Test
{
    public class BaseTest
    {
        private const int UserCount = 100;

        protected static readonly Random Random = new Random();
        protected static readonly Words Words = new Words();
        protected static DateTime LastDateTime = DateTime.Now;

        public TestContext TestContext { get; set; }

        protected ServiceProvider ServiceProvider { get; set; }
        protected IServiceScope ServiceScope { get; set; }

        public void OnTestInitialize(bool mockEvents)
        {
            var configuration = Setup.LoadConfiguration(TestContext);
            ServiceProvider = Setup.ConfigureServices(configuration, mockEvents);
            ServiceScope = ServiceProvider.CreateScope();
            var services = ServiceScope.ServiceProvider;
            Setup.ConfigureFactories(services);
        }

        public void OnTestCleanup()
        {
            ServiceScope.Dispose();
            ServiceProvider.Dispose();
        }

        protected DateTime GetUniqueNow()
        {
            var now = DateTime.Now;
            while (now == LastDateTime)
            {
                now = DateTime.Now;
            }
            LastDateTime = now;
            return now;
        }

        #region Class Services

        protected UserManager<IdentityUser> UserManager
        {
            get
            {
                return ServiceProvider.GetService<UserManager<IdentityUser>>();
            }
        }

        #endregion

        #region Test Services

        protected IApplicationLocale Locale
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<IApplicationLocale>();
            }
        }

        //protected IApplicationLogger Logger
        //{
        //    get
        //    {
        //        return ServiceScope.ServiceProvider.GetService<IApplicationLogger>();
        //    }
        //}

        protected IQuiltContextFactory QuiltContextFactory
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<IQuiltContextFactory>();
            }
        }

        protected ICacheMicroService CacheMicroService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<ICacheMicroService>();
            }
        }

        protected ICommunicationMicroService CommunicationMicroService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<ICommunicationMicroService>();
            }
        }

        protected IDesignMicroService DesignMicroService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<IDesignMicroService>();
            }
        }

        protected IDomainMicroService DomainMicroService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<IDomainMicroService>();
            }
        }

        protected IEventProcessorMicroService EventProcessorMicroService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<IEventProcessorMicroService>();
            }
        }

        protected IFulfillmentMicroService FulfillmentMicroService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<IFulfillmentMicroService>();
            }
        }

        protected IFundingMicroService FundingMicroService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<IFundingMicroService>();
            }
        }

        protected IInventoryMicroService InventoryMicroService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<IInventoryMicroService>();
            }
        }

        protected IKitMicroService KitMicroService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<IKitMicroService>();
            }
        }

        protected ILedgerMicroService LedgerMicroService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<ILedgerMicroService>();
            }
        }

        protected IOrderMicroService OrderMicroService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<IOrderMicroService>();
            }
        }

        protected IProjectMicroService ProjectMicroService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<IProjectMicroService>();
            }
        }

        protected ISquareMicroService SquareMicroService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<ISquareMicroService>();
            }
        }

        protected IUserMicroService UserMicroService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<IUserMicroService>();
            }
        }

        protected IColorAjaxService ColorAjaxService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<IColorAjaxService>();
            }
        }

        protected IDesignAjaxService DesignAjaxService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<IDesignAjaxService>();
            }
        }

        protected IProjectUserService ProjectUserService
        {
            get
            {
                return ServiceScope.ServiceProvider.GetService<IProjectUserService>();
            }
        }

        #endregion

        #region User Methods

        private IList<IdentityUser> m_users;
        protected async Task<IList<IdentityUser>> GetUsersAsync()
        {
            if (m_users == null)
            {
                var users = await UserManager.GetUsersInRoleAsync(ApplicationRoles.User);

                if (users.Count < UserCount)
                {
                    users = new List<IdentityUser>(users);
                    for (int idx = users.Count; idx < UserCount; ++idx)
                    {
                        users.Add(await CreateRandomUserAsync());
                    }
                }

                m_users = users;
            }

            return m_users;
        }

        protected async Task<IdentityUser> GetRandomUserAsync()
        {
            var users = await GetUsersAsync();
            var idx = Random.Next(users.Count);
            return users[idx];
        }

        protected async Task<IdentityUser> CreateRandomUserAsync()
        {
            string email = Words.GetRandomEmail();
            string password = "testtest";

            return await CreateUserAsync(email, email, password, ApplicationRoles.User);
        }

        protected async Task<IdentityUser> CreateUserAsync(string userName, string email, string password, string role)
        {
            var user = await UserManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user != null)
            {
                return user;
            }

            user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = userName,
                Email = email,
                EmailConfirmed = true
            };

            var result = await UserManager.CreateAsync(user, password).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new Exception("Could not create user " + userName);
            }

            m_users = null;

            user = await UserManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user == null)
            {
                throw new Exception("Could not find user " + userName);
            }

            result = await UserManager.AddToRoleAsync(user, role).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new Exception("Could not add role " + role + " to user " + userName);
            }

            return user;
        }

        #endregion
    }
}
