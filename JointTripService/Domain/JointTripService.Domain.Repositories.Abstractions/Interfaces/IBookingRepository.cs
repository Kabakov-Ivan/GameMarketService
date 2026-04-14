using JointTripService.Domain.Entities;
using JointTripService.Domain.Repositories.Abstractions.Base;

namespace JointTripService.Domain.Repositories.Abstractions.Interfaces;

public interface IBookingRepository : IRepository<Booking, Guid>
{
    Task<IEnumerable<Booking>> GetBookingsByTripIdAsync(Guid tripId, CancellationToken cancellationToken, bool asNoTracking = false);
    Task<IEnumerable<Booking>> GetBookingsByPassengerIdAsync(Guid passengerId, CancellationToken cancellationToken, bool asNoTracking = false);
}