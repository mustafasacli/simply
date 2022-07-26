namespace Simply.Data.Interfaces
{
    /// <summary>
    ///
    /// </summary>
    public interface IPageInfo
    {
        /// <summary>
        /// Gets, sets item count for skipping.
        /// </summary>
        uint Skip { get; set; }

        /// <summary>
        /// Gets, sets item length in a page.
        /// </summary>
        uint Take { get; set; }

        /// <summary>
        /// Gets Page info is pageable.
        /// </summary>
        bool IsPageable { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="skip">item count for skipping.</param>
        /// <returns>Returns object instance</returns>
        IPageInfo SetSkip(uint skip);

        /// <summary>
        ///
        /// </summary>
        /// <param name="take">item count for take.</param>
        /// <returns>Returns object instance</returns>
        IPageInfo SetTake(uint take);
    }
}