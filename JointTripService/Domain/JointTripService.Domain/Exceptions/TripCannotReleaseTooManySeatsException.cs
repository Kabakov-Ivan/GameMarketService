using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripCannotReleaseTooManySeatsException(Trip trip)
    : DomainException($"У поездки {trip.Id} нельзя освободить больше мест, чем всего доступно")
{
    public Trip Trip => trip;
}