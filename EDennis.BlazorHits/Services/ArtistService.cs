using EDennis.BlazorUtils;

namespace EDennis.BlazorHits.Services
{
    public class ArtistService : CrudService<HitsContext, Artist>
    {
        public ArtistService(CrudServiceDependencies<HitsContext, Artist> deps): base(deps) { }
    }
}
