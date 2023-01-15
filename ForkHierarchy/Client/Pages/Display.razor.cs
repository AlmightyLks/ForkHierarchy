using Blazor.Diagrams.Core;
using Microsoft.AspNetCore.Components;

namespace ForkHierarchy.Client.Pages;

public partial class Display
{
    [Parameter]
    public string? Id { get; set; }

    [Inject]
    public HierarchyViewModel ViewModel { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        ViewModel.StateHasChanged = StateHasChanged;

        if (int.TryParse(Id, out int id))
        {
            await ViewModel.InitializeAsync(id);
        }

        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            ViewModel.Diagram.ContainerChanged += DiagramReady;

            ViewModel.Render();
            StateHasChanged();
        }
    }

    private void DiagramReady()
    {
        ViewModel.Diagram.ContainerChanged -= DiagramReady;

        ViewModel.Render();
        StateHasChanged();
    }
}
