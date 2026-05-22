using JointTripService.Domain.Entities;

namespace JointTripService.Domain.Exceptions;

public class DriverCannotReviewHimselfException(Driver driver)
    : DomainException($"Водитель {driver.FullName} не может оставить отзыв самому себе")
{
    public Driver Driver => driver;
}