namespace MyHealthProfile.Services.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    int EnterpriseId { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
}
