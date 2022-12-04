﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Database.Models;

public class GitHubRepository
{
    public int Id { get; set; }

    public long GHId { get; set; }
    public string HtmlUrl { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Stars { get; set; }
    public bool IsFork { get; set; }
    public int ForksCount { get; set; }
    public DateTime CreatedAt { get; set; }

    public GitHubUser Owner { get; set; } = null!;
    public GitHubRepository? Parent { get; set; }

    public int? SourceId { get; set; }
    public GitHubRepository? Source { get; set; }

    public ICollection<GitHubRepository> Children { get; set; } = new List<GitHubRepository>();
}
public class GitHubRepositoryEtityTypeConfiguration : IEntityTypeConfiguration<GitHubRepository>
{
    public void Configure(EntityTypeBuilder<GitHubRepository> builder)
    {
        builder
            .HasIndex(g => g.Id)
            .IsUnique();

        builder
            .HasMany(g => g.Children)
            .WithOne(g => g.Parent);

        builder
            .HasOne(g => g.Parent)
            .WithMany(g => g.Children);

        builder
            .HasOne(g => g.Source);

        builder
            .HasOne(g => g.Owner);

        builder
            .Property(g => g.GHId)
            .IsRequired();
        builder
            .Property(g => g.FullName)
            .IsRequired();
        builder
            .Property(g => g.Name)
            .IsRequired();
        builder
            .Property(g => g.HtmlUrl)
            .IsRequired();
    }
}