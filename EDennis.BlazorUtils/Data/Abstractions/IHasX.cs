namespace EDennis.BlazorUtils
{
    public interface IHasSysUser
    {
        string SysUser { get; set; }
    }

    public interface IHasSysGuid
    {
        Guid SysGuid { get; set; }
    }

    public interface IHasIntegerId
    {
        int Id { get; set; }
    }
}
