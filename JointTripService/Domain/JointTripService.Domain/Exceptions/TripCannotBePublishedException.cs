using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripCannotBePublishedException(Trip trip)
    : DomainException($"The trip {trip.Id} cannot be published in the current state")
{
    public Trip Trip => trip;
}