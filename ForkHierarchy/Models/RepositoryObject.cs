using Octokit;

namespace ForkHierarchy.Models;

public class RepositoryObject
{
    public Repository Repository { get; set; } = null!;
    public List<RepositoryObject> Forks { get; set; } = new List<RepositoryObject>();
}
