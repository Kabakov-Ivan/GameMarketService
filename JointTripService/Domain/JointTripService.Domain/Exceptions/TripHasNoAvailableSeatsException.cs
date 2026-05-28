using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripHasNoAvailableSeatsException(Trip trip)
    : DomainException($"У поездки {trip.Id} недостаточно свободных мест")
{
    public Trip Trip => trip;
}