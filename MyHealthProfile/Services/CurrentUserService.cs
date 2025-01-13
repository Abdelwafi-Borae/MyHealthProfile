using Microsoft.AspNetCore.Http;
using MyHealthProfile.Services.Interfaces;
using System.Security.Claims;



namespace MyHealthProfile.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(UserId);

    public string Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public int EnterpriseId
    {
        get
        {
            return int.Parse(_httpContextAccessor.HttpContext.Request?.Headers["EnterpriseId"].FirstOrDefault() ?? "0");
        }
    }
}
