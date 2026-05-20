using JointTripService.ValueObjects.Base;
using JointTripService.ValueObjects.Exceptions;

namespace JointTripService.ValueObjects.Validators;

public class EmailValidator : IValidator<string>
{
    public static int MAX_LENGTH => 100;

    public void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullOrWhiteSpaceException(nameof(value));

        value = value.Trim();

        if (value.Length > MAX_LENGTH)
            throw new ArgumentLongValueException(nameof(value), value, MAX_LENGTH);

        if (!value.Contains('@') || value.StartsWith('@') || value.EndsWith('@'))
            throw new ArgumentException("Invalid email format", nameof(value));
    }
}