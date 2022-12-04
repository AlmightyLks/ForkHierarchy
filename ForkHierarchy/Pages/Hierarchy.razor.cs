using Microsoft.AspNetCore.Components;

namespace ForkHierarchy.Pages;

public partial class Hierarchy
{
    [Inject]
    public HierarchyViewModel ViewModel { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ViewModel.Initialize();
        StateHasChanged();
    }
}
