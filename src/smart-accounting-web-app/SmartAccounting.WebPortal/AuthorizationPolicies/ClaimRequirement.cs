using Microsoft.AspNetCore.Authorization;

namespace SmartAccounting.WebPortal.AuthorizationPolicies
{
    public class ClaimRequirement : IAuthorizationRequirement
    {
        public string ClaimName { get; set; }

        public ClaimRequirement(string claimName)
        {
            ClaimName = claimName;
        }
    }
}
