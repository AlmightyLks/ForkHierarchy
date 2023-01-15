using Octokit;

namespace ForkHierarchy.Core.Models;

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
    public long CommitCount { get; set; }
    public DateTime LastCommit { get; set; }
    public DateTime CreatedAt { get; set; }

    public GitHubUser Owner { get; set; } = null!;
    public int? ParentId { get; set; }
    public int? SourceId { get; set; }
    public List<GitHubRepository> Children { get; set; }

    public GitHubRepository()
    {
        Children = new List<GitHubRepository>();
    }
    public GitHubRepository(Repository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        GHId = repository.Id;
        Name = repository.Name;
        Description = repository.Description;
        ForksCount = repository.ForksCount;
        FullName = repository.FullName;
        HtmlUrl = repository.HtmlUrl;
        IsFork = repository.Fork;
        Stars = repository.StargazersCount;
        Owner = new GitHubUser()
        {
            HtmlUrl = repository.Owner.HtmlUrl,
            AvatarUrl = repository.Owner.AvatarUrl,
            Email = repository.Owner.Email,
            Location = repository.Owner.Location,
            Id = repository.Owner.Id,
            Login = repository.Owner.Login,
            Name = repository.Owner.Login,
            Type = (ForkHierarchy.Core.Models.AccountType)(repository.Owner.Type ?? default)
        };
        Children = new List<GitHubRepository>();
        LastCommit = repository.PushedAt?.UtcDateTime ?? default;
        CreatedAt = DateTime.UtcNow;
    }
}
