using JointTripService.Domain.Entities;
using JointTripService.Domain.Repositories.Abstractions.Base;

namespace JointTripService.Domain.Repositories.Abstractions.Interfaces;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
}