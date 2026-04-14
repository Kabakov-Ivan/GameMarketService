using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class AnotherUserEditTripException(Trip trip, User user)
    : InvalidOperationException($"The user {user.FullName} can't edit the trip {trip.Id} owned by the user {trip.Driver.FullName}.")
{
    public Trip Trip => trip;
    public User User => user;
}