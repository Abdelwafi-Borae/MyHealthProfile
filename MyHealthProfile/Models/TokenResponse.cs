namespace Data.ModelViews;
public record TokenResponse(string Token, DateTime? TokenExpiryTime, string UserId = "");
