namespace CRM.Shared.Results
{
    public sealed class ValidationResult : Result, IValidationResult
    {
        private ValidationResult(Error[] errors)
            : base(false, IValidationResult.ValidationError)
        {
            Errors = errors;
        }

        public Error[] Errors { get; }

        public static ValidationResult WithErrors(Error[] errors) => new(errors);
    }

    public interface IValidationResult
    {
        public static readonly Error ValidationError = new(
            "ValidationError",
            "A validation problem occurred.");

        Error[] Errors { get; }
    }
}
