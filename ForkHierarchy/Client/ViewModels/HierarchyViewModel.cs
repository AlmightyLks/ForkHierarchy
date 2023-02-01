using Blazor.Diagrams;
using Blazor.Diagrams.Components;
using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using Blazor.Diagrams.Options;
using ForkHierarchy.Client.Api;
using ForkHierarchy.Client.Components;
using ForkHierarchy.Client.Pages;
using ForkHierarchy.Core.Helpers;
using ForkHierarchy.Core.Models;
using ForkHierarchy.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Octokit;
using System;
using System.Diagnostics;
using static MudBlazor.CategoryTypes;

public class HierarchyViewModel
{
    private ILogger _logger = null!;

    public bool ResetPosition { get; set; } = true;
    public BlazorDiagram Diagram { get; set; } = null!;
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

    public async Task InitializeAsync(int id, ILogger logger)
    {
        _logger = logger;
        var options = new BlazorDiagramOptions
        {
            Zoom =
            {
                Minimum = 0.1, // Minimum zoom value
                Maximum = 2_500, // Maximum zoom value
                ScaleFactor = 1.45,
                Inverse = true, // Whether to inverse the direction of the zoom when using the wheel
                                // Other
            },
            Virtualization =
            {
                Enabled = false
            }
        };
        Diagram = new BlazorDiagram(options);
        Diagram.RegisterComponent<RepositoryNodeModel, RepositoryNode>();
        Diagram.PointerClick += Diagram_PointerClick;

        await PrepareData(id);
    }

    public void Render()
    {
        if (_originalRootNode is null)
            return;

        Diagram.Links.Clear();
        Diagram.Nodes.Clear();

        //Diagram.SuspendRefresh = true;

        _whitelistedNodeIds.Clear();
        var whiteListBefore = Stopwatch.GetTimestamp();
        WhitelistFilteredNodes(_originalRootNode);
        var whiteListAfter = Stopwatch.GetTimestamp();

        _logger.LogInformation($"Whitelisting took: {Stopwatch.GetElapsedTime(whiteListBefore, whiteListAfter)}");
        StateHasChanged?.Invoke();

        var calcPositionsBefore = Stopwatch.GetTimestamp();
        _treeBuilder.CalculateNodePositions(_originalRootNode);
        var calcPositionsAfter = Stopwatch.GetTimestamp();

        _logger.LogInformation($"Calculating Positions took: {Stopwatch.GetElapsedTime(calcPositionsBefore, calcPositionsAfter)}");
        StateHasChanged?.Invoke();

        if (ResetPosition)
            Diagram.CenterOnNode(_originalRootNode, 1_500);

        //Diagram.SuspendRefresh = false;

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

            if (current.Parent.GetPort(PortAlignment.Bottom) is { } sourcePort)
                link.SetSource(new SinglePortAnchor(sourcePort));

            if (current.GetPort(PortAlignment.Top) is { } targetPort)
                link.SetTarget(new SinglePortAnchor(targetPort));

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

    private void Diagram_PointerClick(Model? model, Blazor.Diagrams.Core.Events.PointerEventArgs arg2)
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
