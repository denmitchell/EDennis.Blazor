using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EDennis.BlazorUtils
{
    public static class OptionsMonitor
    {

        public static IOptionsMonitor<T> GetOptionsFromConfig<T>(this IConfiguration config, string key)
            where T: class
            => New<T>(config.GetSection(key).Get<T>());

        public static OptionsMonitor<T> New<T>(T options)
            where T : class
            => new(options);


    }

    public class OptionsMonitor<TOptions> : IOptionsMonitor<TOptions>
        where TOptions : class
    {

        
        public OptionsMonitor(TOptions options)
        {
            CurrentValue = options;
        }

        public TOptions CurrentValue { get; set; }

        public TOptions Get(string name)
        {
            throw new NotImplementedException();
        }

        public IDisposable OnChange(Action<TOptions, string> listener)
        {
            throw new NotImplementedException();
        }
    }
}
