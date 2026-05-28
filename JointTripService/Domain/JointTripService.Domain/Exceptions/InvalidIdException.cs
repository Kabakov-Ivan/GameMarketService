namespace JointTripService.Domain.Exceptions;

public class InvalidIdException()
    : DomainException("Идентификатор имеет некорректное значение")
{
}