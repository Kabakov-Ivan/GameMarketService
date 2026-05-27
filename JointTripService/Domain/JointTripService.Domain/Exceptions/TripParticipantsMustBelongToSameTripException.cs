using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripParticipantsMustBelongToSameTripException(Trip trip)
    : DomainException($"У поездки {trip.Id} водитель и пассажир должны относиться к одной поездке")
{
    public Trip Trip => trip;
}