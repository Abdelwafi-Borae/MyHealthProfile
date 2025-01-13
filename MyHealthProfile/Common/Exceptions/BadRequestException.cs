using MyHealthProfile.Common.Extensions;

public class BadRequestException : Exception
{
    public BadRequestException(string message)
        : base(message) { }
}
