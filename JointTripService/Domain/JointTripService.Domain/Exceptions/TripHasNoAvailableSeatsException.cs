using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripHasNoAvailableSeatsException(Trip trip)
    : DomainException($"The trip {trip.Id} does not have enough available seats")
{
    public Trip Trip => trip;
}