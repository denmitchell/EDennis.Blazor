using EDennis.BlazorUtils;

namespace EDennis.BlazorHits
{
    public static class TestRecords
    {
        public static IEnumerable<AppRole> GetAppRoles()
        {
            var roles = new List<AppRole>
            {
                new AppRole
                {
                    Id = -1,
                    RoleName = "IT",
                    SysGuid = GuidUtils.FromId(1)
                },
                new AppRole
                {
                    Id = -2,
                    RoleName = "admin",
                    SysGuid = GuidUtils.FromId(2)
                },
                new AppRole
                {
                    Id = -3,
                    RoleName = "user",
                    SysGuid = GuidUtils.FromId(3)
                },
                new AppRole
                {
                    Id = -4,
                    RoleName = "readonly",
                    SysGuid = GuidUtils.FromId(4)
                },
                new AppRole
                {
                    Id = -5,
                    RoleName = "disabled",
                    SysGuid = GuidUtils.FromId(5)
                },
            };

            return roles;
        }

        public static IEnumerable<AppUser> GetAppUsers()
        {
            var users = new List<AppUser>
            {
                new AppUser
                {
                    Id = -1,
                    UserName = "Starbuck",
                    SysGuid = GuidUtils.FromId(1),
                    RoleId = -1 
                },
                new AppUser
                {
                    Id = -2,
                    UserName = "Maria",
                    SysGuid = GuidUtils.FromId(2),
                    RoleId = -2
                },
                new AppUser
                {
                    Id = -3,
                    UserName = "Darius",
                    SysGuid = GuidUtils.FromId(3),
                    RoleId = -3
                },
                new AppUser
                {
                    Id = -4,
                    UserName = "Huan",
                    SysGuid = GuidUtils.FromId(4),
                    RoleId = -4
                },
                new AppUser
                {
                    Id = -5,
                    UserName = "Jack",
                    SysGuid = GuidUtils.FromId(5),
                    RoleId = -5
                },
            };

            return users;
        }

        public static IEnumerable<Artist> GetArtists()
        {
            var artists = new Artist[]
            {
                new Artist
                {
                    Id = -1,
                    Name = "Beatles",
                    IsSolo = false,
                    SysGuid = GuidUtils.FromId(1)
                },
                new Artist
                {
                    Id = -2,
                    Name = "Billy Joel",
                    IsSolo = true,
                    SysGuid = GuidUtils.FromId(2)
                },
                new Artist
                {
                    Id = -3,
                    Name = "Led Zeppelin",
                    IsSolo = false,
                    SysGuid = GuidUtils.FromId(3)
                },
                new Artist
                {
                    Id = -4,
                    Name = "Eagles",
                    IsSolo = false,
                    SysGuid = GuidUtils.FromId(4)
                },
                new Artist
                {
                    Id = -5,
                    Name = "Marvin Gaye",
                    IsSolo = true,
                    SysGuid = GuidUtils.FromId(5)
                },
                new Artist
                {
                    Id = -6,
                    Name = "Queen",
                    IsSolo = false,
                    SysGuid = GuidUtils.FromId(6)
                },
                new Artist
                {
                    Id = -7,
                    Name = "Aretha Franklin",
                    IsSolo = true, 
                    SysGuid = GuidUtils.FromId(7)
                },
                new Artist
                {
                    Id = -8,
                    Name = "Aerosmith",
                    IsSolo = false, 
                    SysGuid = GuidUtils.FromId(8)
                }
            };
            return artists;
        }

        public static IEnumerable<Song> GetSongs()
        {
            var songs = new List<Song>()
            {
                new Song {
                    Id = -1,
                    Title = "Yesterday",
                    ArtistId = -1,
                    ReleaseDate = new DateTime(1965,9,13),
                    SysGuid = GuidUtils.FromId(1)
                },
                new Song {
                    Id = -2,
                    Title = "Hey Jude",
                    ArtistId = -1,
                    ReleaseDate = new DateTime(1968,9,14),
                    SysGuid = GuidUtils.FromId(2)
                },
                new Song {
                    Id = -3,
                    Title = "Piano Man",
                    ArtistId = -2,
                    ReleaseDate = new DateTime(1973,11,2),
                    SysGuid = GuidUtils.FromId(3)
                },
                new Song {
                    Id = -4,
                    Title = "My Life",
                    ArtistId = -2,
                    ReleaseDate = new DateTime(1978,10,13),
                    SysGuid = GuidUtils.FromId(4)
                },
                new Song {
                    Id = -5,
                    Title = "Kashmir",
                    ArtistId = -3,
                    ReleaseDate = new DateTime(1975,2,24),
                    SysGuid = GuidUtils.FromId(5)
                },
                new Song {
                    Id = -6,
                    Title = "Fool in the Rain",
                    ArtistId = -3,
                    ReleaseDate = new DateTime(1979,12,7),
                    SysGuid = GuidUtils.FromId(6)
                },
                new Song {
                    Id = -7,
                    Title = "Hotel California",
                    ArtistId = -4,
                    ReleaseDate = new DateTime(1976,12,8),
                    SysGuid = GuidUtils.FromId(7)
                },
                new Song {
                    Id = -8,
                    Title = "Take It Easy",
                    ArtistId = -4,
                    ReleaseDate = new DateTime(1972,5,1),
                    SysGuid = GuidUtils.FromId(8)
                },
                new Song {
                    Id = -9,
                    Title = "I Heard It Through the Grapevine",
                    ArtistId = -5,
                    ReleaseDate = new DateTime(1967,9,28),
                    SysGuid = GuidUtils.FromId(9)
                },
                new Song {
                    Id = -10,
                    Title = "What's Going On",
                    ArtistId = -5,
                    ReleaseDate = new DateTime(1971,5,21),
                    SysGuid = GuidUtils.FromId(10)
                },
                new Song {
                    Id = -11,
                    Title = "Bohemian Rhapsody",
                    ArtistId = -6,
                    ReleaseDate = new DateTime(1975,10,13),
                    SysGuid = GuidUtils.FromId(11)
                },
                new Song {
                    Id = -12,
                    Title = "We Will Rock You",
                    ArtistId = -6,
                    ReleaseDate = new DateTime(1977,10,7),
                    SysGuid = GuidUtils.FromId(12)
                },
                new Song {
                    Id = -13,
                    Title = "Respect",
                    ArtistId = -7,
                    ReleaseDate = new DateTime(1967,3,10),
                    SysGuid = GuidUtils.FromId(13)
                },
                new Song {
                    Id = -14,
                    Title = "Think",
                    ArtistId = -7,
                    ReleaseDate = new DateTime(1968,5,12),
                    SysGuid = GuidUtils.FromId(14)
                },
                new Song {
                    Id = -15,
                    Title = "Dream On",
                    ArtistId = -8,
                    ReleaseDate = new DateTime(1973,6,27),
                    SysGuid = GuidUtils.FromId(15)
                },
                new Song {
                    Id = -16,
                    Title = "Sweet Emotion",
                    ArtistId = -8,
                    ReleaseDate = new DateTime(1975,5,19),
                    SysGuid = GuidUtils.FromId(16)
                },
            };
            return songs;
        }

    }
}
