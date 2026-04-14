using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripCannotBeBookedByDriverException(Trip trip)
    : DomainException($"The driver of the trip {trip.Id} cannot book his own trip")
{
    public Trip Trip => trip;
}