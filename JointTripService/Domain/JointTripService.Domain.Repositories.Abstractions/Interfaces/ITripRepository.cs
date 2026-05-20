using JointTripService.Domain.Entities;
using JointTripService.Domain.Repositories.Abstractions.Base;
using JointTripService.ValueObjects;

namespace JointTripService.Domain.Repositories.Abstractions.Interfaces;

public interface ITripRepository : IRepository<Trip, Guid>
{
    Task<IEnumerable<Trip>> GetTripsByRouteAsync(City origin, City destination, CancellationToken cancellationToken, bool asNoTracking = false);
    Task<IEnumerable<Trip>> GetPublishedTripsAsync(CancellationToken cancellationToken, bool asNoTracking = false);
}