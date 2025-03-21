using Microsoft.AspNetCore.Authorization;

namespace BlazorWebAppBuilInAuthentication.Client.Requirements;

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
        if (context.User.Identity?.Name == resource.AssignedTo)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;

    }
}