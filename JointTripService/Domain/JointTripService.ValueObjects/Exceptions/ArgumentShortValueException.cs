namespace JointTripService.ValueObjects.Exceptions;

public class ArgumentShortValueException(string paramName, string value, int minLength)
    : ArgumentException($"Argument \"{paramName}\" value \"{value}\" is shorter than {minLength}", paramName);