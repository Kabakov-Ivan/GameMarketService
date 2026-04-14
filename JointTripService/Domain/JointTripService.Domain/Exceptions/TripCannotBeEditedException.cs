using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripCannotBeEditedException(Trip trip)
    : DomainException($"The trip {trip.Id} cannot be edited in the current state")
{
    public Trip Trip => trip;
}