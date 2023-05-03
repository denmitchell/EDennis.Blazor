namespace EDennis.BlazorUtils
{

    /// <summary>
    /// The result returned from a Dynamic Linq query.  This
    /// version of DynamicLinqResult provides a dynamic
    /// response.  The class is used when the Select property
    /// IS provided (and hence only a subset of properties are 
    /// returned).
    /// </summary>
    public class DynamicLinqResult
    {

        /// <summary>
        /// Data for the current page
        /// </summary>
        public List<dynamic> Data { get; set; }

        /// <summary>
        /// 1-based page number
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// Number of records per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of records across all pages
        /// </summary>
        public int RowCount { get; set; }
    }


    /// <summary>
    /// The result returned from a Dynamic Linq query.  This
    /// generic version of DynamicLinqResult provides a typed
    /// response.  The class is used when the Select property
    /// is not provided (and hence all properties are returned).
    /// </summary>
    /// <typeparam name="TEntity">The relevant model class</typeparam>
    public class DynamicLinqResult<TEntity>
    {

        /// <summary>
        /// The collection of entity records in the current page
        /// </summary>
        public virtual List<TEntity> Data { get; set; }

        /// <summary>
        /// The current page number of the record
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The total number of pages in the query result
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// The total number of records in each page.  The
        /// final page may contain fewer records.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of records in the query result
        /// </summary>
        public int RowCount { get; set; }
    }
}
