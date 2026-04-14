using JointTripService.ValueObjects.Base;
using JointTripService.ValueObjects.Exceptions;

namespace JointTripService.ValueObjects.Validators;

public class ReviewTextValidator : IValidator<string>
{
    public static int MAX_LENGTH => 500;

    public void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullOrWhiteSpaceException(nameof(value));

        value = value.Trim();

        if (value.Length > MAX_LENGTH)
            throw new ArgumentLongValueException(nameof(value), value, MAX_LENGTH);
    }
}