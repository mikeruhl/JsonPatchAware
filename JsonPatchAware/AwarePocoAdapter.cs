using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Newtonsoft.Json.Serialization;

namespace JsonPatchAware
{
public class AwarePocoAdapter:PocoAdapter
    {
        public override bool TryAdd(
            object target,
            string segment,
            IContractResolver contractResolver,
            object value,
            out string errorMessage)
        {
            var classAttributes = target.GetType().GetCustomAttributes(true);
            if(classAttributes.Any(c=>c is JsonPatchReadOnlyAttribute))
            {
                errorMessage = null;
                return true;
            }

            if (!TryGetJsonProperty(target, contractResolver, segment, out var jsonProperty))
            {
                errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
                return false;
            }

            var attributes = jsonProperty.AttributeProvider.GetAttributes(true);
            if (attributes.Any(c => c is JsonPatchReadOnlyAttribute))
            {
                errorMessage = null;
                return true;
            }

            if (!TryConvertValue(value, jsonProperty.PropertyType, out var convertedValue) | attributes.Any(c=>c is JsonPatchReadOnlyAttribute))
            {
                errorMessage = Resources.FormatInvalidValueForProperty(value);
                return false;
            }

            jsonProperty.ValueProvider.SetValue(target, convertedValue);

            errorMessage = null;
            return true;
        }

        public override bool TryRemove(
            object target,
            string segment,
            IContractResolver contractResolver,
            out string errorMessage)
        {
            var classAttributes = target.GetType().GetCustomAttributes(true);
            if(classAttributes.Any(c=>c is JsonPatchReadOnlyAttribute))
            {
                errorMessage = null;
                return true;
            }

            if (!TryGetJsonProperty(target, contractResolver, segment, out var jsonProperty))
            {
                errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
                return false;
            }

            var attributes = jsonProperty.AttributeProvider.GetAttributes(true);

            if (!jsonProperty.Writable | attributes.Any(c=>c is JsonPatchReadOnlyAttribute))
            {
                errorMessage = Resources.FormatCannotUpdateProperty(segment);
                return false;
            }

            // Setting the value to "null" will use the default value in case of value types, and
            // null in case of reference types
            object value = null;
            if (jsonProperty.PropertyType.GetTypeInfo().IsValueType
                && Nullable.GetUnderlyingType(jsonProperty.PropertyType) == null)
            {
                value = Activator.CreateInstance(jsonProperty.PropertyType);
            }

            jsonProperty.ValueProvider.SetValue(target, value);

            errorMessage = null;
            return true;
        }

        public override bool TryReplace(
            object target,
            string segment,
            IContractResolver
            contractResolver,
            object value,
            out string errorMessage)
        {
            var classAttributes = target.GetType().GetCustomAttributes(true);
            if(classAttributes.Any(c=>c is JsonPatchReadOnlyAttribute))
            {
                errorMessage = null;
                return true;
            }

            if (!TryGetJsonProperty(target, contractResolver, segment, out var jsonProperty))
            {
                errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
                return false;
            }

            var attributes = jsonProperty.AttributeProvider.GetAttributes(true);
            if (!jsonProperty.Writable | attributes.Any(c=>c is JsonPatchReadOnlyAttribute))
            {
                errorMessage = Resources.FormatCannotUpdateProperty(segment);
                return false;
            }

            if (!TryConvertValue(value, jsonProperty.PropertyType, out var convertedValue))
            {
                errorMessage = Resources.FormatInvalidValueForProperty(value);
                return false;
            }

            jsonProperty.ValueProvider.SetValue(target, convertedValue);

            errorMessage = null;
            return true;
        }
    }
}
