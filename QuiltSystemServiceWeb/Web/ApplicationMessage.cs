//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Web
{
    public class ApplicationMessage
    {

        private readonly string m_category;
        private readonly string m_message;

        private ApplicationMessage(string category, string message)
        {
            if (string.IsNullOrEmpty(category)) throw new ArgumentNullException(nameof(category));
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

            m_category = category;
            m_message = message;
        }

        public string Category
        {
            get
            {
                return m_category;
            }
        }

        public string Message
        {
            get
            {
                return m_message;
            }
        }

        public static ApplicationMessage NewErrorMessage(string message)
        {
            return new ApplicationMessage("Error", message);
        }

        public static ApplicationMessage NewInformationMessage(string message)
        {
            return new ApplicationMessage("Information", message);
        }

    }
}