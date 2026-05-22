using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class AnotherDriverEditTripException(Trip trip, Driver driver)
    : DomainException($"Водитель {driver.FullName} не может редактировать поездку {trip.Origin} -> {trip.Destination}")
{
    public Trip Trip => trip;
    public Driver Driver => driver;
}