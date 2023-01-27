namespace Simply.Common.Interfaces
{
    /// <summary>
    /// Factory interface for generating ISimpleDefinitor object.
    /// </summary>
    public interface ISimpleDefinitorFactory
    {
        /// <summary>
        /// Gets the definitor.
        /// </summary>
        /// <returns>A ISimpleDefinitor object instance.</returns>
        ISimpleDefinitor<T> GetDefinitor<T>() where T : class;
    }
}