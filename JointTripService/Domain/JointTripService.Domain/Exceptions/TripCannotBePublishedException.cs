using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripCannotBePublishedException(Trip trip)
    : DomainException($"Поездка {trip.Id} не может быть опубликована в текущем состоянии")
{
    public Trip Trip => trip;
}