using EDennis.BlazorUtils;

namespace EDennis.BlazorHits.Services
{
    public class SongService : CrudService<HitsContext, Song>
    {
        public SongService(CrudServiceDependencies<HitsContext, Song> deps) : base(deps) { }
    }
}
