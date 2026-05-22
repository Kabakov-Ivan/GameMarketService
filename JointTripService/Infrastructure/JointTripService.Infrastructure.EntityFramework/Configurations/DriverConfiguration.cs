using JointTripService.Domain.Entities;
using JointTripService.ValueObjects;
using JointTripService.ValueObjects.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JointTripService.Infrastructure.EntityFramework.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.FullName)
            .IsRequired()
            .HasConversion(fullName => fullName.Value, value => new FullName(value))
            .HasMaxLength(FullNameValidator.MAX_LENGTH);
        builder.Property(x => x.Email)
            .IsRequired()
            .HasConversion(email => email.Value, value => new Email(value))
            .HasMaxLength(EmailValidator.MAX_LENGTH);
        builder.Property(x => x.CreationData).IsRequired().HasConversion
        (
            src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
            dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
        );
        builder.Property(x => x.ModificationData).IsRequired(false).HasConversion
        (
            src => !src.HasValue ? src : src.Value.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src.Value, DateTimeKind.Utc),
            dst => !dst.HasValue ? dst : dst.Value.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst.Value, DateTimeKind.Utc)
        );
        builder.Ignore(x => x.Trips);
        builder.HasMany<Trip>("_trips")
            .WithOne(x => x.Driver)
            .HasForeignKey("DriverId")
            .HasPrincipalKey(x => x.Id);
        builder.Ignore(x => x.ReviewsWritten);
        builder.Ignore(x => x.ReviewsReceived);
    }
}