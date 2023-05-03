using System.Collections.Concurrent;

namespace EDennis.BlazorUtils
{
    public class RolesCache: ConcurrentDictionary<string, (DateTime ExpiresAt, string Role)>
    {
    }
}
