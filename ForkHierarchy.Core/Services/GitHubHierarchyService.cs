using ForkHierarchy.Core.Models;
using Octokit;
using System.Collections.Concurrent;

namespace ForkHierarchy.Core.Services;

public class GitHubHierarchyService
{
    private readonly GitHubClient _client;

    public GitHubHierarchyService(GitHubClient client)
    {
        _client = client;
    }

    public async Task<GitHubRepository> GetRepositoryAsync(string owner, string name, bool fromSource = true)
    {
        // Get Target Repo
        // If we want all repos beginning from source, get source if it has one
        var repository = await _client.Repository.Get(owner, name);
        if (fromSource && repository.Source is not null)
            repository = repository.Source;

        var dto = new GitHubRepository(repository);
        dto.Children = await GetChildrenAsync(repository.Owner.Login, repository.Name);
        //var rootRepo = new RepositoryNodeModel(dto, null, children);
        //rootRepo.AddPort(new NodePort(rootRepo, PortAlignment.Top));
        //rootRepo.AddPort(new NodePort(rootRepo, PortAlignment.Bottom));
        return dto;
    }

    public async Task<List<GitHubRepository>> GetChildrenAsync(string owner, string name)
    {
        var result = new ConcurrentBag<GitHubRepository>();

        await Parallel.ForEachAsync(await _client.Repository.Forks.GetAll(owner, name), async (repository, ct) =>
        {
            try
            {
                var dto = new GitHubRepository(repository);

                dto.Children = await GetChildrenAsync(repository.Owner.Login, repository.Name);
                //var node = new RepositoryNodeModel(dto, null, children);
                //node.AddPort(new NodePort(node, PortAlignment.Top));
                //node.AddPort(new NodePort(node, PortAlignment.Bottom));
                result.Add(dto);
            }
            catch (NotFoundException)
            {

            }
        });

        return result.ToList();
    }

    public async Task<MiscellaneousRateLimit> GetRateLimit()
        => await _client.RateLimit.GetRateLimits();
}
