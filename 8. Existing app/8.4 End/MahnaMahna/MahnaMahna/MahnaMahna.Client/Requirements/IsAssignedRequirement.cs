using MahnaMahna.Client.Models;
using Microsoft.AspNetCore.Authorization;

namespace MahnaMahna.Client.Requirements;

public class IsAssignedToUserRequirement : IAuthorizationRequirement
{
}

public class IsAssignedToUserRequirementAuthorizationHandler : AuthorizationHandler<IsAssignedToUserRequirement, TodoItem>
{
    protected override Task HandleRequirementAsync(
    AuthorizationHandlerContext context,
    IsAssignedToUserRequirement requirement,
    TodoItem resource)
    {
        var userId = context.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        if (resource.AssignedTo == userId || string.IsNullOrEmpty(resource.AssignedTo) )
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}