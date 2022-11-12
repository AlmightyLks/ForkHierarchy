using Blazor.Diagrams.Core.Models;
using ForkHierarchy.Models;
using Octokit;

namespace ForkHierarchy.Services;

public class HierachyBuilder
{
    private readonly GitHubClient _client;

    public HierachyBuilder(GitHubClient client)
    {
        _client = client;
    }

    public async Task<TreeNodeModel<Repository>> GetRepositoryAsync(string owner, string name, bool fromSource = true)
    {
        // Get Target Repo
        // If we want all repos beginning from source, get source if it has one
        var repository = await _client.Repository.Get(owner, name);
        if (fromSource && repository.Source is not null)
            repository = repository.Source;

        var rootRepo = new TreeNodeModel<Repository>(repository, null, () => GetChildrenAsync(repository.Owner.Login, repository.Name));
        rootRepo.AddPort(new NodePort(rootRepo, PortAlignment.Top));
        rootRepo.AddPort(new NodePort(rootRepo, PortAlignment.Bottom));
        return rootRepo;
    }

    public async Task<List<TreeNodeModel<Repository>>> GetChildrenAsync(string owner, string name)
    {
        var result = new List<TreeNodeModel<Repository>>();
        foreach (var fork in await _client.Repository.Forks.GetAll(owner, name))
        {
            var node = new TreeNodeModel<Repository>(fork, null, () => GetChildrenAsync(fork.Owner.Login, fork.Name));
            node.AddPort(new NodePort(node, PortAlignment.Top));
            node.AddPort(new NodePort(node, PortAlignment.Bottom));
            result.Add(node);
        }
        return result;
    }

    public async Task<MiscellaneousRateLimit> GetRateLimit()
        => await _client.RateLimit.GetRateLimits();
}
