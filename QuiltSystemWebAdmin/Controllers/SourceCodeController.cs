//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.WebAdmin.Models.SourceCode;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class SourceCodeController : ApplicationController<SourceCodeModelFactory>
    {
        public SourceCodeController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            //m_cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        }

        public ActionResult Index()
        {
            var model = GetSourceCodeModel(this.GetPagingState(0).Filter);

            return View("List", model);
        }

        #region Methods

        private static string GetTypeName(Type type, Type sentinelType)
        {
            var codeDomProvider = CodeDomProvider.CreateProvider("C#");
            var typeReferenceExpression = new CodeTypeReferenceExpression(new CodeTypeReference(type, CodeTypeReferenceOptions.GenericTypeParameter));

            using var writer = new StringWriter();

            codeDomProvider.GenerateCodeFromExpression(typeReferenceExpression, writer, new CodeGeneratorOptions());
            var name = writer.GetStringBuilder().ToString();

            name = name.Replace(type.Namespace + ".", "");
            name = name.Replace(sentinelType.Namespace + ".", "");
            name = name.Replace("RichTodd.QuiltSystem.AdminService.Data.", "");

            return name;
        }

        private IList<SourceCodePropertyModel> GetPropertyModels(Type sentinelType, Type type)
        {
            var properties = new List<SourceCodePropertyModel>();

            foreach (var propertyInfo in type.GetProperties().OrderBy(r => r.Name))
            {
                var display = (DisplayAttribute)propertyInfo.GetCustomAttribute(typeof(DisplayAttribute));
                var displayFormat = (DisplayFormatAttribute)propertyInfo.GetCustomAttribute(typeof(DisplayFormatAttribute));

                var property = new SourceCodePropertyModel
                {
                    PropertyName = propertyInfo.Name,
                    DataType = GetTypeName(propertyInfo.PropertyType, sentinelType)
                };

                if (display != null)
                {
                    property.DisplayName = display.Name;
                }

                if (displayFormat != null)
                {
                    property.DisplayFormatDataFormatString = displayFormat.DataFormatString;
                }

                properties.Add(property);
            }

            return properties;
        }

        private SourceCodeModel GetSourceCodeModel(string filter)
        {
            var model = new SourceCodeModel();

            var types = new List<SourceCodeTypeModel>();
            types.AddRange(GetTypeModels(typeof(Service.Admin.Abstractions.Data.ReflectionSentinel)));
            types.AddRange(GetTypeModels(typeof(Models.ReflectionSentinel)));
            model.Types = types;

            model.Filter = filter;

            return model;
        }

        private IList<SourceCodeTypeModel> GetTypeModels(Type sentinelType)
        {
            var types = new List<SourceCodeTypeModel>();

            var assembly = Assembly.GetAssembly(sentinelType);

            foreach (var typeInfo in assembly.GetTypes().Where(r => r.Namespace == sentinelType.Namespace).OrderBy(r => r.Name))
            {
                var type = new SourceCodeTypeModel
                {
                    TypeName = typeInfo.Name,
                    Properties = GetPropertyModels(sentinelType, typeInfo)
                };
                types.Add(type);
            }

            return types;
        }

        #endregion Methods
    }
}