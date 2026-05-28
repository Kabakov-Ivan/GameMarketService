using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripCannotBeBookedByDriverException(Trip trip)
    : DomainException($"Водитель поездки {trip.Id} не может бронировать свою поездку")
{
    public Trip Trip => trip;
}