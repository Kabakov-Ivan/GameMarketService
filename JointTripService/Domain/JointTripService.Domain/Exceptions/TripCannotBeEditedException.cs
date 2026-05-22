using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripCannotBeEditedException(Trip trip)
    : DomainException($"Поездка {trip.Id} не может быть изменена в текущем состоянии")
{
    public Trip Trip => trip;
}