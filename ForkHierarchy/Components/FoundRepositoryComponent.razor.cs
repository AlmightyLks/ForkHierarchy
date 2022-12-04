using Database;
using ForkHierarchy.Core.Models;
using Microsoft.AspNetCore.Components;

#nullable disable

namespace ForkHierarchy.Components;

public partial class FoundRepositoryComponent
{
    [Inject]
    public ForkHierarchyContext DbContext { get; set; }

    [Parameter]
    public RepositoryNodeModel Node { get; set; }

    [Parameter]
    public Database.Models.GitHubRepository DatabaseRepository { get; set; }

    [Parameter]
    public Database.Models.QueuedRepository DatabaseQueue { get; set; }

    [Parameter]
    public State RepoState { get; set; }

    public DateTime CalculateETA()
    {
        if (DatabaseQueue is null)
            return default;

        var queueItemsBeforeThis = DbContext.QueuedRepositories
            .Count(x => x.AddedAt <= DatabaseQueue.AddedAt);

        var normalizedQueueItems = Math.Max(queueItemsBeforeThis, 1);

        var result = DatabaseQueue.AddedAt.AddMinutes(normalizedQueueItems * 5);
        return result;
    }
}

public enum State
{
    Unknown,
    Queued,
    Known
}