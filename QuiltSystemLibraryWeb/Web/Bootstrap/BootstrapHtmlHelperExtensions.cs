//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using System.Web;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using RichTodd.QuiltSystem.Web.Extensions;

namespace RichTodd.QuiltSystem.Web.Bootstrap
{
    public static class BootstrapHtmlHelperExtensions
    {
        public static string BootstrapFormViewKey = "BootstrapFormViewKey";

        private const string ButtonCssClasses = "btn btn-sm btn-primary mt-1 mr-1";

        private static readonly HtmlString s_blank = new HtmlString(" ");

        #region Buttons

        public static IHtmlContent BootstrapActionLink(this IHtmlHelper htmlHelper, string linkText, string actionName, string controllerName)
        {
            return new HtmlContentBuilder()
                .AppendHtml(htmlHelper.ActionLink(HttpUtility.HtmlDecode(linkText), actionName, controllerName, null, null, null, null, new { @class = ButtonCssClasses }));
        }

        public static IHtmlContent BootstrapActionLink(this IHtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues)
        {
            return new HtmlContentBuilder()
                .AppendHtml(htmlHelper.ActionLink(linkText, actionName, controllerName, null, null, null, routeValues, new { @class = ButtonCssClasses }));
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for extension method.")]
        public static IHtmlContent BootstrapButton(this IHtmlHelper htmlHelper, string linkText, string id, bool isHeader = false, bool isDisabled = false, string dataUrl = null)
        {
            var button = new TagBuilder("button");
            if (!string.IsNullOrEmpty(id))
            {
                button.MergeAttribute("id", id);
            }
            button.AddCssClass(ButtonCssClasses);
            if (isHeader)
            {
                button.AddCssClass("header__button");
                if (isDisabled)
                {
                    button.AddCssClass("header__button--disabled");
                }
            }
            if (!string.IsNullOrEmpty(dataUrl))
            {
                button.MergeAttribute("data-url", dataUrl);
            }
            _ = button.InnerHtml.Append(linkText);

            return button;
        }

        public static IHtmlContent BootstrapListActionLink(this IHtmlHelper htmlHelper, string linkText, string actionName, bool isHeader = false, object id = null, int index = 0, string filter = null)
        {
            var classValue = ButtonCssClasses;
            if (isHeader)
            {
                classValue += " header__button";
            }

            var hc = new HtmlContentBuilder();
            _ = hc.AppendHtml(htmlHelper.ListActionLink(linkText, actionName, new { @class = classValue }, id, index, filter));
            _ = hc.AppendHtml(s_blank);

            return hc;
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for extension method.")]
        public static IHtmlContent BootstrapSubmitButton(this IHtmlHelper htmlHelper, string linkText, string action, string parameter = null, bool isCancel = false, string id = null, bool isHeader = false)
        {
            var actionValue = "action";
            if (!string.IsNullOrEmpty(action))
            {
                actionValue += "~" + action;
                if (!string.IsNullOrEmpty(parameter))
                {
                    actionValue += "~" + parameter;
                }
            }

            var input = new TagBuilder("input");
            input.MergeAttribute("type", "submit");
            input.MergeAttribute("name", actionValue);
            input.MergeAttribute("value", linkText);
            if (!string.IsNullOrEmpty(id))
            {
                input.MergeAttribute("id", id);
            }
            input.AddCssClass(ButtonCssClasses);
            if (isHeader)
            {
                input.AddCssClass("header__button");
            }
            if (isCancel)
            {
                input.AddCssClass("cancel");
            }

            var hc = new HtmlContentBuilder();
            _ = hc.AppendHtml(input);
            _ = hc.AppendHtml(s_blank);

            return hc;
        }

        #endregion

        #region Forms

        public static BootstrapForm BootstrapBeginForm(this IHtmlHelper htmlHelper)
        {
            if (htmlHelper.ViewContext.ViewData[BootstrapFormViewKey] != null)
            {
                throw new InvalidOperationException("BootstrapForm already exists in ViewData.");
            }

            var bootstrapForm = new BootstrapForm(htmlHelper.ViewContext);

            htmlHelper.ViewContext.ViewData[BootstrapFormViewKey] = bootstrapForm;

            return bootstrapForm;
        }

        public static IHtmlContent BootstrapDisplay<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string labelFormat = null, string linkText = null, string link = null, bool? compact = null, bool? alignRight = null)
        {
            var bootstrapForm = htmlHelper.GetBootstrapForm();

            var expressionValue = expression.Compile()(htmlHelper.ViewData.Model);
            if (bootstrapForm.SuppressNullValues && expressionValue == null)
            {
                return HtmlString.Empty;
            }

            bootstrapForm.ContainsFields = true;

            var field = new TagBuilder("div");
            {
                field.AddCssClass("row");
                field.AddCssClass("form-group");
                if (compact ?? bootstrapForm.Compact)
                {
                    field.AddCssClass("mb-0");
                }

                var htmlFormLabel = htmlHelper.BootstrapFormLabel(expression, labelFormat);
                _ = field.InnerHtml.AppendHtml(htmlFormLabel);

                // If a label was not generated for this field, we have to manually add offset classes to the input control in order to maintain alignment.
                //
                var offsetRequired = htmlFormLabel == HtmlString.Empty;
                var htmlField = htmlHelper.BootstrapFormDisplayControl(expression, linkText, link, alignRight ?? bootstrapForm.AlignRight, offsetRequired);
                _ = field.InnerHtml.AppendHtml(htmlField);

                var htmlDescription = htmlHelper.BootstrapFormDescription(linkText, link);
                _ = field.InnerHtml.AppendHtml(htmlDescription);
            }

            return field;
        }

        public static IHtmlContent BootstrapInput<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression,
            IEnumerable<SelectListItem> selectList = null,
            string fieldClass = null,
            string controlClass = null,
            string dataField = null,
            bool optionLabel = false,
            string labelFormat = null)
        {
            var bootstrapForm = htmlHelper.GetBootstrapForm();
            bootstrapForm.ContainsFields = true;

            var field = new TagBuilder("div");
            {
                field.AddCssClass("row");
                field.AddCssClass("form-group");
                if (bootstrapForm.Compact)
                {
                    field.AddCssClass("mb-0");
                }
                if (!string.IsNullOrEmpty(fieldClass))
                {
                    field.AddCssClass(fieldClass);
                }
                if (!string.IsNullOrEmpty(dataField))
                {
                    field.Attributes.Add("data-field", dataField);
                }

                IHtmlContent htmlLabel;
                if (expression is Expression<Func<TModel, bool>>)
                {
                    htmlLabel = HtmlString.Empty;
                }
                else
                {
                    htmlLabel = htmlHelper.BootstrapFormLabel(expression, labelFormat);
                    _ = field.InnerHtml.AppendHtml(htmlLabel);
                }

                var htmlField = htmlHelper.BootstrapFormInputControl(expression, selectList, controlClass, optionLabel ? "(Selection Required)" : null, htmlLabel == HtmlString.Empty);
                _ = field.InnerHtml.AppendHtml(htmlField);
            }

            return field;
        }

        public static IHtmlContent ControlCssClasses(this IHtmlHelper htmlHelper)
        {
            var bootstrapForm = htmlHelper.GetBootstrapForm();

            var result = $"{ColumnClasses(bootstrapForm.FieldWidth, bootstrapForm.LabelWidth)} {OffsetClasses(bootstrapForm.LabelWidth)}";

            return new HtmlString(result);
        }

        public static BootstrapInput BeginBootstrapInput<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> selectList = null, string fieldClass = null, string controlClass = null, string dataField = null, bool optionLabel = false, string labelFormat = null)
        {
            var bootstrapForm = htmlHelper.GetBootstrapForm();
            bootstrapForm.ContainsFields = true;

            var field = new TagBuilder("div");
            {
                field.TagRenderMode = TagRenderMode.StartTag;
                field.AddCssClass("row");
                field.AddCssClass("form-group");
                if (bootstrapForm.Compact)
                {
                    field.AddCssClass("mb-0");
                }
                if (!string.IsNullOrEmpty(fieldClass))
                {
                    field.AddCssClass(fieldClass);
                }
                if (!string.IsNullOrEmpty(dataField))
                {
                    field.Attributes.Add("data-field", dataField);
                }

            }
            htmlHelper.ViewContext.Writer.Write(field);

            var htmlLabel = htmlHelper.BootstrapFormLabel(expression, labelFormat);
            htmlHelper.ViewContext.Writer.Write(htmlLabel);

            var htmlField = htmlHelper.BootstrapFormInputControl(expression, selectList, controlClass, optionLabel ? "(Selection Required)" : null, htmlLabel == HtmlString.Empty);
            htmlHelper.ViewContext.Writer.Write(htmlField);

            var htmlDescription = new TagBuilder("div");
            {
                htmlDescription.TagRenderMode = TagRenderMode.StartTag;
                htmlDescription.AddCssClass($"{ColumnClasses(bootstrapForm.DescriptionWidth, bootstrapForm.FieldWidth + bootstrapForm.LabelWidth)}");
            }
            htmlHelper.ViewContext.Writer.Write(htmlDescription);

            var bootstrapInput = new BootstrapInput(htmlHelper.ViewContext.Writer);

            return bootstrapInput;
        }

        #endregion

        public static BootstrapButtonRow BootstrapBeginFormButtonRow(this IHtmlHelper htmlHelper)
        {
            var bootstrapForm = htmlHelper.GetBootstrapForm();

            // <div class="form-group [form-group-compressed]">
            //
            {
                var divFormGroup = new TagBuilder("div");
                {
                    divFormGroup.TagRenderMode = TagRenderMode.StartTag;
                    divFormGroup.AddCssClass("form-group");
                    divFormGroup.AddCssClass("row");
                    if (bootstrapForm.Compact)
                    {
                        divFormGroup.AddCssClass("form-group--condensed");
                    }
                }

                htmlHelper.ViewContext.Writer.Write(divFormGroup.GetHtml());
            }

            // <div class="col-sm-{0}">
            //
            {
                var divColumn = new TagBuilder("div");
                {
                    divColumn.TagRenderMode = TagRenderMode.StartTag;
                    divColumn.AddCssClass("col");
                    divColumn.AddCssClass($"{ColumnClasses(bootstrapForm.FieldWidth, bootstrapForm.LabelWidth)}");
                    divColumn.AddCssClass($"{OffsetClasses(bootstrapForm.LabelWidth)}");

                    if (bootstrapForm.AlignRight)
                    {
                        divColumn.AddCssClass("text-right");
                    }
                }
                htmlHelper.ViewContext.Writer.Write(divColumn.GetHtml());
            }

            return new BootstrapButtonRow(htmlHelper.ViewContext.Writer);
        }

        public static BootstrapGridButtonRow BootstrapBeginGridButtonRow(this IHtmlHelper htmlHelper)
        {
            // <div class="form-group [form-group-compressed]">
            //
            {
                var divFormGroup = new TagBuilder("div");
                {
                    divFormGroup.TagRenderMode = TagRenderMode.StartTag;
                    divFormGroup.AddCssClass("list-grid__item-options");
                }
                htmlHelper.ViewContext.Writer.Write(divFormGroup.GetHtml());
            }

            return new BootstrapGridButtonRow(htmlHelper.ViewContext.Writer);
        }

        public static HtmlString BootstrapValidationSummary(this IHtmlHelper htmlHelper)
        {
            var sb = new StringBuilder();

            if (!htmlHelper.ViewData.ModelState.IsValid)
            {
                var messages = new List<string>();
                foreach (var key in htmlHelper.ViewData.ModelState.Keys)
                {
                    foreach (var err in htmlHelper.ViewData.ModelState[key].Errors)
                    {
                        if (!string.IsNullOrEmpty(err.ErrorMessage))
                        {
                            messages.Add(htmlHelper.Encode(err.ErrorMessage));
                        }
                        else if (err.Exception != null)
                        {
                            messages.Add(htmlHelper.Encode(err.Exception.Message));
                        }
                        else
                        {
                            messages.Add("An unknown error has occurred.");
                        }
                    }
                }

                foreach (var message in messages)
                {
                    _ = sb.Append("<div class='alert alert-danger' role='alert'>");
                    _ = sb.Append(message);
                    _ = sb.Append("</div>");
                }
            }

            return new HtmlString(sb.ToString());
        }

        private static IHtmlContent BootstrapFormLabel<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string labelFormat)
        {
            var bootstrapForm = htmlHelper.GetBootstrapForm();

            var displayName = GetDisplayName(expression, false);
            if (string.IsNullOrEmpty(displayName) && string.IsNullOrEmpty(labelFormat))
            {
                return HtmlString.Empty;
            }

            var labelClass = $"{ColumnClasses(bootstrapForm.LabelWidth)} col-form-label col-form-label-sm";
            if (bootstrapForm.AlignLabelRight)
            {
                labelClass += " text-lg-right";
            }

            if (labelFormat != null)
            {
                var labelText = string.Format(labelFormat, displayName);
                return !string.IsNullOrEmpty(labelText)
                    ? htmlHelper.LabelFor(expression, labelText, new { @class = labelClass })
                    : HtmlString.Empty;
            }

            return htmlHelper.LabelFor(expression, new { @class = labelClass });
        }

        private static IHtmlContent BootstrapFormDisplayControl<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string linkText, string link, bool alignRight, bool offsetRequired)
        {
            var bootstrapForm = htmlHelper.GetBootstrapForm();

            var expressionValue = expression.Compile()(htmlHelper.ViewData.Model);
            if (bootstrapForm.SuppressNullValues && expressionValue == null)
            {
                return HtmlString.Empty;
            }

            // Create input control.
            //
            var htmlDivControl = new TagBuilder("div");
            {
                htmlDivControl.AddCssClass("form-control-plaintext form-control-sm d-inline-block");
                if (alignRight)
                {
                    htmlDivControl.AddCssClass("text-right");
                }

                IHtmlContent htmlControl;
                if (expressionValue is IEnumerable<string> enumerableExpressionValue)
                {
                    var sb = new StringBuilder();
                    var prefix = "";
                    foreach (var value in enumerableExpressionValue)
                    {
                        _ = sb.Append(prefix); prefix = "<br />";
                        _ = sb.Append(value);
                    }

                    htmlControl = new HtmlString(sb.ToString());
                }
                else if (expressionValue is IBootstrapAddress bootstrapAddress)
                {
                    var sb = new StringBuilder();
                    var prefix = "";
                    foreach (var value in BootstrapUtility.FormatAddress(bootstrapAddress))
                    {
                        _ = sb.Append(prefix); prefix = "<br />";
                        _ = sb.Append(value);
                    }

                    htmlControl = new HtmlString(sb.ToString());
                }
                else
                {
                    htmlControl = htmlHelper.DisplayFor(expression);
                }

                _ = htmlDivControl.InnerHtml.AppendHtml(htmlControl);

                if (!string.IsNullOrEmpty(link) && string.IsNullOrEmpty(linkText))
                {
                    var anchor = new TagBuilder("a");
                    anchor.MergeAttribute("href", link);
                    _ = anchor.InnerHtml.Append("View");

                    _ = htmlDivControl.InnerHtml
                        .Append(" (")
                        .AppendHtml(anchor)
                        .Append(")");
                }

            }

            // Create input div
            //
            var htmlDivColumn = new TagBuilder("div");
            {
                htmlDivColumn.AddCssClass($"{ColumnClasses(bootstrapForm.FieldWidth, bootstrapForm.LabelWidth)}");
                if (offsetRequired)
                {
                    htmlDivColumn.AddCssClass($"{OffsetClasses(bootstrapForm.LabelWidth)}");
                }

                _ = htmlDivColumn.InnerHtml.AppendHtml(htmlDivControl);
            }

            return htmlDivColumn;
        }

        private static IHtmlContent BootstrapFormDescription<TModel>(this IHtmlHelper<TModel> htmlHelper, string linkText, string link)
        {
            var bootstrapForm = htmlHelper.GetBootstrapForm();

            if (string.IsNullOrEmpty(link) || string.IsNullOrEmpty(linkText))
            {
                return HtmlString.Empty;
            }

            var htmlDescription = new TagBuilder("div");
            {
                htmlDescription.AddCssClass($"{ColumnClasses(bootstrapForm.DescriptionWidth, bootstrapForm.FieldWidth + bootstrapForm.LabelWidth)}");

                var htmlLinkDivControl = new TagBuilder("a");
                htmlLinkDivControl.AddCssClass(ButtonCssClasses);
                htmlLinkDivControl.MergeAttribute("href", link);
                _ = htmlLinkDivControl.InnerHtml.Append(linkText);

                _ = htmlDescription.InnerHtml.AppendHtml(htmlLinkDivControl);
            }

            return htmlDescription;
        }

        private static IHtmlContent BootstrapFormInputControl<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> selectList, string controlClass, string optionLabel, bool offset)
        {
            var bootstrapForm = htmlHelper.GetBootstrapForm();
            bootstrapForm.ContainsFields = true;

            var provider = htmlHelper.ViewContext.HttpContext.RequestServices.GetService<ModelExpressionProvider>();
            var propertyName = provider.GetExpressionText(expression);

            var modelState = htmlHelper.ViewData.ModelState[propertyName];
            var inError = modelState?.Errors.Count > 0;

            var name = htmlHelper.NameFor(expression);
            var id = htmlHelper.IdFor(expression);

            // Create input control.
            //
            IHtmlContent htmlFormControl;
            {
                var inputClass = "form-control";
                if (!string.IsNullOrEmpty(controlClass))
                {
                    inputClass += " ";
                    inputClass += controlClass;
                }
                if (inError)
                {
                    inputClass += " ";
                    inputClass += "is-invalid";
                }

                var dataType = GetDataType(expression) ?? DataType.Custom;
                if (selectList != null)
                {
                    htmlFormControl = htmlHelper.DropDownListFor(expression, selectList, optionLabel, new
                    {
                        @class = inputClass,
                        required = GetIsRequired(expression),
                    });
                }
                else if (dataType == DataType.Password)
                {
                    var htmlAttributes = new RouteValueDictionary();
                    if (!string.IsNullOrEmpty(inputClass))
                    {
                        htmlAttributes["class"] = inputClass;
                    }
                    var minimumLength = GetMinimumLength(expression);
                    if (!string.IsNullOrEmpty(minimumLength))
                    {
                        htmlAttributes["minlength"] = minimumLength;
                    }
                    var maximumLength = GetMaximumLength(expression);
                    if (!string.IsNullOrEmpty(maximumLength))
                    {
                        htmlAttributes["maxlength"] = maximumLength;
                    }
                    if (GetIsRequired(expression))
                    {
                        htmlAttributes["required"] = "required";
                    }

                    htmlFormControl = htmlHelper.PasswordFor(expression, htmlAttributes);
                }
                else if (expression is Expression<Func<TModel, bool>> booleanExpression)
                {
                    var htmlFormCheck = new TagBuilder("div");
                    htmlFormCheck.AddCssClass("form-check");

                    _ = htmlFormCheck.InnerHtml.AppendHtml(
                        htmlHelper.CheckBoxFor(booleanExpression, new { @class = "form-check-input" }));

                    _ = htmlFormCheck.InnerHtml.AppendHtml(
                        htmlHelper.LabelFor(booleanExpression, GetDisplayName(booleanExpression, true), new { @class = "form-check-label" }));

                    htmlFormControl = htmlFormCheck;
                }
                else
                {
                    var htmlAttributes = new RouteValueDictionary();
                    if (!string.IsNullOrEmpty(inputClass))
                    {
                        htmlAttributes["class"] = inputClass;
                    }
                    var minimumLength = GetMinimumLength(expression);
                    if (!string.IsNullOrEmpty(minimumLength))
                    {
                        htmlAttributes["minlength"] = minimumLength;
                    }
                    var maximumLength = GetMaximumLength(expression);
                    if (!string.IsNullOrEmpty(maximumLength))
                    {
                        htmlAttributes["maxlength"] = maximumLength;
                    }
                    if (GetIsRequired(expression))
                    {
                        htmlAttributes["required"] = "required";
                    }
                    if (GetType(expression) == typeof(int))
                    {
                        htmlAttributes["number"] = "number";
                    }
                    if (dataType == DataType.EmailAddress)
                    {
                        htmlAttributes["email"] = "email";
                    }
                    if (dataType == DataType.Date)
                    {
                        htmlAttributes["type"] = "date";
                    }

                    if (dataType == DataType.MultilineText)
                    {
                        var rows = GetMultilineRows(expression);
                        htmlFormControl = htmlHelper.TextAreaFor(expression, rows, 0, htmlAttributes);
                    }
                    else
                    {
                        htmlFormControl = htmlHelper.EditorFor(expression, new { htmlAttributes });
                    }
                }
            }

            // Create input field error feedback.
            //
            var htmlFormControlFeedback = new TagBuilder("div");
            {
                htmlFormControlFeedback.Attributes["id"] = $"{id}-error";
                htmlFormControlFeedback.Attributes["data-valmsg-for"] = name;
                htmlFormControlFeedback.Attributes["data-valmsg-replace"] = "true";
                htmlFormControlFeedback.AddCssClass("text-danger");
            }

            // Create input div.
            //
            var htmlField = new TagBuilder("div");
            {
                htmlField.AddCssClass($"{ColumnClasses(bootstrapForm.FieldWidth, bootstrapForm.LabelWidth)}");
                if (offset)
                {
                    htmlField.AddCssClass($"{OffsetClasses(bootstrapForm.LabelWidth)}");
                }

                _ = htmlField.InnerHtml.AppendHtml(htmlFormControl);
                _ = htmlField.InnerHtml.AppendHtml(htmlFormControlFeedback);
            }

            return htmlField;
        }

        private static BootstrapForm GetBootstrapForm(this IHtmlHelper htmlHelper)
        {
            return (BootstrapForm)htmlHelper.ViewContext.ViewData[BootstrapFormViewKey];
        }

        private static DataType? GetDataType<TSource, TProperty>(Expression<Func<TSource, TProperty>> expression)
        {
            return Attribute.GetCustomAttribute(((MemberExpression)expression.Body).Member, typeof(DataTypeAttribute)) is DataTypeAttribute attribute
                ? (DataType?)attribute.DataType
                : null;
        }

        private static string GetDisplayName<TSource, TProperty>(Expression<Func<TSource, TProperty>> expression, bool allowDefault)
        {
            var member = (MemberExpression)expression.Body;

            return Attribute.GetCustomAttribute(member.Member, typeof(DisplayAttribute)) is DisplayAttribute attribute
                ? attribute.Name
                : allowDefault
                    ? member.Member.Name
                    : null;
        }

        private static bool GetIsRequired<TSource, TProperty>(Expression<Func<TSource, TProperty>> expression)
        {
            return Attribute.GetCustomAttribute(((MemberExpression)expression.Body).Member, typeof(RequiredAttribute)) is RequiredAttribute;
        }

        private static string GetMaximumLength<TSource, TProperty>(Expression<Func<TSource, TProperty>> expression)
        {
            if (Attribute.GetCustomAttribute(((MemberExpression)expression.Body).Member, typeof(StringLengthAttribute)) is StringLengthAttribute stringLength)
            {
                return stringLength.MaximumLength.ToString();
            }

            if (Attribute.GetCustomAttribute(((MemberExpression)expression.Body).Member, typeof(MaxLengthAttribute)) is MaxLengthAttribute maxLength)
            {
                return maxLength.Length.ToString();
            }

            //var integer = Attribute.GetCustomAttribute(((MemberExpression)expression.Body).Member, typeof(IntegerAttribute)) as IntegerAttribute;
            //if (integer != null)
            //{
            //    return integer.MaxLength.ToString();
            //}

            return "";
        }

        private static string GetMinimumLength<TSource, TProperty>(Expression<Func<TSource, TProperty>> expression)
        {
            return Attribute.GetCustomAttribute(((MemberExpression)expression.Body).Member, typeof(StringLengthAttribute)) is StringLengthAttribute stringLength
                ? stringLength.MinimumLength.ToString()
                : "";
        }

        private static int GetMultilineRows<TSource, TProperty>(Expression<Func<TSource, TProperty>> expression)
        {
            return Attribute.GetCustomAttribute(((MemberExpression)expression.Body).Member, typeof(MultilineAttribute)) is MultilineAttribute multilineRows
                ? multilineRows.Rows
                : 0;
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for generics.")]
        private static Type GetType<TSource, TProperty>(Expression<Func<TSource, TProperty>> expression)
        {
            return typeof(TProperty);
        }

        private static string ColumnClasses(int width)
        {
            var sb = new StringBuilder();

            int smColumns = SmColumns(width);
            _ = sb.Append($"col-sm-{smColumns}");

            int mdColumns = MdColumns(width);
            if (mdColumns < smColumns)
            {
                _ = sb.Append($" col-md-{mdColumns}");
            }

            int lgColumns = LgColumns(width);
            if (lgColumns < mdColumns)
            {
                _ = sb.Append($" col-lg-{lgColumns}");
            }

            int xlColumns = XlColumns(width);
            if (xlColumns < lgColumns)
            {
                _ = sb.Append($" col-xl-{xlColumns}");
            }

            return sb.ToString();
        }

        private static string ColumnClasses(int width, int reservedWidth)
        {
            var sb = new StringBuilder();

            int smColumns = SmColumns(width, 12 - SmColumns(reservedWidth));
            if (smColumns > 0)
            {
                _ = sb.Append($"col-sm-{smColumns}");
            }

            int mdColumns = MdColumns(width, 12 - MdColumns(reservedWidth));
            if (mdColumns > 0 && mdColumns != smColumns)
            {
                _ = sb.Append($" col-md-{mdColumns}");
            }

            int lgColumns = LgColumns(width, 12 - LgColumns(reservedWidth));
            if (lgColumns > 0 && lgColumns != mdColumns)
            {
                _ = sb.Append($" col-lg-{lgColumns}");
            }

            int xlColumns = XlColumns(width, 12 - XlColumns(reservedWidth));
            if (xlColumns > 0 && xlColumns != lgColumns)
            {
                _ = sb.Append($" col-xl-{xlColumns}");
            }

            return sb.ToString();
        }

        private static string OffsetClasses(int width)
        {
            var sb = new StringBuilder();

            int smColumns = SmColumns(width);
            if (smColumns > 0)
            {
                _ = sb.Append($"offset-sm-{smColumns}");
            }

            int mdColumns = MdColumns(width);
            if (mdColumns > 0 && mdColumns < smColumns)
            {
                _ = sb.Append($" offset-md-{mdColumns}");
            }

            int lgColumns = LgColumns(width);
            if (lgColumns > 0 && lgColumns < mdColumns)
            {
                _ = sb.Append($" offset-lg-{lgColumns}");
            }

            int xlColumns = XlColumns(width);
            if (xlColumns > 0 && xlColumns < lgColumns)
            {
                _ = sb.Append($" offset-xl-{xlColumns}");
            }

            return sb.ToString();
        }

        private static int SmColumns(int width, int? maxColumns = null)
        {
            if (width == 0) return 0;

            var percent = width / 540.0;
            var cols = (int)(12 * percent);
            cols = Math.Max(cols, 1);
            if (maxColumns.HasValue)
            {
                cols = Math.Min(cols, maxColumns.Value);
            }
            return cols;
        }

        private static int MdColumns(int width, int? maxColumns = null)
        {
            if (width == 0) return 0;

            var percent = width / 720.0;
            var cols = (int)(12 * percent);
            cols = Math.Max(cols, 1);
            if (maxColumns.HasValue)
            {
                cols = Math.Min(cols, maxColumns.Value);
            }
            return cols;
        }

        private static int LgColumns(int width, int? maxColumns = null)
        {
            if (width == 0) return 0;

            var percent = width / 960.0;
            var cols = (int)(12 * percent);
            cols = Math.Max(cols, 1);
            if (maxColumns.HasValue)
            {
                cols = Math.Min(cols, maxColumns.Value);
            }
            return cols;
        }

        private static int XlColumns(int width, int? maxColumns = null)
        {
            if (width == 0) return 0;

            var percent = width / 1140.0;
            var cols = (int)(12 * percent);
            cols = Math.Max(cols, 1);
            if (maxColumns.HasValue)
            {
                cols = Math.Min(cols, maxColumns.Value);
            }
            return cols;
        }
    }
}