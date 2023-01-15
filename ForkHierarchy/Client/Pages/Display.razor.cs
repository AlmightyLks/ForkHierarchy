using Blazor.Diagrams.Core;
using Microsoft.AspNetCore.Components;

namespace ForkHierarchy.Client.Pages;

public partial class Display
{
    [Parameter]
    public string? Id { get; set; }

    [Inject]
    public ILogger<Display> Logger { get; set; } = null!;
    
    [Inject]
    public HierarchyViewModel ViewModel { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        ViewModel.StateHasChanged = StateHasChanged;

        if (int.TryParse(Id, out int id))
        {
            await ViewModel.InitializeAsync(id, Logger);
        }

        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await Task.Delay(250);

            ViewModel.Render();
            StateHasChanged();
        }
    }
}
