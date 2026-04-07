using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AiMeetingSummariser.Api.Middleware;

public class JwtAuthFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Token is missing" });
            return;
        }
        
        var token = authHeader.Substring("Bearer ".Length);
        
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            var claims = jwtToken.Claims.ToList();
            
            var claimsIdentity = new ClaimsIdentity(claims, "jwt");
            
            context.HttpContext.User = new ClaimsPrincipal(claimsIdentity);
        }
        catch
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Invalid token" });
        }
    }
    
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
