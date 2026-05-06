using JointTripService.Domain.Entities;
using JointTripService.Domain.Repositories.Abstractions.Base;

namespace JointTripService.Domain.Repositories.Abstractions.Interfaces;

public interface IDriverRepository : IRepository<Driver, Guid>
{
    Task<Driver?> GetDriverByEmailAsync(string email, CancellationToken cancellationToken);
}