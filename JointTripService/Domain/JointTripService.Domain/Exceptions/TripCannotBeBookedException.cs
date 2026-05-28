using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripCannotBeBookedException(Trip trip)
    : DomainException($"Поездка {trip.Id} не может быть забронирована в текущем состоянии")
{
    public Trip Trip => trip;
}