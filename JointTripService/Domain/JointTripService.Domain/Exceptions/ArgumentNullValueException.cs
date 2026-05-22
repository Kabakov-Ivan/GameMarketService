namespace JointTripService.Domain.Exceptions;

public class ArgumentNullValueException(string paramName)
    : ArgumentNullException(paramName, $"Значение аргумента \"{paramName}\" не должно быть null");