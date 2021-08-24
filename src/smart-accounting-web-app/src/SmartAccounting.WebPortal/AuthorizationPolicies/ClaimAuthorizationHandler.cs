using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartAccounting.WebPortal.AuthorizationPolicies
{
    public class ClaimAuthorizationHandler : AuthorizationHandler<ClaimRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimRequirement requirement)
        {
            var roleClaims = context
                                .User
                                .Claims
                                .Where(c => c.Type == ClaimTypes.Role);

            if (roleClaims != null)
            {
                foreach (var roleClaim in roleClaims)
                {
                    if (roleClaim != null)
                    {
                        if (roleClaim.Value.Equals(requirement.ClaimName,
                                                   StringComparison.InvariantCultureIgnoreCase))
                        {
                            context.Succeed(requirement);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
