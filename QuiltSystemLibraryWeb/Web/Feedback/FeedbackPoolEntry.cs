//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Web.Feedback
{
    public class FeedbackPoolEntry
    {
        private readonly double m_createTimestamp;
        private readonly Guid m_id;
        private readonly List<FeedbackMessage> m_messages = new List<FeedbackMessage>();

        public FeedbackPoolEntry(Guid id, double createTimestamp)
        {
            m_id = id;
            m_createTimestamp = createTimestamp;
        }

        public double CreateTimestamp
        {
            get
            {
                return m_createTimestamp;
            }
        }

        public Guid Id
        {
            get
            {
                return m_id;
            }
        }

        public List<FeedbackMessage> Messages
        {
            get
            {
                return m_messages;
            }
        }
    }
}