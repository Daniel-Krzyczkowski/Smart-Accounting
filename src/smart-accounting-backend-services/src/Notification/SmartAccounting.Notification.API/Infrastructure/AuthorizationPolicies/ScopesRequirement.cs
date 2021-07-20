using Microsoft.AspNetCore.Authorization;

namespace SmartAccounting.Notification.API.Infrastructure.AuthorizationPolicies
{
    public class ScopesRequirement : IAuthorizationRequirement
    {
        public readonly string ScopeName;

        public ScopesRequirement(string scopeName)
        {
            ScopeName = scopeName;
        }
    }
}
