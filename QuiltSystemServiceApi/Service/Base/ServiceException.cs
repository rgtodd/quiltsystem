//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Base
{
    public class ServiceException : Exception
    {
        private readonly IList<string> m_details;

        public ServiceException()
        {
            m_details = new List<string>();
        }

        public ServiceException(string message)
            : base(message)
        {
            m_details = new List<string>();
        }

        public ServiceException(string message, IEnumerable<string> details)
            : base(message)
        {
            m_details = details != null
                ? new List<string>(details)
                : new List<string>();
        }

        //public ServiceException(string message, IEnumerable<IdentityError> details)
        //    : base(message)
        //{
        //    if (details != null)
        //    {
        //        m_details = new List<string>();
        //        foreach (var detail in details)
        //        {
        //            m_details.Add(detail.Description);
        //        }
        //    }
        //    else
        //    {
        //        m_details = new List<string>();
        //    }
        //}

        public ServiceException(string message, Exception inner)
            : base(message, inner)
        {
            m_details = new List<string>();
        }

        public ServiceException(string message, Exception inner, IEnumerable<string> details)
            : base(message, inner)
        {
            m_details = details != null
                ? new List<string>(details)
                : new List<string>();
        }

        public IEnumerable<string> Details
        {
            get
            {
                return m_details;
            }
        }

    }
}