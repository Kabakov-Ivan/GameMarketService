using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class BookingCannotBeRejectedException(Booking booking)
    : DomainException($"Бронирование {booking.Id} не может быть отклонено в текущем состоянии")
{
    public Booking Booking => booking;
}