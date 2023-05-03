
using Xunit.Abstractions;

namespace EDennis.BlazorUtils.Tests
{
    public class AppRolesCrudServiceTestFixture :
        CrudServiceTestFixture<AppUserRolesContext, AppRoleService<AppUserRolesContext>, AppRole>
    {
    }
}
