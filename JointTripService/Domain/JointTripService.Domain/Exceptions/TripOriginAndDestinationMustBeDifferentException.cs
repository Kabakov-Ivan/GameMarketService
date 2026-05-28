using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripOriginAndDestinationMustBeDifferentException(Trip trip)
    : DomainException($"У поездки {trip.Id} пункт отправления и пункт назначения должны отличаться")
{
    public Trip Trip => trip;
}