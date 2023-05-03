namespace EDennis.BlazorUtils
{
    public class PageResult<TEntity>
    {
        public List<TEntity> Data { get; set; }
        public int CountAcrossPages { get; set; }
    }

    public class PageResult
    {
        public List<object> Data { get; set; }
        public int CountAcrossPages { get; set; }
    }

}
