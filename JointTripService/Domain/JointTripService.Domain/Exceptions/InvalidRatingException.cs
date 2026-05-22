namespace JointTripService.Domain.Exceptions;

public class InvalidRatingException(int rating)
    : DomainException($"Оценка {rating} недопустима")
{
    public int Rating => rating;
}