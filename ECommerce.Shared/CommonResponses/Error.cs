namespace ECommerce.Shared.CommonResponses
{
    public class Error
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public ErrorType ErrorType { get; set; }
        private Error(string code, string description, ErrorType errortype)
        {
            Code = code;
            Description = description;
            ErrorType = errortype;
        }

        public static Error Faliure(string code = "General.Faliure",
            string description = "A general failure has occurred.")
        {
            return new Error(code, description, ErrorType.Failure);
        }
        public static Error Validation(string code = "General.Validation",
            string description = "Validation Error has Occured")
        {
            return new Error(code, description, ErrorType.Validation);
        }
        public static Error NotFound(string code = "General.NotFound",
            string description = "The requested resource was not found.")
        {
            return new Error(code, description, ErrorType.NotFound);
        }
        public static Error Unauthorized(string code = "General.Unauthorized",
            string description = "Your are not Autherized to perform this Action.")
        {
            return new Error(code, description, ErrorType.Unauthorized);
        }
        public static Error Forbidden(
            string code = "General.Forbidden",
            string description = "Your don't have access to this Source, Access denied.")
        {
            return new Error(code, description, ErrorType.Forbidden);
        }
        public static Error InvalidCredentials(
            string code = "General.InvalidCradentials",
            string description = "Your credentials are invalid to reach this Source.")
        {
            return new Error(code, description, ErrorType.InvalidCredentials);
        }
    }

}
