using JointTripService.Domain.Entities;
using JointTripService.Domain.Repositories.Abstractions.Base;

namespace JointTripService.Domain.Repositories.Abstractions.Interfaces;

public interface IReviewRepository : IRepository<Review, Guid>
{
    Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId, CancellationToken cancellationToken, bool asNoTracking = false);
    Task<IEnumerable<Review>> GetReviewsByTripIdAsync(Guid tripId, CancellationToken cancellationToken, bool asNoTracking = false);
}