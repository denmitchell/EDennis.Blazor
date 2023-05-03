using Xunit;

namespace EDennis.BlazorUtils.Utils
{
    public static class XUnitUtils
    {
        public static void AssertOrderedIds<T>(IEnumerable<T> recs, params int[] ids)
            where T : class, IHasIntegerId
        {
            Action<T>[] asserts = ids.ToArray()
                .Select(i => {
                    return (Action<T>)(actual => Assert.Equal(i, actual.Id));
                }).ToArray();

            Assert.Collection(recs, asserts);
        }

        public static void AssertOrderedSysGuids<T>(IEnumerable<T> recs, params int[] guidInts)
            where T : class, IHasSysGuid
        {
            Action<T>[] asserts = guidInts.ToArray()
                .Select(i => {
                    return (Action<T>)(actual => Assert.Equal(GuidUtils.FromId(i), actual.SysGuid));
                }).ToArray();

            Assert.Collection(recs, asserts);
        }
    }
}
