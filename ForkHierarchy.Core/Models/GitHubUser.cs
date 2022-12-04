namespace ForkHierarchy.Core.Models;

public class GitHubUser
{
    public int Id { get; set; }

    public int GHId { get; internal set; }
    public string? Name { get; set; }
    public string Login { get; set; } = null!;
    public string? Email { get; set; }
    public AccountType Type { get; set; }
    public string? Location { get; set; }
    public string HtmlUrl { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
}
