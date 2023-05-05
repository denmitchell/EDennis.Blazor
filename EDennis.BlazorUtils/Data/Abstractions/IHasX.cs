using System.ComponentModel.DataAnnotations;

namespace EDennis.BlazorUtils
{
    /// <summary>
    /// Entities that implement this interface have
    /// a property that holds the name of the user 
    /// who created or last saved the record.
    /// </summary>
    public interface IHasSysUser
    {
        string SysUser { get; set; }
    }

    /// <summary>
    /// Entities that implement this interface have
    /// a GUID property
    /// </summary>
    public interface IHasSysGuid
    {
        Guid SysGuid { get; set; }
    }

    /// <summary>
    /// Entities that implement this interface have an
    /// integer Id property.
    /// </summary>
    public interface IHasIntegerId
    {
        int Id { get; set; }
    }
}
