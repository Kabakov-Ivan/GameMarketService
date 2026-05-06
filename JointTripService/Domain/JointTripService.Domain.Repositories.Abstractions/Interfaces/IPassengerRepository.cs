using JointTripService.Domain.Entities;
using JointTripService.Domain.Repositories.Abstractions.Base;

namespace JointTripService.Domain.Repositories.Abstractions.Interfaces;

public interface IPassengerRepository : IRepository<Passenger, Guid>
{
    Task<Passenger?> GetPassengerByEmailAsync(string email, CancellationToken cancellationToken);
}