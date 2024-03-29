﻿using Blazor.Diagrams.Core.Geometry;
using ForkHierarchy.Core.Models;
using Microsoft.AspNetCore.Components;

namespace ForkHierarchy.Client.Components;

public partial class RepositoryNode
{
    public static readonly Size Size = new Size(300, 300);

    public static bool SupressRender { get; set; }

    protected override bool ShouldRender()
        => !SupressRender;

    [Parameter]
    public RepositoryNodeModel Node { get; set; } = null!;

    [Parameter]
    public string Class { get; set; } = "";
    [Parameter]
    public string Style { get; set; } = "";

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {

        }
    }
}
