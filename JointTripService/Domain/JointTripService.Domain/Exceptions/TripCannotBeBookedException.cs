using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripCannotBeBookedException(Trip trip)
    : DomainException($"The trip {trip.Id} cannot be booked in the current state")
{
    public Trip Trip => trip;
}