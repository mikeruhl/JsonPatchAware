using System;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JsonPatchAware.Mvc
{
public static class JsonPatchExtensions
    {
        /// <summary>
        /// Applies JSON patch operations on object and logs errors in <see cref="ModelStateDictionary"/>.
        /// </summary>
        /// <param name="patchDoc">The <see cref="JsonPatchDocument{T}"/>.</param>
        /// <param name="objectToApplyTo">The entity on which <see cref="JsonPatchDocument{T}"/> is applied.</param>
        /// <param name="objectAdapter">Object Adapter to use</param>
        /// <param name="modelState">The <see cref="ModelStateDictionary"/> to add errors.</param>
        public static void ApplyTo<T>(
            this JsonPatchDocument<T> patchDoc,
            T objectToApplyTo,
            IObjectAdapter objectAdapter,
            ModelStateDictionary modelState) where T : class
        {
            if (patchDoc == null)
            {
                throw new ArgumentNullException(nameof(patchDoc));
            }

            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }

            patchDoc.ApplyTo(objectToApplyTo, objectAdapter, modelState, string.Empty);
        }

        /// <summary>
        /// Applies JSON patch operations on object and logs errors in <see cref="T:Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary" />.
        /// </summary>
        /// <param name="patchDoc">The <see cref="T:Microsoft.AspNetCore.JsonPatch.JsonPatchDocument`1" />.</param>
        /// <param name="objectToApplyTo">The entity on which <see cref="T:Microsoft.AspNetCore.JsonPatch.JsonPatchDocument`1" /> is applied.</param>
        /// <param name="objectAdapter">Object Adapter to use</param>
        /// <param name="modelState">The <see cref="T:Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary" /> to add errors.</param>
        /// <param name="prefix">The prefix to use when looking up values in <see cref="T:Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary" />.</param>
        public static void ApplyTo<T>(
            this JsonPatchDocument<T> patchDoc,
            T objectToApplyTo, 
            IObjectAdapter objectAdapter,
            ModelStateDictionary modelState,
            string prefix) where T : class
        {
            if (patchDoc == null)
            {
                throw new ArgumentNullException(nameof(patchDoc));
            }

            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }

            patchDoc.ApplyTo(objectToApplyTo, objectAdapter, jsonPatchError =>
            {
                var affectedObjectName = jsonPatchError.AffectedObject.GetType().Name;
                var key = string.IsNullOrEmpty(prefix) ? affectedObjectName : prefix + "." + affectedObjectName;

                modelState.TryAddModelError(key, jsonPatchError.ErrorMessage);
            });
        }
    }
}
