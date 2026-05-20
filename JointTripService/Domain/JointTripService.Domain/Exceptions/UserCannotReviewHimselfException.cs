using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class UserCannotReviewHimselfException(User user)
    : DomainException($"The user {user.FullName} cannot leave a review to himself")
{
    public User User => user;
}