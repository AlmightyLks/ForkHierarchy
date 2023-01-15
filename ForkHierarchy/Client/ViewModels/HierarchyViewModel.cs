﻿using Blazor.Diagrams.Components;
using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using ForkHierarchy.Client.Api;
using ForkHierarchy.Client.Components;
using ForkHierarchy.Core.Helpers;
using ForkHierarchy.Core.Models;
using ForkHierarchy.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Octokit;
using System;
using static MudBlazor.CategoryTypes;

public class HierarchyViewModel
{
    public bool ResetPosition { get; set; } = true;
    public Diagram Diagram { get; set; } = null!;
    public bool Rendering { get; set; }

    public Filter Filter { get; } = new Filter();

    public Action? StateHasChanged { get; set; }

    private RepositoryNodeModel? _originalRootNode;
    private List<int> _whitelistedNodeIds;

    private TreeBuilder<RepositoryNodeModel> _treeBuilder;

    private ForkHierarchyApiClient _apiClient;

    public HierarchyViewModel(ForkHierarchyApiClient apiClient)
    {
        _treeBuilder = new TreeBuilder<RepositoryNodeModel>(RenderNode);
        _whitelistedNodeIds = new List<int>();
        _apiClient = apiClient;
    }

    public async Task InitializeAsync(int id)
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
        Diagram.MouseClick += Diagram_MouseClick;

        await PrepareData(id);
    }

    public void Render()
    {
        if (_originalRootNode is null)
            return;

        Diagram.Links.Clear();
        Diagram.Nodes.Clear();

        _whitelistedNodeIds.Clear();
        WhitelistFilteredNodes(_originalRootNode);

        _treeBuilder.CalculateNodePositions(_originalRootNode);

        if (ResetPosition)
            Diagram.CenterOnNode(_originalRootNode, 1_500);

        Console.WriteLine(_originalRootNode.Id);

        StateHasChanged?.Invoke();
    }

    private bool WhitelistFilteredNodes(RepositoryNodeModel node)
    {
        bool hasWhitelistedChild = false;
        foreach (var child in node.Children)
        {
            if (WhitelistFilteredNodes(child))
            {
                hasWhitelistedChild = true;
            }
        }

        if (Filter.Applies(node) || hasWhitelistedChild)
        {
            _whitelistedNodeIds.Add(node.Item.Id);
            return true;
        }
        return false;
    }

    private void RenderNode(RepositoryNodeModel current)
    {
        if (!_whitelistedNodeIds.Contains(current.Item.Id))
            return;

        if (Diagram.Nodes.Contains(current))
            return;

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
    }

    private async Task PrepareData(int id)
    {
        var rootRepo = await _apiClient.GitHubRepository.GetGitHubRepositoryByIdAsync(id);
        if (rootRepo is null)
            return;

        _originalRootNode = new RepositoryNodeModel(rootRepo, RepositoryNode.Size);

        PrepareChildren(_originalRootNode);

        /*
        // Child Nodes
        var allNodes = rootRepo.Children
            .Select(child => new RepositoryNodeModel(child!, RepositoryNode.Size))
            .ToList();
        // Root Node
        _originalRootNode = new RepositoryNodeModel(rootRepo!, RepositoryNode.Size);
        allNodes.Add(_originalRootNode);

        return ConnectNodes(allNodes);
        */
    }
    public void PrepareChildren(RepositoryNodeModel node)
    {
        foreach (var child in node.Item.Children)
        {
            var childNodeModel = new RepositoryNodeModel(child, RepositoryNode.Size);
            childNodeModel.Parent = node;

            node.Children.Add(childNodeModel);
            PrepareChildren(childNodeModel);
        }
    }

    private void Diagram_MouseClick(Model model, MouseEventArgs arg2)
    {
        if (arg2.Button == 0 && model is NodeModel nodeModel)
        {
            Diagram.CenterOnNode(nodeModel, 1000);
        }
    }
}

public class Filter
{
    public string? OwnerName { get; set; }
    public string? TextSearch { get; set; }
    public int? MinStars { get; set; }
    public DateTime? LastCommitAfter { get; set; }

    public bool Applies(RepositoryNodeModel node)
    {
        return HasNoFilter()
            || TextSearchApplies()
            || OwnerNameApplies()
            || MinStarsApplies()
            || LastCommitAfterApplies();

        bool LastCommitAfterApplies()
            => LastCommitAfter is not null
            && (node.Item.LastCommit >= LastCommitAfter);

        bool MinStarsApplies()
            => MinStars is not null
            && (node.Item.Stars >= MinStars);

        bool OwnerNameApplies()
            => !String.IsNullOrWhiteSpace(OwnerName)
            && (node.Item.Owner.Login.Contains(OwnerName, StringComparison.OrdinalIgnoreCase));

        bool TextSearchApplies()
            => !String.IsNullOrWhiteSpace(TextSearch)
            && (node.Item.FullName.Contains(TextSearch, StringComparison.OrdinalIgnoreCase)
            || node.Item.Description.Contains(TextSearch, StringComparison.OrdinalIgnoreCase));

        // If no filter is mentioned, give it a free-pass
        bool HasNoFilter()
            => LastCommitAfter is null
            && MinStars is null
            && String.IsNullOrWhiteSpace(OwnerName)
            && String.IsNullOrWhiteSpace(TextSearch);
    }
}
