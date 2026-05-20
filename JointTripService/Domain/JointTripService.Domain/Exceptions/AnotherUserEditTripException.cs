using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class AnotherUserEditTripException(Trip trip, Driver driver)
    : InvalidOperationException($"The driver {driver.FullName} can't edit the trip {trip.Id} owned by the driver {trip.Driver.FullName}.")
{
    public Trip Trip => trip;
    public Driver Driver => driver;
}