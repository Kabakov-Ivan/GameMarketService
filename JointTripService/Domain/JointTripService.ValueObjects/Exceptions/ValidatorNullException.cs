namespace JointTripService.ValueObjects.Exceptions;

public class ValidatorNullException(string paramName)
    : ArgumentNullException(paramName, $"Валидатор \"{paramName}\" не должен быть null");