using System.Diagnostics.CodeAnalysis;

namespace EDennis.BlazorUtils
{
    public partial class CountCache<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Internal class for holding the cached record count and last calculated date.
        /// The IEqualityComparer is used to simplify comparison of objects
        /// </summary>
        public class CountAndDate : IEqualityComparer<CountAndDate>
        {
            public int Count { get; set; }
            public DateTime LastCalculated { get; set; }

            public bool Equals(CountAndDate x, CountAndDate y)
                => x.Count == y.Count && x.LastCalculated == y.LastCalculated;

            public int GetHashCode([DisallowNull] CountAndDate obj)
                => HashCode.Combine(obj.Count, obj.LastCalculated);
        }



    }

}
