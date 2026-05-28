using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class TripCannotBeCompletedBeforeDepartureException(Trip trip, Driver driver)
    : DomainException($"Поездку {trip.Id} не может завершить водитель {driver.FullName} до времени отправления")
{
    public Trip Trip => trip;
    public Driver Driver => driver;
}