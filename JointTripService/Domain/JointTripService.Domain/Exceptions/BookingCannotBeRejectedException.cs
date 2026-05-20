using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class BookingCannotBeRejectedException(Booking booking)
    : DomainException($"The booking {booking.Id} cannot be rejected in the current state")
{
    public Booking Booking => booking;
}