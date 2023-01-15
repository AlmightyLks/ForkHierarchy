using ForkHierarchy.Core.Models;
using Refit;

namespace ForkHierarchy.Client.Api;

public interface IGitHubRepository
{
    [Get("/api/GitHubRepositories/{id}")]
    Task<GitHubRepository> GetGitHubRepositoryByIdAsync(int id);
    [Get("/api/GitHubRepositories/{owner}/{repo}")]
    Task<GitHubRepository> GetGitHubRepositoryByFullNameAsync(string owner, string repo);
}
