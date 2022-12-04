using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class GitHubUser
{
    public int Id { get; set; }

    public int GHId { get; set; }
    public string? Name { get; set; }
    public string Login { get; set; } = null!;
    public string? Email { get; set; }
    public AccountType Type { get; set; }
    public string? Location { get; set; }
    public string HtmlUrl { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
}

public class GitHubUserEtityTypeConfiguration : IEntityTypeConfiguration<GitHubUser>
{
    public void Configure(EntityTypeBuilder<GitHubUser> builder)
    {
        builder
            .HasIndex(g => g.Id)
            .IsUnique();

        builder
            .Property(g => g.Login)
            .IsRequired();
        builder
            .Property(g => g.AvatarUrl)
            .IsRequired();
        builder
            .Property(g => g.HtmlUrl)
            .IsRequired();
    }
}