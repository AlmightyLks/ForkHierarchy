using Refit;

namespace ForkHierarchy.Client.Api;

public class ForkHierarchyApiClient
{
    public IGitHubRepository GitHubRepository { get; set; }
    public IQueuedRepositories QueuedRepositories { get; set; }

    public ForkHierarchyApiClient(HttpClient httpClient)
    {
        GitHubRepository = RestService.For<IGitHubRepository>(httpClient);
        QueuedRepositories = RestService.For<IQueuedRepositories>(httpClient);
    }
}
