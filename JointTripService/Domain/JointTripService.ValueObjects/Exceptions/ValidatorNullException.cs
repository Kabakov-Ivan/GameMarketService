namespace JointTripService.ValueObjects.Exceptions;

public class ValidatorNullException(string paramName)
    : ArgumentNullException(paramName, $"Validator \"{paramName}\" is null");