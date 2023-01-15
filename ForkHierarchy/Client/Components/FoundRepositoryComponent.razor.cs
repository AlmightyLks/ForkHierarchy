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

    public void ShowHierachy()
    {
        NavigationManager.NavigateTo($"/Display/{Node.Item.Id}");
    }
}
