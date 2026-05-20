namespace JointTripService.ValueObjects.Exceptions;

public class ArgumentNullOrWhiteSpaceException(string paramName)
    : ArgumentException($"Argument \"{paramName}\" is null, empty or whitespace", paramName);