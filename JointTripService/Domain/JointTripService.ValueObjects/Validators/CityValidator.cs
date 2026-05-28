using JointTripService.ValueObjects.Base;
using JointTripService.ValueObjects.Exceptions;

namespace JointTripService.ValueObjects.Validators;

public class CityValidator : IValidator<string>
{
    public static int MAX_LENGTH => 60;
    public static int MIN_LENGTH => 2;

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