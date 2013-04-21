using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BuildingBlocks.DD4T.MarkupModels.ExtensionMethods;

using DD4T.ContentModel;

namespace BuildingBlocks.DD4T.MarkupModels
{
    /// <summary>
    /// Indicates that this property shouldn have the value of the ComponentPathExpression from the Linked Component Field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BooleanComponentLinkFieldAttribute : BaseBooleanTridionViewModelPropertyAttribute, IComponentLinkFieldTridionViewModelPropertyAttribute
    {
        public string ComponentFieldName { get; set; }

        public bool IsLinkedFieldMetadata { get; set; }

        public BooleanComponentLinkFieldAttribute(string schemaFieldName, string componentExpression)
            : base(schemaFieldName)
        {
            ComponentFieldName = componentExpression;
        }

        public override bool GetValue(IComponent component)
        {
            string value = component.GetComponentLinkedValue(SchemaFieldName, IsMetadata, ComponentFieldName);
            return ParseString(value);
        }

        public override IEnumerable<bool> GetMultiValue(IComponent component)
        {
            var values = component.GetComponentLinkedMultiValue(SchemaFieldName, IsMetadata, ComponentFieldName);
            return values.Select(ParseString).ToList();
        }
    }
}