using Octokit;

namespace ForkHierarchy.Services;

public class HierachyBuilder
{
    private readonly GitHubClient _gitHubClient;

    public HierachyBuilder(GitHubClient githubClient)
    {
        _gitHubClient = githubClient;
    }

    public async Task Build(string owner, string name)
    {
        var repository = await _gitHubClient.Repository.Get(owner, name);
        //_gitHubClient.Repository.
    }
}
