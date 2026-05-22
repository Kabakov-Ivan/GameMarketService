using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class PassengerCannotReviewHimselfException(Passenger passenger)
    : DomainException($"Пассажир {passenger.FullName} не может оставить отзыв самому себе")
{
    public Passenger Passenger => passenger;
}