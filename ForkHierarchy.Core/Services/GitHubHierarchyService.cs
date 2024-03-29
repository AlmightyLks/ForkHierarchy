﻿using ForkHierarchy.Core.Models;
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
                result.Add(dto);
            }
            catch (NotFoundException)
            {
                // If not found, ignore.
                // For whatever reason, GitHub stores and provides
                // Repositories that no longer exist.
            }
        });

        return result.ToList();
    }

    public async Task<MiscellaneousRateLimit> GetRateLimit()
        => await _client.RateLimit.GetRateLimits();
}
