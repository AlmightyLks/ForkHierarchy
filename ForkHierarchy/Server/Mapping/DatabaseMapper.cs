using ForkHierarchy.Core.Models;

namespace ForkHierarchy.Server.Mapping;

public static class DatabaseMapper
{
    public static Database.Models.GitHubRepository? ToDbo(this GitHubRepository? dto)
        => dto == null ? null : new Database.Models.GitHubRepository()
        {
            Id = dto.Id,
            GHId = dto.GHId,
            Description = dto.Description,
            Stars = dto.Stars,
            IsFork = dto.IsFork,
            ForksCount = dto.ForksCount,
            Name = dto.Name,
            FullName = dto.FullName,
            HtmlUrl = dto.HtmlUrl,
            Owner = dto.Owner.ToDbo()!,
            ParentId = dto.ParentId,
            SourceId = dto.SourceId,
            LastCommit = dto.LastCommit,
            CreatedAt = dto.CreatedAt
        };
    public static GitHubRepository? ToDto(this Database.Models.GitHubRepository? dto)
        => dto == null ? null : new GitHubRepository()
        {
            Id = dto.Id,
            GHId = dto.GHId,
            Description = dto.Description,
            Stars = dto.Stars,
            IsFork = dto.IsFork,
            ForksCount = dto.ForksCount,
            Name = dto.Name,
            FullName = dto.FullName,
            HtmlUrl = dto.HtmlUrl,
            Owner = dto.Owner.ToDto()!,
            ParentId = dto.ParentId,
            SourceId = dto.SourceId,
            LastCommit = dto.LastCommit,
            CreatedAt = dto.CreatedAt
        };

    public static Database.Models.GitHubUser? ToDbo(this GitHubUser? dto)
        => dto == null ? null : new Database.Models.GitHubUser()
        {
            Id = dto.Id,
            Name = dto.Name,
            Login = dto.Login,
            HtmlUrl = dto.HtmlUrl,
            AvatarUrl = dto.AvatarUrl,
            Email = dto.Email,
            Location = dto.Location,
            Type = (Database.Models.AccountType)dto.Type
        };
    public static GitHubUser? ToDto(this Database.Models.GitHubUser? dto)
        => dto == null ? null : new GitHubUser()
        {
            Id = dto.Id,
            Name = dto.Name,
            Login = dto.Login,
            HtmlUrl = dto.HtmlUrl,
            AvatarUrl = dto.AvatarUrl,
            Email = dto.Email,
            Location = dto.Location,
            Type = (AccountType)dto.Type
        };

    public static Database.Models.QueuedRepository? ToDbo(this QueuedRepository? dto)
        => dto == null ? null : new Database.Models.QueuedRepository()
        {
            Id = dto.Id,
            Name = dto.Name,
            Owner = dto.Owner,
            AddedAt = dto.AddedAt
        };
    public static QueuedRepository? ToDto(this Database.Models.QueuedRepository? dto)
        => dto == null ? null : new QueuedRepository()
        {
            Id = dto.Id,
            Name = dto.Name,
            Owner = dto.Owner,
            AddedAt = dto.AddedAt
        };
}
