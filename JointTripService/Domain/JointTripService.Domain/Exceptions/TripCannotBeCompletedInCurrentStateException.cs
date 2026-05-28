using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripCannotBeCompletedInCurrentStateException(Trip trip)
    : DomainException($"Поездку {trip.Id} нельзя завершить в текущем состоянии")
{
    public Trip Trip => trip;
}