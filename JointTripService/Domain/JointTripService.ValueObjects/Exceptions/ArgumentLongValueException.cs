namespace JointTripService.ValueObjects.Exceptions;

public class ArgumentLongValueException(string paramName, string value, int maxLength)
    : ArgumentException($"Argument \"{paramName}\" value \"{value}\" is longer than {maxLength}", paramName);