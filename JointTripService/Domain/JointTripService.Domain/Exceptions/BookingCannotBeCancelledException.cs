namespace JointTripService.Domain.Exceptions;

public class BookingCannotBeCancelledException(string message)
    : DomainException(message)
{
}