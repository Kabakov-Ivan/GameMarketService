using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripCannotBeCancelledException(Trip trip)
    : DomainException($"Поездка {trip.Id} не может быть отменена в текущем состоянии")
{
    public Trip Trip => trip;
}