//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Web.Feedback
{
    public class FeedbackPool
    {
        private static readonly FeedbackPool s_singleton = new FeedbackPool();

        private readonly List<FeedbackMessage> m_emptyMessageList = new List<FeedbackMessage>();
        private readonly ConcurrentDictionary<Guid, FeedbackPoolEntry> m_entries = new ConcurrentDictionary<Guid, FeedbackPoolEntry>();
        private readonly DateTime m_startDateTime = DateTime.UtcNow;

        private FeedbackPool()
        { }

        public static FeedbackPool Singleton
        {
            get { return s_singleton; }
        }

        public void AddFeedbackMessage(Guid id, FeedbackMessageTypes messageType, string message)
        {
            var entry = GetFeedbackPoolEntry(id);

            entry.Messages.Add(
                new FeedbackMessage()
                {
                    MessageType = messageType,
                    Message = message
                });
        }

        public IReadOnlyList<FeedbackMessage> GetFeedbackMessages(Guid id)
        {
            return m_entries.TryRemove(id, out var entry)
                ? entry.Messages
                : m_emptyMessageList;
        }

        private FeedbackPoolEntry CreateFeedbackPoolEntry(Guid id)
        {
            var entry = new FeedbackPoolEntry(id, GetCurrentTimestamp());

            return entry;
        }

        private double GetCurrentTimestamp()
        {
            var now = DateTime.UtcNow;
            var elapsedTime = now - m_startDateTime;
            return elapsedTime.TotalMilliseconds;
        }

        private FeedbackPoolEntry GetFeedbackPoolEntry(Guid id)
        {
            var entry = m_entries.GetOrAdd(id, CreateFeedbackPoolEntry);

            return entry;
        }
    }
}