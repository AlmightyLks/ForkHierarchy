using Database;
using ForkHierarchy.Core.Models;
using ForkHierarchy.Core.Options;
using ForkHierarchy.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Octokit;
using Quartz;
using Serilog;
using ForkHierarchy.Server.Mapping;

namespace ForkHierarchy.Core.Jobs;

public class ProcessQueuedRepositoriesJob : IJob
{
    private static readonly Serilog.ILogger Logger = Log.ForContext<ProcessQueuedRepositoriesJob>();

    private readonly ForkHierarchyContext _dbContext;
    private readonly GitHubHierarchyService _gitHubHierarchyService;
    private readonly IOptions<GitHubOptions> _gitHubOptions;
    private readonly GitHubClient _gitHubClient;

    public ProcessQueuedRepositoriesJob(
        ForkHierarchyContext dbContext,
        GitHubHierarchyService gitHubHierarchyService,
        IOptions<GitHubOptions> gitHubOptions,
        GitHubClient gitHubClient)
    {
        _dbContext = dbContext;
        _gitHubHierarchyService = gitHubHierarchyService;
        _gitHubOptions = gitHubOptions;
        _gitHubClient = gitHubClient;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Logger.Information($"Starting Queue Process Job...");

        var queuedRepos = await _dbContext.QueuedRepositories
            .OrderBy(x => x.AddedAt)
            .Take(5)
            .ToListAsync();

        foreach (var repo in queuedRepos)
        {
            try
            {
                GitHubRepository curRepo = await _gitHubHierarchyService.GetRepositoryAsync(repo.Owner, repo.Name);

                if (curRepo.ForksCount > _gitHubOptions.Value.MaxForks)
                {
                    Logger.Warning($"{repo.Owner}/{repo.Name} exceeds max fork count. {curRepo.ForksCount}/{_gitHubOptions.Value.MaxForks}");
                    continue;
                }

                var dbRepo = await _dbContext.GitHubRepositories
                    .Include(x => x.Owner)
                    .FirstOrDefaultAsync(x => x.GHId == curRepo.GHId);

                if (dbRepo is not null)
                {
                    var children = await _dbContext.GitHubRepositories
                        .Where(x => x.SourceId == dbRepo.Id)
                        .ToListAsync();

                    foreach (var child in children)
                    {
                        _dbContext.GitHubRepositories.Remove(child);
                    }

                    _dbContext.GitHubRepositories.Remove(dbRepo);

                    await _dbContext.SaveChangesAsync();
                }

                await AddRepoAndChildrenAsync(curRepo);
            }
            catch (NotFoundException)
            {
                Logger.Warning($"{repo.Owner}/{repo.Name} doesn't exist anymore");
            }
            finally
            {
                _dbContext.QueuedRepositories.Remove(repo);
            }

            await _dbContext.SaveChangesAsync();
        }

        var rateLimits = await _gitHubClient.RateLimit.GetRateLimits();

        Logger.Information($"Finished Queue Process Job!");
        Logger.Information($"Current rate limits\n" +
            $"\tSearch: {rateLimits.Resources.Search.Remaining}/{rateLimits.Resources.Search.Limit}\n" +
            $"\tCore: {rateLimits.Resources.Core.Remaining}/{rateLimits.Resources.Core.Limit}");
    }

    private async Task<Database.Models.GitHubRepository> AddRepoAndChildrenAsync(GitHubRepository repository, Database.Models.GitHubRepository? source = null)
    {
        var dbRepo = repository.ToDbo()!;
        dbRepo.SourceId = source?.Id;
        dbRepo.Owner = repository.Owner.ToDbo()!;

        if (source is null)
            source = dbRepo;

        var dboOwner = await _dbContext.GitHubUsers.FirstOrDefaultAsync(x => x.Id == dbRepo.Owner.Id);

        if (dboOwner is null)
        {
            _dbContext.GitHubUsers.Add(dbRepo.Owner);
            //await _dbContext.SaveChangesAsync();
        }
        else
        {
            dbRepo.Owner = dboOwner;
        }

        _dbContext.GitHubRepositories.Add(dbRepo);

        // Save source repo to db, to give it an id
        // So that we can give the children our source and parent repo id
        await _dbContext.SaveChangesAsync();

        foreach (var child in repository.Children)
        {
            var childDbRepo = await AddRepoAndChildrenAsync(child, source);
            childDbRepo.ParentId = dbRepo.Id;
        }

        return dbRepo;
    }
}
