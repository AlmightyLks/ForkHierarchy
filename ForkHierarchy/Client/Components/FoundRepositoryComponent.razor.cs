using ForkHierarchy.Core.Models;
using Microsoft.AspNetCore.Components;

#nullable disable

namespace ForkHierarchy.Client.Components;

public partial class FoundRepositoryComponent
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public RepositoryNodeModel Node { get; set; }

    //[Parameter]
    //public State RepoState { get; set; }

    public DateTime CalculateETA()
    {
        /*
        if (DatabaseQueue is null)
            return default;

        var queueItemsBeforeThis = DbContext.QueuedRepositories
            .Count(x => x.AddedAt <= DatabaseQueue.AddedAt);

        var normalizedQueueItems = Math.Max(queueItemsBeforeThis, 1);

        var result = DatabaseQueue.AddedAt.AddMinutes(normalizedQueueItems * 5);
        return result;
        */
        return default;
    }

    public void ShowHierachy()
    {
        NavigationManager.NavigateTo($"/Display/{Node.Item.Id}");
    }
}

/*
public enum State
{
    Unknown,
    Queued,
    Known
}
*/