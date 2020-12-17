//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq.Expressions;
using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace RichTodd.QuiltSystem.Resources.Web
{
    public class ModelMetadata<TModel>
    {
        private readonly IModelExpressionProvider m_expressionHelper;
        private readonly ViewDataDictionary<TModel> m_viewDataDictionary;

        public ModelMetadata(IModelExpressionProvider expressionHelper, ViewDataDictionary<TModel> viewDataDictionary)
        {
            m_expressionHelper = expressionHelper;
            m_viewDataDictionary = viewDataDictionary;
        }

        public static ModelMetadata<TModel> Create(HttpContext httpContext)
        {
            var expressionHelper = httpContext.RequestServices.GetService<IModelExpressionProvider>();
            var modelMetadataProvider = httpContext.RequestServices.GetService<IModelMetadataProvider>();
            var modelViewDataDictionary = new ViewDataDictionary<TModel>(modelMetadataProvider, new ModelStateDictionary());

            return new ModelMetadata<TModel>(expressionHelper, modelViewDataDictionary);
        }

        public static ModelMetadata<TModel> Create(IServiceProvider serviceProvider)
        {
            var expressionHelper = serviceProvider.GetService<IModelExpressionProvider>();
            var modelMetadataProvider = serviceProvider.GetService<IModelMetadataProvider>();
            var modelViewDataDictionary = new ViewDataDictionary<TModel>(modelMetadataProvider, new ModelStateDictionary());

            return new ModelMetadata<TModel>(expressionHelper, modelViewDataDictionary);
        }

        public string GetDisplayName<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            var modelExpression = m_expressionHelper.CreateModelExpression(m_viewDataDictionary, expression);
            var metadata = modelExpression.Metadata;
            var displayName = metadata.DisplayName;
            if (displayName == null)
            {
                displayName = metadata.Name;
            }
            return ProcessCamelCase(displayName);
        }

        private string ProcessCamelCase(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var sb = new StringBuilder();

            var length = value.Length;
            var prefix = "";
            for (int idx = 0; idx < length; ++idx)
            {
                var c = value[idx];
                if (c == ' ')
                {
                    prefix = "";
                }
                else if (char.IsUpper(c))
                {
                    _ = sb.Append(prefix);
                    prefix = "";
                }
                else
                {
                    prefix = " ";
                }
                _ = sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
