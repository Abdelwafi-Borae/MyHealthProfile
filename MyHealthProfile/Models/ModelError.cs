using Microsoft.AspNetCore.Identity;

namespace Data.ModelViews
{
    public class ModelError
    {
        public bool IsError { get; set; }
        public string? Message { get; set; }
        public IEnumerable<IdentityError>? identityErrors { get; set; }
        public Object? tokenObject { get; set; }
        public Object? Data { get; set; }
    }
}
