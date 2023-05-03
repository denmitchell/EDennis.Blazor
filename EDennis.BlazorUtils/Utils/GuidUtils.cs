namespace EDennis.BlazorUtils
{
    public static class GuidUtils
    {
        public static Guid FromId(int id)
        {
            return Guid.Parse($"00000000{Math.Abs(id)}"[^8..] + "-0000-0000-0000-" + $"000000000000{Math.Abs(id)}"[^12..]);
        }

    }
}
