using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace HDEMS.Api.Filters;

/// <summary>
/// 权限验证过滤器
/// </summary>
public class AuthorizeFilter : ActionFilterAttribute
{
    private readonly string[] _allowedRoles;

    public AuthorizeFilter(params string[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (_allowedRoles.Length > 0)
        {
            var userRoles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!userRoles.Any(r => _allowedRoles.Contains(r)))
            {
                context.Result = new ForbidResult();
                return;
            }
        }

        base.OnActionExecuting(context);
    }
}
