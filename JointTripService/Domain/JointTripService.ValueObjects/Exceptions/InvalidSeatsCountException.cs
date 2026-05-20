namespace JointTripService.ValueObjects.Exceptions;

public class InvalidSeatsCountException(string paramName)
    : ArgumentOutOfRangeException(paramName, "Количество мест должно быть больше нуля");