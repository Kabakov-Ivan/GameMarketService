using JointTripService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JointTripService.Infrastructure.EntityFramework.Configurations;

public class PassengerConfiguration : IEntityTypeConfiguration<Passenger>
{
    public void Configure(EntityTypeBuilder<Passenger> builder)
    {
        builder.HasBaseType<User>();
        builder.Ignore(x => x.Bookings);
    }
}
