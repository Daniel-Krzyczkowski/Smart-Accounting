using Microsoft.AspNetCore.Authorization;

namespace SmartAccounting.FileProcessor.API.Infrastructure.AuthorizationPolicies
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
