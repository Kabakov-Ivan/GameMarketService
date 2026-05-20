using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripCannotBeCancelledException(Trip trip)
    : DomainException($"The trip {trip.Id} cannot be cancelled in the current state")
{
    public Trip Trip => trip;
}