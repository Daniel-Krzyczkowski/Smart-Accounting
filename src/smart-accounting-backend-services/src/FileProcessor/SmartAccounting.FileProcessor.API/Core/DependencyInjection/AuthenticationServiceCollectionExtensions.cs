using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using SmartAccounting.FileProcessor.API.Infrastructure.AuthorizationPolicies;
using SmartAccounting.FileProcessor.API.Infrastructure.Identity;

namespace SmartAccounting.FileProcessor.API.Core.DependencyInjection
{
    internal static class AuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationWithAuthorizationSupport(this IServiceCollection services,
                                                                           IConfiguration config)
        {
            services.AddMicrosoftIdentityWebApiAuthentication(config, "AzureAdB2CConfig");

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AccessAsUser",
                        policy => policy.Requirements.Add(new ScopesRequirement("User.Access")));
            });

            services.AddSingleton<IAuthorizationHandler, ScopesHandler>();

            services.AddHttpContextAccessor();
            services.AddTransient<IIdentityService, IdentityService>();

            return services;
        }
    }
}
