//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RichTodd.QuiltSystem.Database
{
    public static class DbRetry
    {
        public static async Task<TResult> FunctionAsync<TResult>(Func<Task<TResult>> function)
        {
            TResult result;

            int timeout = 1000;
            for (int idx = 0; idx < 10; ++idx)
            {
                try
                {
                    result = await function.Invoke();
                    return result;
                }
                catch (DbRetryException)
                {
                    Thread.Sleep(timeout);
                }
            }

            result = await function.Invoke();
            return result;
        }

        public static async Task FunctionAsync(Func<Task> function)
        {
            int timeout = 1000;
            for (int idx = 0; idx < 10; ++idx)
            {
                try
                {
                    await function.Invoke();
                    return;
                }
                catch (DbRetryException)
                {
                    Thread.Sleep(timeout);
                }
            }

            await function.Invoke();
        }
    }
}
