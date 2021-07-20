using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using SmartAccounting.Notification.API.Infrastructure.AuthorizationPolicies;
using System.Threading.Tasks;

namespace SmartAccounting.Notification.API.Core.DependencyInjection
{
    internal static class AuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationWithAuthorizationSupport(this IServiceCollection services,
                                                                           IConfiguration config)
        {
            services.AddMicrosoftIdentityWebApiAuthentication(config, "AzureAdB2CConfig");

            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/direct-notification")))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AccessAsUser",
                        policy => policy.Requirements.Add(new ScopesRequirement("User.Access")));
            });

            services.AddSingleton<IAuthorizationHandler, ScopesHandler>();

            services.AddHttpContextAccessor();

            return services;
        }
    }
}
