using EDennis.BlazorHits;
using EDennis.BlazorHits.Services;
using EDennis.BlazorUtils.Utils;
using Radzen;
using Xunit.Abstractions;

namespace EDennis.BlazorUtils.Tests
{
    public class CrudServiceTests_Song : IClassFixture<CrudServiceTestFixture<HitsContext,
        SongService, Song>>
    {
        private readonly CrudServiceTestFixture<HitsContext, SongService, Song> _fixture;
        public const string _appsettingsFile = "appsettings.Test.json";
        public readonly static Dictionary<string, string> _userRoles = new() {
            { "Starbuck", "IT" },
            { "Maria", "admin" },
            { "Darius", "user" },
            { "Huan", "readonly" },
            { "Jack", "disabled" },
        };

        private readonly ITestOutputHelper _output;


        public CrudServiceTests_Song(CrudServiceTestFixture<HitsContext,
        SongService, Song> fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }


        private static Dictionary<string, object[]> _filterArgs = new()
        {
            {"TestGetPage_Date", new object[] { new DateTime(1970,1,1) } },
            {"TestGetPage_Title", new object[] { "o" } }
        };

        [Theory]
        [InlineData("ReleaseDate > @0", "TestGetPage_Date", "Title", 2, 3, 11, /*guidIds:*/ 6, 7, 5)]
        [InlineData("Title.Contains(@0)", "TestGetPage_Title", "ReleaseDate desc", 3, 4, 9, /*guidIds:*/ 11, 16, 3, 15 )]
        public async Task TestGetPageNoSelect(string filter, string filterArgsKey, string orderBy, int skip, int take, int countAcrossPages, params int[] guidIds)
        {
            var filterArgs = filterArgsKey == null ? null : _filterArgs[filterArgsKey];

            var query = new Query { Filter = filter, FilterParameters = filterArgs, OrderBy = orderBy, Skip = skip, Top = take };

            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _userRoles["Starbuck"],
                DbContextType.SqlServerOpenTransaction, _output);

            var page = await service
                .GetPageAsync(query);

                Assert.Equal(countAcrossPages, page.CountAcrossPages);
                XUnitUtils.AssertOrderedSysGuids(page.Data, guidIds);            

        }

        [Theory]
        [InlineData("ReleaseDate > \"1970-01-01\"", "Title", 2, 3, 11, /*guidIds:*/ 6, 7, 5)]
        [InlineData("Title.Contains(\"o\")", "ReleaseDate desc", 3, 4, 9, /*guidIds:*/ 11, 16, 3, 15)]
        public async Task TestGetDynamicLinqNoSelect(string where, string orderBy, int skip, int take, int countAcrossPages, params int[] guidIds)
        {
            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _userRoles["Starbuck"],
                DbContextType.SqlServerOpenTransaction, _output);

            var dlResult = await service
                .GetDynamicLinqResultAsync(null,where,orderBy,skip,take);

            Assert.Equal(countAcrossPages, dlResult.RowCount);
            XUnitUtils.AssertOrderedSysGuids(dlResult.Data, guidIds);

        }

        public class SysGuidTitle { public Guid SysGuid { get; set; } public string Title { get; set; } }
        public class SysGuidReleaseDate { public Guid SysGuid { get; set; } public DateTime ReleaseDate { get; set; } }

        private static readonly Dictionary<string, (Guid SysGuid, dynamic Prop)[]> _select = new()
        {
            {
                "new {SysGuid, Title}", new (Guid, dynamic)[] {
                    (GuidUtils.FromId(6), "Fool in the Rain"), 
                    (GuidUtils.FromId(7), "Hotel California"),
                    (GuidUtils.FromId(5), "Kashmir"),
                }
            },
            {
                "new {SysGuid, ReleaseDate}", new (Guid, dynamic)[] {
                    (GuidUtils.FromId(11), new DateTime(1975, 10, 13)),
                    (GuidUtils.FromId(16), new DateTime(1975, 5, 19)),
                    (GuidUtils.FromId(3), new DateTime(1973, 11, 2)),
                    (GuidUtils.FromId(15), new DateTime(1973, 6, 27)),
                }
            }
        };


        [Theory]
        [InlineData("new {SysGuid, Title}", "ReleaseDate > @0", "TestGetPage_Date", "Title", 2, 3, 11)]
        [InlineData("new {SysGuid, ReleaseDate}", "Title.Contains(@0)", "TestGetPage_Title", "ReleaseDate desc", 3, 4, 9)]
        public async Task TestGetPageSelect(string select, string filter, string filterArgsKey, string orderBy, int skip, int take, int countAcrossPages)
        {
            var filterArgs = filterArgsKey == null ? null : _filterArgs[filterArgsKey];
            var expected = _select[select];

            var query = new Query { Select = select, Filter = filter, FilterParameters = filterArgs, OrderBy = orderBy, Skip = skip, Top = take };

            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _userRoles["Starbuck"],
                DbContextType.SqlServerOpenTransaction, _output);

            var page = await service
                .GetPageSelectAsync(query);

            Assert.Equal(countAcrossPages, page.CountAcrossPages);

            var range = Enumerable.Range(0, Math.Min(expected.Length, page.Data.Count)).ToList();

            if (select == "new {SysGuid, Title}")
            {
                var typedResults = TypeUtils.Cast<List<object>, List<SysGuidTitle>>(page.Data);

                IEnumerable<Action<SysGuidTitle>> asserts =
                    range.Select<int,Action<SysGuidTitle>>(i =>
                    rec =>
                    {
                        Assert.Equal(expected[i].SysGuid, rec.SysGuid);
                        Assert.Equal(expected[i].Prop, rec.Title);
                    });

                Assert.Collection(typedResults, asserts.ToArray());
            }
            else if (select == "new {SysGuid, ReleaseDate}")
            {
                var typedResults = TypeUtils.Cast<List<object>, List<SysGuidReleaseDate>>(page.Data);

                IEnumerable<Action<SysGuidReleaseDate>> asserts =
                    range.Select<int, Action<SysGuidReleaseDate>>(i =>
                    rec =>
                    {
                        Assert.Equal(expected[i].SysGuid, rec.SysGuid);
                        Assert.Equal(expected[i].Prop, rec.ReleaseDate);
                    });

                Assert.Collection(typedResults, asserts.ToArray());
            }
        }

        [Theory]
        [InlineData("new {SysGuid, Title}", "ReleaseDate > \"1970-01-01\"", "Title", 2, 3, 11)]
        [InlineData("new {SysGuid, ReleaseDate}", "Title.Contains(\"o\")", "ReleaseDate desc", 3, 4, 9)]
        public async Task TestGetDynamicLinqSelect(string select, string where, string orderBy, int skip, int take, int countAcrossPages)
        {
            var expected = _select[select];

            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _userRoles["Starbuck"],
                DbContextType.SqlServerOpenTransaction, _output);

            var dlResult = await service
                .GetDynamicLinqResultAsync(select:select, null, where, orderBy, skip, take);

            Assert.Equal(countAcrossPages, dlResult.RowCount);

            var range = Enumerable.Range(0, Math.Min(expected.Length, dlResult.Data.Count)).ToList();

            if (select == "new {SysGuid, Title}")
            {
                var typedResults = TypeUtils.Cast<List<object>, List<SysGuidTitle>>(dlResult.Data);

                IEnumerable<Action<SysGuidTitle>> asserts =
                    range.Select<int, Action<SysGuidTitle>>(i =>
                    rec =>
                    {
                        Assert.Equal(expected[i].SysGuid, rec.SysGuid);
                        Assert.Equal(expected[i].Prop, rec.Title);
                    });

                Assert.Collection(typedResults, asserts.ToArray());
            }
            else if (select == "new {SysGuid, ReleaseDate}")
            {
                var typedResults = TypeUtils.Cast<List<object>, List<SysGuidReleaseDate>>(dlResult.Data);

                IEnumerable<Action<SysGuidReleaseDate>> asserts =
                    range.Select<int, Action<SysGuidReleaseDate>>(i =>
                    rec =>
                    {
                        Assert.Equal(expected[i].SysGuid, rec.SysGuid);
                        Assert.Equal(expected[i].Prop, rec.ReleaseDate);
                    });

                Assert.Collection(typedResults, asserts.ToArray());
            }
        }

    }
}