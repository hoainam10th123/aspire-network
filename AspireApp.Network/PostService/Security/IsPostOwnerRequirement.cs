using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PostService.Data;
using System.Security.Claims;

namespace PostService.Security
{
    //Microsoft.AspNetCore.Authorization
    public class IsPostOwnerRequirement : IAuthorizationRequirement { }

    public class IsPostOwnerRequirementHandler : AuthorizationHandler<IsPostOwnerRequirement>
    {
        private readonly DataContext _dbContext;
        // Microsoft.AspNetCore.Http.Abstractions
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IsPostOwnerRequirementHandler(DataContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsPostOwnerRequirement requirement)
        {
            var username = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(username)) return;

            //Microsoft.AspNetCore.Authentication.JwtBearer: RouteValues
            var postid = _httpContextAccessor.HttpContext!.Request.RouteValues
                .SingleOrDefault(x => x.Key == "id").Value!.ToString();

            var post = await _dbContext.Posts
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.UserId == username && x.Id == postid);

            if (post != null)
                context.Succeed(requirement);
        }
    }
}
