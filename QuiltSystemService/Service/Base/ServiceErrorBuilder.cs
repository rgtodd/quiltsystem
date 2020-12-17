//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.Base.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Base
{
    internal class ServiceErrorBuilder
    {

        private ServiceErrorData m_serviceError;

        public ServiceErrorData ServiceError
        {
            get
            {
                return m_serviceError;
            }
        }

        public void AddFieldError(string fieldName, string message)
        {
            var fieldError = new ServiceFieldErrorData()
            {
                FieldName = fieldName,
                Message = message
            };

            GetFieldErrors().Add(fieldError);
        }

        public void AddPageError(string message)
        {
            var pageError = new ServicePageErrorData()
            {
                Message = message
            };

            GetPageErrors().Add(pageError);
        }

        private IList<ServiceFieldErrorData> GetFieldErrors()
        {
            var serviceError = GetServiceError();
            if (serviceError.FieldErrors == null)
            {
                serviceError.FieldErrors = new List<ServiceFieldErrorData>();
            }

            return serviceError.FieldErrors;
        }

        private IList<ServicePageErrorData> GetPageErrors()
        {
            var serviceError = GetServiceError();
            if (serviceError.PageErrors == null)
            {
                serviceError.PageErrors = new List<ServicePageErrorData>();
            }

            return serviceError.PageErrors;
        }

        private ServiceErrorData GetServiceError()
        {
            if (m_serviceError == null)
            {
                m_serviceError = new ServiceErrorData();
            }

            return m_serviceError;
        }

    }
}