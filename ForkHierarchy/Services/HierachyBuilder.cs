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

    public async Task<RepositoryObject> Gather(string owner, string name, bool fromSource = true)
    {
        // Get Target Repo
        // If we want all repos beginning from source, get source if it has one
        var repository = await _client.Repository.Get(owner, name);
        if (fromSource && repository.Source is not null)
            repository = repository.Source;

        var allForks = await GetAllForkRepositoriesAsync(repository.Owner.Login, repository.Name).ToListAsync();
        var rootRepo = new RepositoryObject()
        {
            Repository = repository,
            Forks = allForks
        };
        return rootRepo;
    }

    public async IAsyncEnumerable<RepositoryObject> GetAllForkRepositoriesAsync(string owner, string name)
    {
        foreach (var fork in await _client.Repository.Forks.GetAll(owner, name))
        {
            var obj = new RepositoryObject()
            {
                Repository = fork,
                Forks = await GetAllForkRepositoriesAsync(fork.Owner.Login, fork.Name).ToListAsync()
            };
            yield return obj;
        }
    }
}
