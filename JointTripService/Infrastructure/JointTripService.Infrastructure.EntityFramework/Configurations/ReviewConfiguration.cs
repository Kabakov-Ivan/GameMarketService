using JointTripService.Domain.Entities;
using JointTripService.ValueObjects;
using JointTripService.ValueObjects.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JointTripService.Infrastructure.EntityFramework.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Rating).IsRequired();
        builder.Property(x => x.Text)
            .IsRequired()
            .HasConversion(text => text.Value, value => new ReviewText(value))
            .HasMaxLength(ReviewTextValidator.MAX_LENGTH);
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
        builder.HasOne(x => x.Author)
            .WithMany("_reviewsWritten")
            .HasForeignKey("AuthorId")
            .HasPrincipalKey(x => x.Id);
        builder.HasOne(x => x.TargetUser)
            .WithMany("_reviewsReceived")
            .HasForeignKey("TargetUserId")
            .HasPrincipalKey(x => x.Id);
        builder.HasOne(x => x.Trip)
            .WithMany()
            .HasForeignKey("TripId")
            .HasPrincipalKey(x => x.Id);
    }
}
