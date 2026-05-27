using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class ReviewIsNotForThisParticipantException(Review review, string participantName)
    : DomainException($"Отзыв {review.Id} не предназначен для участника {participantName}")
{
    public Review Review => review;
    public string ParticipantName => participantName;
}