using JointTripService.ValueObjects.Base;
using JointTripService.ValueObjects.Exceptions;

namespace JointTripService.ValueObjects.Validators;

public class FullNameValidator : IValidator<string>
{
    public static int MAX_LENGTH => 80;
    public static int MIN_LENGTH => 3;

    public void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullOrWhiteSpaceException(nameof(value));

        value = value.Trim();

        if (value.Length > MAX_LENGTH)
            throw new ArgumentLongValueException(nameof(value), value, MAX_LENGTH);

        if (value.Length < MIN_LENGTH)
            throw new ArgumentShortValueException(nameof(value), value, MIN_LENGTH);
    }
}