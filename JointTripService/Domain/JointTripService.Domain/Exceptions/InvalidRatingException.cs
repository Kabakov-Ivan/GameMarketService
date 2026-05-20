namespace JointTripService.Domain.Exceptions;

public class InvalidRatingException(int rating)
    : DomainException($"The rating {rating} is invalid")
{
    public int Rating => rating;
}