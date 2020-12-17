//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.Base.Abstractions.Data;

namespace RichTodd.QuiltSystem.Web.Mvc.Models
{
    public static class ErrorVcModelFactory
    {
        public static ErrorVcModel CreateErrorVcModel(ServiceErrorData serviceError)
        {
            if (serviceError != null)
            {
                var error = new ErrorVcModel();

                var pageErrors = new List<ErrorPageVcModel>();
                foreach (var svcPageError in serviceError.PageErrors)
                {
                    var pageError = new ErrorPageVcModel()
                    {
                        message = svcPageError.Message
                    };
                    pageErrors.Add(pageError);
                }
                error.pageErrors = pageErrors.ToArray();

                var fieldErrors = new List<ErrorFieldVcModel>();
                foreach (var svcFieldError in serviceError.FieldErrors)
                {
                    var fieldError = new ErrorFieldVcModel()
                    {
                        fieldName = svcFieldError.FieldName,
                        message = svcFieldError.Message
                    };
                    fieldErrors.Add(fieldError);
                }
                error.fieldErrors = fieldErrors.ToArray();

                return error;
            }
            else
            {
                return null;
            }
        }
    }
}