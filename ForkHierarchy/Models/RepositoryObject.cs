using Octokit;

namespace ForkHierarchy.Models;

public class RepositoryObject
{
    public Repository? Repository { get; set; }
    public List<RepositoryObject>? Forks { get; set; }
}
