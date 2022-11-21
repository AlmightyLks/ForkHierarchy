using Blazor.Diagrams.Core.Geometry;
using ForkHierarchy.Models;
using Microsoft.AspNetCore.Components;
using Octokit;

namespace ForkHierarchy.Components;

public partial class RepositoryNode
{
    public static readonly Size Size = new Size(300, 500);

    public float Opacity
    {
        get;
        set;
    } = 1f;

    public static bool SupressRender { get; set; }

    protected override bool ShouldRender()
        => !SupressRender;

    [Parameter]
    public TreeNodeModel<Repository> Node { get; set; } = null!;

    protected override void OnInitialized()
    {
        Node.RepositoryNode = this;
    }
}
