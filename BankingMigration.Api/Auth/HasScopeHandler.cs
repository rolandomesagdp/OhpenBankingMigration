using Microsoft.AspNetCore.Authorization;

namespace BankingMigration.Api.Auth
{
    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
