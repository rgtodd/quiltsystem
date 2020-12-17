//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Azure.Storage.Queues;

using Microsoft.Extensions.Configuration;

namespace RichTodd.QuiltSystem.Business.Job
{
    public class JobRequest
    {
        private readonly IConfiguration m_configuration;
        private readonly string m_queue;
        private readonly string m_message;

        public JobRequest(IConfiguration configuration, string queue, string message)
        {
            if (string.IsNullOrEmpty(queue)) throw new ArgumentNullException(nameof(queue));
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

            m_configuration = m_configuration ?? throw new ArgumentNullException(nameof(configuration));
            m_queue = queue;
            m_message = message;
        }

        public async Task EnqueueAsync()
        {
            var queue = GetQueue();
            await queue.SendMessageAsync(m_message).ConfigureAwait(false);
            //var queueMessage = new CloudQueueMessage(m_message);
            //await queue.AddMessageAsync(queueMessage).ConfigureAwait(false);
        }

        private QueueClient GetQueue()
        {
            var connectionString = m_configuration.GetConnectionString(ConnectionStringNames.Storage);
            var queue = new QueueClient(connectionString, m_queue);

            //var storageAccount = CloudStorageAccount.Parse(m_configuration.GetConnectionString(ConnectionStringNames.Storage));
            //var queueClient = storageAccount.CreateCloudQueueClient();
            //queueClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);
            //var queue = queueClient.GetQueueReference(m_queue);

            return queue;
        }
    }
}
