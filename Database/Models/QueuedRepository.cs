using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Database.Models;

public class QueuedRepository
{
    public int Id { get; set; }
    public string Owner { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime AddedAt { get; set; }
}

public class QueuedRepositoryEtityTypeConfiguration : IEntityTypeConfiguration<QueuedRepository>
{
    public void Configure(EntityTypeBuilder<QueuedRepository> builder)
    {
        builder
            .Property(g => g.Owner)
            .IsRequired();
        builder
            .Property(g => g.Name)
            .IsRequired();
        builder
            .Property(g => g.AddedAt)
            .IsRequired();
    }
}