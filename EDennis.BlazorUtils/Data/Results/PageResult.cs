namespace EDennis.BlazorUtils
{
    /// <summary>
    /// Wrapper for a query result that includes the records for a given
    /// page of data and the count of records across pages. This
    /// generic version of PageResult provides a response that
    /// includes a typed list of data.
    /// See <see cref="PageResult"/>
    /// </summary>
    /// <typeparam name="TEntity">The entity type of each record/object</typeparam>
    public class PageResult<TEntity>
    {
        /// <summary>
        /// The page of data
        /// </summary>
        public List<TEntity> Data { get; set; }

        /// <summary>
        /// The count of records across all pages
        /// </summary>
        public int CountAcrossPages { get; set; }
    }

    /// <summary>
    /// Wrapper for a query result that includes the records for a given
    /// page of data and the count of records across pages. This
    /// version of PageResult provides a response that includes an
    /// untyped list of data.
    /// See <see cref="PageResult"/>
    /// </summary>
    public class PageResult
    {
        /// <summary>
        /// The page of data
        /// </summary>
        public List<object> Data { get; set; }

        /// <summary>
        /// The count of records across all pages
        /// </summary>
        public int CountAcrossPages { get; set; }
    }

}
