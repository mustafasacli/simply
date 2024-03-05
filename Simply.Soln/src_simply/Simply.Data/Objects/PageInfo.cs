using Simply.Data.Constants;
using Simply.Data.Interfaces;
using System;

namespace Simply.Data.Objects
{
    /// <summary>
    /// Page Info for Getting Database Records.
    /// </summary>
    public class PageInfo : IPageInfo
    {
        /// <summary>
        /// Default instance for IPageInfo.
        /// </summary>
        public static readonly IPageInfo DefaultInstance = new PageInfo();

        /// <summary>
        /// Default instance
        /// </summary>
        private PageInfo()
        { }

        /// <summary>
        /// Get IPage instance from skip and take.
        /// </summary>
        /// <param name="skip">item count for skipping.</param>
        /// <param name="take">item length in a page</param>
        private PageInfo(uint skip, uint take)
        {
            this.Skip = skip;
            this.Take = take;
        }

        /// <summary>
        /// Get IPage instance from page number and item length.
        /// </summary>
        /// <param name="pageNumber">page number</param>
        /// <param name="pageItemLength">item length in a page</param>
        /// <returns>Returns IPage instance from page number and item length.</returns>
        /// <exception cref="ArgumentOutOfRangeException">if page number is less than 1 throw this error.</exception>
        public static IPageInfo GetPageWithPageNumber(uint pageNumber, uint pageItemLength)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(DbAppMessages.PageNumberLessThanOne);

            return new PageInfo((pageNumber - 1) * pageItemLength, pageItemLength);
        }

        /// <summary>
        /// Get IPage instance from skip and take.
        /// </summary>
        /// <param name="skip">item count for skipping.</param>
        /// <param name="take">item count for take.</param>
        /// <returns>Returns IPage instance from skip and take.</returns>
        /// <exception cref="ArgumentOutOfRangeException">if page number is less than 1 throw this error.</exception>
        public static IPageInfo GetPage(uint skip, uint take)
        {
            return new PageInfo(skip, take);
        }

        /// <summary>
        /// Gets, sets item count for skipping.
        /// </summary>
        public uint Skip
        { get; set; }

        /// <summary>
        /// Gets, sets item length in a page.
        /// </summary>
        public uint Take
        { get; set; }

        /// <summary>
        /// Gets Page info is pageable.
        /// </summary>
        public bool IsPageable
        {
            get
            {
                bool isPageable = Take > 0;
                return isPageable;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="skip">item count for skipping.</param>
        /// <returns>Returns object instance</returns>
        public IPageInfo SetSkip(uint skip)
        {
            this.Skip = skip;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="take">item count for take.</param>
        /// <returns>Returns object instance</returns>
        public IPageInfo SetTake(uint take)
        {
            this.Take = take;
            return this;
        }

        /// <summary>
        /// Returns page info object instance for First record of result set.
        /// </summary>
        /// <returns>Returns page info object instance for First record of result set.</returns>
        public static IPageInfo First()
        {
            return new PageInfo(0, 1);
        }
    }
}