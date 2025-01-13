using System.Text.Json.Serialization;

namespace MyHealthProfile.Extensions
{
    public class GenericResult<T>
    {
        public bool Status { get; set; }

        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T Data { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ErrorMessage { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, string[]> ValidationErrors { get; set; }


        public GenericResult<T> Success(T data)
        {
            Status = true;
            Data = data;
            return this;
        }
        public GenericResult<T> Created(T data)
        {
            Status = true;
            Data = data;
            return this;
        }
        public GenericResult<T> Fail(string errorMessage)
        {
            Status = false;
            ErrorMessage = errorMessage;
            return this;
        }

        public GenericResult<T> ValidationFail(IDictionary<string, string[]> validationErrors)
        {
            Status = false;
            ErrorMessage = "Validation Error";
            ValidationErrors = validationErrors;
            return this;
        }
    }

    public static class ResultMaker
    {
        public static GenericResult<T> ToSuccessResult<T>(this T data)
        {
            return new GenericResult<T>().Success(data);
        }
        public static GenericResult<T> ToCreatedResult<T>(this T data)
        {
            return new GenericResult<T>().Created(data);
        }

        public static GenericResult<T> ToFailResult<T>(this T data, string errorMessage)
        {
            return new GenericResult<T>().Fail(errorMessage);
        }

        public static GenericResult<T> ToFailResult<T>(this T data, IDictionary<string, string[]> errors)
        {
            return new GenericResult<T>().ValidationFail(errors);
        }
    }
}
