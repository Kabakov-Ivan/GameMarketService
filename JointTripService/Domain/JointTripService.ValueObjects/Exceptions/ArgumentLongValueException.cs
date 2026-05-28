namespace JointTripService.ValueObjects.Exceptions;

public class ArgumentLongValueException(string paramName, string value, int maxLength)
    : ArgumentException($"Значение аргумента \"{paramName}\" со значением \"{value}\" длиннее {maxLength}", paramName);