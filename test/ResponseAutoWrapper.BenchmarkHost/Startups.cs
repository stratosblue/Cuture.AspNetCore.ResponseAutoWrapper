using AutoWrapper;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ResponseAutoWrapper.BenchmarkHost
{
    public class AutoWrapperCoreStartup : BaseStartup
    {
        #region Public 方法

        public override void Configure(IApplicationBuilder app)
        {
            app.UseAutoWrapper();
            base.Configure(app);
        }

        #endregion Public 方法
    }

    public abstract class BaseStartup
    {
        #region Public 方法

        public virtual void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        #endregion Public 方法
    }

    public class NoWrapperStartup : BaseStartup
    {
    }

    public class ResponseAutoWrapperStartup : BaseStartup
    {
        #region Public 方法

        public override void Configure(IApplicationBuilder app)
        {
            app.UseResponseAutoWrapper();

            base.Configure(app);
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddResponseAutoWrapper();
        }

        #endregion Public 方法
    }
}