using ForkHierarchy.Core.Models;
using Refit;

namespace ForkHierarchy.Client.Api;

public interface IQueuedRepositories
{
    [Get("/api/QueuedRepositories/{fullName}")]
    Task<QueuedRepository> GetQueuedRepositoryAsync(string fullName);
    [Post("/api/QueuedRepositories/{owner}/{repo}")]
    Task CreateQueuedRepositoryAsync(string owner, string repo);
}
