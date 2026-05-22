using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class BookingCannotBeApprovedException(Booking booking)
    : DomainException($"Бронирование {booking.Id} не может быть подтверждено в текущем состоянии")
{
    public Booking Booking => booking;
}