﻿using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using ForkHierarchy.Components;
using ForkHierarchy.Core.Models;
using ForkHierarchy.Core.Services;

public class HierarchyViewModel
{
    public GitHubHierarchyService HierarchyBuilder { get; set; } = null!;

    public Diagram Diagram { get; set; } = null!;
    public bool Rendering { get; set; }
    public bool FromSource { get; set; } = true;

    //public string RepositoryUrl { get; set; } = "https://github.com/jaywalnut310/vits/";
    public string RepositoryUrl { get; set; } = "https://github.com/SynapseSL/Synapse/";

    private RepositoryNodeModel? _rootNode;

    private string? _lastRepository;

    public HierarchyViewModel(GitHubHierarchyService gitHubHierarchyService)
    {
        HierarchyBuilder = gitHubHierarchyService;
    }

    public void Initialize()
    {
        var options = new DiagramOptions
        {
            DefaultNodeComponent = null, // Default component for nodes
            EnableVirtualization = false,
            Zoom = new DiagramZoomOptions
            {
                Minimum = 0.1, // Minimum zoom value
                Maximum = 2_500, // Maximum zoom value
                ScaleFactor = 1.45,
                Inverse = true, // Whether to inverse the direction of the zoom when using the wheel
                                // Other
            }
        };
        Diagram = new Diagram(options);
        Diagram.RegisterModelComponent<RepositoryNodeModel, RepositoryNode>();
        Diagram.SelectionChanged += Diagram_SelectionChanged;
        //Diagram.MouseClick += Diagram_MouseClick;
    }

    private void Diagram_MouseClick(Model model, Microsoft.AspNetCore.Components.Web.MouseEventArgs arg2)
    {
        if (model is null)
        {
            foreach (var dNode in Diagram.Nodes)
            {
                if (dNode is RepositoryNodeModel dNodeModel)
                {
                    dNodeModel.Opacity = 1f;
                }
            }
        }
        else
        {
            var nodeModel = model as RepositoryNodeModel;

            foreach (var dNode in Diagram.Nodes)
            {
                if (dNode is RepositoryNodeModel dNodeModel)
                {
                    dNodeModel.Opacity = 0.25f;
                }
            }

            while (nodeModel is not null)
            {
                nodeModel.Opacity = 1f;
                nodeModel = nodeModel.Parent;
            }
        }
    }

    private void Diagram_SelectionChanged(SelectableModel obj)
    {
        /*
        if (obj.Selected)
        {
            foreach (var dNode in Diagram.Nodes)
            {
                if (dNode is RepositoryNodeModel nodeModel)
                {
                    nodeModel.RepositoryNode.Opacity = 0.25f;
                }
            }

            if (obj.Selected && obj is RepositoryNodeModel node)
            {
                while (node is not null)
                {
                    node.RepositoryNode.Opacity = 1f;
                    node = node.Parent;
                }
            }
        }
        else
        {
            foreach (var dNode in Diagram.Nodes)
            {
                if (dNode is RepositoryNodeModel nodeModel)
                {
                    nodeModel.RepositoryNode.Opacity = 1f;
                }
            }
        }
        */
    }

    public async Task UserRender()
    {
        if (String.IsNullOrWhiteSpace(RepositoryUrl))
            return;
        RepositoryUrl = RepositoryUrl.Replace(".git", "").Trim('/');
        var vals = RepositoryUrl!.Split('/').TakeLast(2).ToArray();
        var foo = await HierarchyBuilder.GetRepositoryAsync(vals[0], vals[1], FromSource);

        Console.WriteLine();

        /*
        Diagram.SuspendRefresh = true;
        Rendering = true;
        RepositoryNode.SupressRender = true;
        if (!String.IsNullOrWhiteSpace(RepositoryUrl) && RepositoryUrl != _lastRepository)
        {
            var vals = RepositoryUrl!.Split('/').TakeLast(2).ToArray();

            _rootNode = await HierarchyBuilder.GetRepositoryAsync(vals[0], vals[1], FromSource);
            //_rootNode.Position = new Point(Diagram.Pan.X, Diagram.Pan.Y);
            _rootNode.Size = RepositoryNode.Size;
            _lastRepository = RepositoryUrl;
        }

        Diagram.Links.Clear();
        Diagram.Nodes.Clear();

        await TreeHelpers<Repository>.CalculateNodePositions(_rootNode, RenderNodeAsync);

        //await RenderNodeAsync(_rootNode, null);

        Diagram.CenterOnNode(_rootNode!, 1_500);

        //Diagram.SetZoom(1);

        Rendering = false;
        RepositoryNode.SupressRender = true;
        Diagram.SuspendRefresh = false;
        */
    }

    public void RenderNodeAsync(RepositoryNodeModel current)
    {
        Diagram.Nodes.Add(current);

        if (current.Parent is not null)
        {
            var link = new LinkModel(current.Parent, current)
            {
                TargetMarker = LinkMarker.Arrow
            };
            link.SetSourcePort(current.Parent.GetPort(PortAlignment.Bottom));
            link.SetTargetPort(current.GetPort(PortAlignment.Top));
            Diagram.Links.Add(link);
        }

        /*
        foreach (var child in await current.GetChildrenAsync())
        {
            child.Size = RepositoryNode.Size;
            await RenderNodeAsync(child, current);
        }
        */
    }
}