namespace JointTripService.ValueObjects.Exceptions;

public class ArgumentShortValueException(string paramName, string value, int minLength)
    : ArgumentException($"Значение аргумента \"{paramName}\" со значением \"{value}\" короче {minLength}", paramName);