namespace JointTripService.ValueObjects.Exceptions;

public class ArgumentNullOrWhiteSpaceException(string paramName)
    : ArgumentException($"Значение аргумента \"{paramName}\" не должно быть null, пустым или состоять только из пробелов", paramName);