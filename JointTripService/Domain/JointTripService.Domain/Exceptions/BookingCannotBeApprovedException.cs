using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class BookingCannotBeApprovedException(Booking booking)
    : DomainException($"The booking {booking.Id} cannot be approved in the current state")
{
    public Booking Booking => booking;
}