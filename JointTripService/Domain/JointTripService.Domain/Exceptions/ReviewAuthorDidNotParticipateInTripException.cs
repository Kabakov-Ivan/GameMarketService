using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class ReviewAuthorDidNotParticipateInTripException(Review review, string authorName)
    : DomainException($"Автор отзыва {authorName} не участвовал в поездке {review.Trip.Id}")
{
    public Review Review => review;
    public string AuthorName => authorName;
}