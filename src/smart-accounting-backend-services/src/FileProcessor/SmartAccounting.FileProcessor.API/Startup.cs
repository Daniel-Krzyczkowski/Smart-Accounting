using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartAccounting.Common.ExceptionMiddleware;
using SmartAccounting.FileProcessor.API.BackgroundServices;
using SmartAccounting.FileProcessor.API.BackgroundServices.Channels;
using SmartAccounting.FileProcessor.API.Core.DependencyInjection;
using SmartAccounting.Logging;

namespace SmartAccounting.FileProcessor.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAppConfiguration(Configuration);
            services.AddLoggingServices();
            services.AddMediatR(typeof(Startup));
            services.AddAuthenticationWithAuthorizationSupport(Configuration);
            services.AddIntegrationServices();
            services.AddStorageServices();

            services.AddSingleton<FileProcessingChannel>();
            services.AddHostedService<FileProcessingBackgroundService>();

            services.AddControllers();
            services.AddSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ApiExceptionMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerServices();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
