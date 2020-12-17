//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Test
{
    internal class StressTest
    {

        //private readonly IServiceEnvironment m_environment;
        //private readonly Random m_random = new Random();

        //public StressTest(IServiceEnvironment environment)
        //{
        //    m_environment = environment ?? throw new ArgumentNullException(nameof(environment));
        //}

        //public async Task AcceptOrderAsync(int count)
        //{
        //    // HACK: Migrate
        //    await Task.CompletedTask.ConfigureAwait(false);
        //    //using (var servicePool = new ServicePool(m_environment))
        //    //{
        //    //    var orders = servicePool.Admin_OrderService.GetOrderSummaries(servicePool.Admin_OrderService.ViewMode_Incomplete, null).Where(o => o.OrderStatusType == "Pending").ToList();

        //    //    for (int idx = 0; idx < count && orders.Count > 0; ++idx)
        //    //    {
        //    //        var order = orders[m_random.Next(orders.Count)];
        //    //        orders.Remove(order);

        //    //        await servicePool.Admin_OrderService.EnsureOrderAcceptedAsync(order.OrderId).ConfigureAwait(false);
        //    //    }
        //    //}
        //}

        //public async Task CreateOrderAsync(int count)
        //{
        //    // HACK: Migrate
        //    await Task.CompletedTask.ConfigureAwait(false);
        //    //using (var servicePool = new ServicePool(m_environment))
        //    //{
        //    //    for (int idx = 0; idx < count; ++idx)
        //    //    {
        //    //        var userId = await GetRandomUser(servicePool).ConfigureAwait(false);

        //    //        // HACK: Migrate
        //    //        //await servicePool.CartService.AddProjectAsync(userId, Guid.Parse("4AF84FDB-F77B-498A-943D-2797B589DE7F"), m_random.Next(9) + 1).ConfigureAwait(false);
        //    //        //await servicePool.CartService.AddProjectAsync(userId, Guid.Parse("DB73A4A5-E520-4050-B6BD-4CD9F37FF72A"), m_random.Next(9) + 1).ConfigureAwait(false);
        //    //    }
        //    //}
        //}

        //public async Task<string> GetRandomUser(ServicePool servicePool)
        //{
        //    var users = await servicePool.Admin_UserService.GetUsersAsync(null).ConfigureAwait(false);
        //    var user = users[m_random.Next(users.Count)];
        //    return user.UserId;
        //}

        //public async Task PayOrderAsync(int count)
        //{
        //    // HACK: Migrate
        //    await Task.CompletedTask.ConfigureAwait(false);
        //using (var servicePool = new ServicePool(m_environment))
        //{
        //    var orders = new List<Admin_Order_SummaryData>(
        //        servicePool.Admin_OrderService.GetOrderSummaries(servicePool.Admin_OrderService.ViewMode_Incomplete, null));

        //    for (int idx = 0; idx < count && orders.Count > 0; ++idx)
        //    {
        //        var order = orders[m_random.Next(orders.Count)];
        //        orders.Remove(order);

        //        var orderDetail = servicePool.Admin_OrderService.GetOrderDetail(order.OrderId);
        //        var incomeReceivable = orderDetail.LedgerAccounts.Where(r => r.LedgerAccountTypeCode == LedgerAccountTypes.AccountReceivable).SingleOrDefault();
        //        var salesTaxPayable = orderDetail.LedgerAccounts.Where(r => r.LedgerAccountTypeCode == LedgerAccountTypes.SalesTaxPayable).SingleOrDefault();

        //        if (incomeReceivable != null && incomeReceivable.Balance > 0)
        //        {
        //            var fee = decimal.Round(incomeReceivable.Balance * 0.03m, 2, MidpointRounding.AwayFromZero);
        //        }
        //    }
        //}
        //}

    }
}