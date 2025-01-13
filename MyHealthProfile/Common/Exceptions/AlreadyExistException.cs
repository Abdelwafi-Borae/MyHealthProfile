


using MyHealthProfile.Common.Extensions;

namespace MyHealthProfile.Common.Exceptions
{
    public class AlreadyExistException : Exception
    {
        public AlreadyExistException(string message)
         : base(message)
        {
        }
        public AlreadyExistException(string key, string message)
         : base("One or more validation failures have occurred.")
        {
            var result = new Dictionary<string, string[]>();
            result.Add(key.ToCamelCase(), new string[] { message });

            Errors = result;
        }
        public IDictionary<string, string[]> Errors { get; }
    }
}
