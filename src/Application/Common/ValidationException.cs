namespace CleanArchitecture.Application.Common;

/// <summary>
/// Thrown when input or a business rule fails validation. Mapped to HTTP 400.
/// Carries a list of human-readable error messages.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(IEnumerable<string> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors.ToList();
    }

    public ValidationException(string error)
        : this(new[] { error })
    {
    }

    public IReadOnlyList<string> Errors { get; }
}
