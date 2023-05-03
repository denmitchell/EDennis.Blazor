using EDennis.BlazorUtils.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace EDennis.BlazorUtils.Tests
{
    public class CrudServiceTests_Role : IClassFixture<CrudServiceTestFixture<AppUserRolesContext,
        AppRoleService<AppUserRolesContext>, AppRole>>
    {
        private readonly CrudServiceTestFixture<AppUserRolesContext, AppRoleService<AppUserRolesContext>, AppRole> _fixture;
        public const string _appsettingsFile = "appsettings.Test.json";
        public readonly static Dictionary<string, string> _userRoles = new() {
            { "Starbuck", "IT" },
            { "Maria", "admin" },
            { "Darius", "user" },
            { "Huan", "readonly" },
            { "Jack", "disabled" },
        };

        private readonly ITestOutputHelper _output;


        public CrudServiceTests_Role(CrudServiceTestFixture<AppUserRolesContext,
        AppRoleService<AppUserRolesContext>, AppRole> fixture, ITestOutputHelper output) { 
            _fixture = fixture;
            _output = output;
        }




        [Fact]
        public async Task TestGetAll()
        {
            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _userRoles["Starbuck"], 
                DbContextType.SqlServerOpenTransaction, _output);

            var recs = await service
                .GetQueryable()
                .Where(r => r.Id < 0) //only test roles, having negative ids
                .OrderBy(r => r.SysGuid)
                .ToListAsync();

            XUnitUtils.AssertOrderedSysGuids(recs, 1, 2, 3, 4, 5);

        }


        [Theory]
        [InlineData(1, "IT")]
        [InlineData(3, "user")]
        [InlineData(5, "disabled")]
        public async Task TestFind(int guidId, string roleName)
        {
            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _userRoles["Starbuck"],
                DbContextType.SqlServerOpenTransaction, _output);

            var guid = GuidUtils.FromId(guidId);

            var qryRec = await service
                .GetQueryable()
                .FirstOrDefaultAsync(r => r.SysGuid == guid);

            Assert.NotNull(qryRec);

            var findRec = await service
                .FindRequiredAsync(qryRec.Id);

            Assert.Equal(roleName, findRec.RoleName);
            
        }


        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public async Task TestDelete(int guidId)
        {
            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _userRoles["Starbuck"],
                DbContextType.SqlServerOpenTransaction, _output);

            //expected remaining Guids after delete
            int[] remaining = Enumerable.Range(1, 5).Except(new int[] { guidId }).ToArray();

            var guid = GuidUtils.FromId(guidId);

            var qryRec = await service
                .GetQueryable()
                .FirstOrDefaultAsync(r => r.SysGuid == guid);

            Assert.NotNull(qryRec);

            await service
                .DeleteAsync(qryRec.Id);


            var recs = await service
                .GetQueryable()
                .Where(r => r.Id < 0) //only test roles, having negative ids
                .OrderBy(r => r.SysGuid)
                .ToListAsync();

            XUnitUtils.AssertOrderedSysGuids(recs, remaining);

        }


        [Theory]
        [InlineData(1, "super user")]
        [InlineData(3, "normal user")]
        public async Task TestUpdate(int guidId, string roleName)
        {
            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _userRoles["Starbuck"],
                DbContextType.SqlServerOpenTransaction, _output);

            var guid = GuidUtils.FromId(guidId);

            var qryRec = await service
                .GetQueryable()
                .FirstOrDefaultAsync(r => r.SysGuid == guid);

            Assert.NotNull(qryRec);

            qryRec.RoleName = roleName;

            var maxSysStart = service.GetMaxSysStart();

            await service
                .UpdateAsync(qryRec, qryRec.Id);

            var recs = service
                .GetModified(maxSysStart)
                .OrderBy(r => r.SysGuid)
                .ToList();

            Assert.Collection<AppRole>(recs,
                actual =>
                {
                    Assert.Equal(roleName, actual.RoleName);
                }
            );

        }

        [Theory]
        [InlineData(998, "super user")]
        [InlineData(999, "normal user")]
        public async Task TestCreate(int guidId, string roleName)
        {
            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _userRoles["Starbuck"],
                DbContextType.SqlServerOpenTransaction, _output);

            var guid = GuidUtils.FromId(guidId);

            var AppRole = new AppRole
            {
                RoleName = roleName,
                SysGuid = guid
            };

            var maxSysStart = service.GetMaxSysStart();

            await service
                .CreateAsync(AppRole);

            var recs = service
                .GetModified(maxSysStart)
                .OrderBy(r => r.SysGuid)
                .ToList();

            Assert.Collection<AppRole>(recs,
                actual =>
                {
                    Assert.Equal(roleName, actual.RoleName);
                    Assert.Equal(guid, actual.SysGuid);
                }
            );

        }

    }
}