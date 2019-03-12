using System.Globalization;
using System.Reflection;
using System.Resources;

namespace JsonPatchAware
{
    /// <summary>
    /// Resources from Asp.net Core's repository.
    /// </summary>
    internal static class Resources
    {
        private static readonly ResourceManager ResourceManager
            = new ResourceManager("Microsoft.AspNetCore.JsonPatch.Resources", typeof(Resources).GetTypeInfo().Assembly);
        /// <summary>
        /// The property at path '{0}' could not be updated.
        /// </summary>
        internal static string FormatCannotUpdateProperty(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("CannotUpdateProperty"), p0);

        /// <summary>
        /// The value '{0}' is invalid for target location.
        /// </summary>
        internal static string FormatInvalidValueForProperty(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("InvalidValueForProperty"), p0);

        /// <summary>
        /// The target location specified by path segment '{0}' was not found.
        /// </summary>
        internal static string FormatTargetLocationAtPathSegmentNotFound(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("TargetLocationAtPathSegmentNotFound"), p0);

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = ResourceManager.GetString(name);

            System.Diagnostics.Debug.Assert(value != null);

            if (formatterNames != null)
            {
                for (var i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }

            return value;
        }
    }
}
