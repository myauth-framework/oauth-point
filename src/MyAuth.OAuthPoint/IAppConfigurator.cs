using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyAuth.OAuthPoint
{
    public interface IAppConfigurator
    {
        void Configure(IServiceCollection services, IConfiguration configuration);
    }
}