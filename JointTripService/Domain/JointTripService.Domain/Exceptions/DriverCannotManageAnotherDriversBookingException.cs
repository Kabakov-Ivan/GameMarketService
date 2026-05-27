using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class DriverCannotManageAnotherDriversBookingException(Driver driver, Booking booking)
    : DomainException($"Водитель {driver.FullName} не может управлять бронированием {booking.Id} для чужой поездки")
{
    public Driver Driver => driver;
    public Booking Booking => booking;
}