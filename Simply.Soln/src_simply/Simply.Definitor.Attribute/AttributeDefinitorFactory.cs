using Simply.Common.Interfaces;

namespace Simply.Definitor.Attribute
{
    /// <summary>
    /// Definitor Factory class for Attribute mapping.
    /// </summary>
    public sealed class AttributeDefinitorFactory : ISimpleDefinitorFactory
    {
        /// <summary>
        /// Gets the definitor.
        /// </summary>
        /// <returns>A ISimpleDefinitor.</returns>
        public ISimpleDefinitor<T> GetDefinitor<T>() where T : class
        {
            return new AttributeDefinitor<T>();
        }
    }
}