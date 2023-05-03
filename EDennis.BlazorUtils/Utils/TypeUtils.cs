using System.Collections.Concurrent;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EDennis.BlazorUtils.Utils
{
    public static class TypeUtils
    {
        public static O Cast<I,O>(I input)
        {
            var serlialized = JsonSerializer.Serialize(input, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles });
            var deserialized = JsonSerializer.Deserialize<O>(serlialized);
            return deserialized;
        }


        private static readonly ConcurrentDictionary<string, Type> _cache = new();

        public static Type GetType(params (string PropertyName, Type PropertyType)[] defs)
        {
            var key = string.Join(',', defs.Select(def => def.PropertyName).OrderBy(x => x));

            if(!_cache.TryGetValue(key, out var type))
            {
                type = DynamicClassFactory.CreateType(
                    defs.
                     Select(def => new DynamicProperty(def.PropertyName, def.PropertyType))
                     .ToArray()
                );
                _cache[key] = type;
            }

            return type;
        }
    }
}
