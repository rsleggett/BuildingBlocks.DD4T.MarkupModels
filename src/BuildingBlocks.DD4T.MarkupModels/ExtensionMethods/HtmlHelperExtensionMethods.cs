﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;

using DD4T.ContentModel;
using DD4T.Mvc.Html;

namespace BuildingBlocks.DD4T.MarkupModels.ExtensionMethods
{
    ///<summary>
    /// HtmlHelperExtensionMethods is responsible for adding Experience Manager JSON
    /// Author: Robert Stevenson-Leggett
    /// Date: 2013-04-13
    ///</summary>
    public static class HtmlHelperExtensionMethods
    {
        public static MvcHtmlString MarkComponentPresentationInlineEditable(this HtmlHelper helper)
        {
            var componentPresentation = GetComponentPresentation(helper);
                
            if(componentPresentation != null)
            {
                return helper.SiteEditComponentPresentation(componentPresentation);
            }

            return new MvcHtmlString(string.Empty);
        }

        public static MvcHtmlString MarkComponentPresentationInlineEditable<T>(this HtmlHelper helper, T model, int index)
        {
            var attribute = (TridionViewModelAttribute)typeof(T).GetCustomAttributes(typeof(TridionViewModelAttribute), true).FirstOrDefault();
            if (attribute!=null && !String.IsNullOrEmpty(attribute.ParentComponentLinkFieldName))
            {
                var parentComponentPresentation = GetComponentPresentation(helper);
                var linkedComponent = parentComponentPresentation.Component.Fields[attribute.ParentComponentLinkFieldName]
                    .LinkedComponentValues[index];

                if (linkedComponent != null)
                {
                    return helper.SiteEditComponentPresentation(linkedComponent,
                                                                parentComponentPresentation.ComponentTemplate.Id, false,
                                                                string.Empty);
                }

            }
            return new MvcHtmlString(string.Empty);
        }

        public static MvcHtmlString InlineEditable<T,TP>(this HtmlHelper<T> helper, Expression<Func<T,TP>> fieldSelector)
        {
            T model = helper.ViewData.Model;
            return InlineEditableInternal(helper, model, fieldSelector, 0);
        }

        public static MvcHtmlString InlineEditable<T,TP>(this HtmlHelper helper, T model, Expression<Func<T,TP>> fieldSelector)
        {
            return InlineEditableInternal(helper, model, fieldSelector, 0);
        }
        
        public static MvcHtmlString InlineEditable<T, TP>(this HtmlHelper helper, T model, Expression<Func<T, TP>> fieldSelector, int index)
        {
            return InlineEditableInternal(helper, model, fieldSelector, index);
        }

        private static MvcHtmlString InlineEditableInternal<T,TP>(this HtmlHelper helper, T model, Expression<Func<T, TP>> fieldSelector, int index)
        {
            Func<T, TP> compiledFieldSelector = fieldSelector.Compile();
            TP value = compiledFieldSelector(model);
            var sb = new StringBuilder();
            sb.Append(GetInlineEditableMarkupInternal(helper, fieldSelector, index));
            sb.Append(value);
            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString GetInlineEditableFieldMarkup<T,TP>(this HtmlHelper helper, T model, Expression<Func<T,TP>> fieldSelector, int index)
        {
            return GetInlineEditableMarkupInternal(helper, fieldSelector, index);
        }        
        
        public static MvcHtmlString GetInlineEditableFieldMarkup<T,TP>(this HtmlHelper<T> helper, Expression<Func<T,TP>> fieldSelector)
        {
            return GetInlineEditableMarkupInternal(helper, fieldSelector, 0);
        }

        public static MvcHtmlString GetInlineEditableFieldMarkup<T,TP>(this HtmlHelper helper, Expression<Func<T,TP>> fieldSelector)
        {
            return GetInlineEditableMarkupInternal(helper, fieldSelector, 0);
        }

        public static MvcHtmlString GetInlineEditableFieldMarkup<T, TP>(this HtmlHelper helper, Expression<Func<T, TP>> fieldSelector, int index)
        {
            return GetInlineEditableMarkupInternal(helper, fieldSelector, index);
        }

        private static MvcHtmlString GetInlineEditableMarkupInternal<T, TP>(HtmlHelper helper, Expression<Func<T, TP>> fieldSelector, int index)
        {
             var componentPresentation = GetComponentPresentation(helper);

            if (componentPresentation != null)
            {
                var member = (MemberExpression)fieldSelector.Body;
                var attributes = member.Member.GetCustomAttributes(typeof(ITridionViewModelPropertyAttribute), true);
                if (attributes.Any())
                {
                    var attribute = (ITridionViewModelPropertyAttribute)attributes.First();
                    if (attribute.InlineEditable)
                    {
                        var viewModelAttribute = (TridionViewModelAttribute)member.Member.DeclaringType.GetCustomAttributes(typeof(TridionViewModelAttribute), true).FirstOrDefault();
                        IFieldSet fields;
                        var component = componentPresentation.Component;
                        if (viewModelAttribute != null && viewModelAttribute.Nested)
                        {
                            if (componentPresentation.Component.Fields.ContainsKey(viewModelAttribute.ParentComponentLinkFieldName))
                            {
                                var linkedComponent =
                                    componentPresentation.Component.Fields[
                                        viewModelAttribute.ParentComponentLinkFieldName].LinkedComponentValues[index];
                                fields = linkedComponent.Fields;
                            }
                            else
                            {
                                return new MvcHtmlString(string.Empty);
                            }
                        }
                        else
                        {
                            fields = attribute.IsMetadata
                                         ? componentPresentation.Component.MetadataFields
                                         : componentPresentation.Component.Fields;    
                        }

                        if(fields.ContainsKey(attribute.SchemaFieldName))
                        {
                            var field = fields[attribute.SchemaFieldName];

                            if (attribute is IComponentLinkFieldTridionViewModelPropertyAttribute)
                            {
                                var clAttribute = (IComponentLinkFieldTridionViewModelPropertyAttribute)attribute;
                                var clFields = clAttribute.IsLinkedFieldMetadata
                                                   ? field.LinkedComponentValues[0].MetadataFields
                                                   : field.LinkedComponentValues[0].Fields;
                                field = clFields[clAttribute.ComponentFieldName];
                                return helper.SiteEditField(component, field);
                            }

                            return helper.SiteEditField(component, field);   
                        }
                    }
                }
            }

            return new MvcHtmlString(string.Empty);  
        }

        private static IComponentPresentation GetComponentPresentation(HtmlHelper helper)
        {
            return helper.ViewContext.RouteData.Values["componentPresentation"] as IComponentPresentation;
        }
    }
}