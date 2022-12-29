using Blazor.Diagrams.Components;
using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using Database;
using Database.Models;
using ForkHierarchy.Components;
using ForkHierarchy.Core.Helpers;
using ForkHierarchy.Core.Mapping;
using ForkHierarchy.Core.Models;
using ForkHierarchy.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Octokit;
using System;

public class HierarchyViewModel
{
    public GitHubHierarchyService HierarchyBuilder { get; set; } = null!;
    public bool ResetPosition { get; set; } = true;
    public Diagram Diagram { get; set; } = null!;
    public bool Rendering { get; set; }

    public Filter Filter { get; } = new Filter();

    public Action? StateHasChanged { get; set; }

    private RepositoryNodeModel? _originalRootNode;
    private List<int> _whitelistedNodeIds;

    private ForkHierarchyContext _dbContext;
    private TreeBuilder<RepositoryNodeModel> _treeBuilder;

    public HierarchyViewModel(GitHubHierarchyService gitHubHierarchyService, ForkHierarchyContext dbContext)
    {
        HierarchyBuilder = gitHubHierarchyService;
        _dbContext = dbContext;
        _treeBuilder = new TreeBuilder<RepositoryNodeModel>(RenderNode);
        _whitelistedNodeIds = new List<int>();
    }

    public async Task<bool> InitializeAsync(int id)
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

        return await PrepareData(id);
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

    private async Task<bool> PrepareData(int id)
    {
        var rootRepo = await _dbContext.GitHubRepositories.Include(x => x.Owner).FirstOrDefaultAsync(x => x.Id == id);
        if (rootRepo is null)
            return false;

        // Child Nodes
        var allNodes = (await _dbContext.GitHubRepositories.Include(x => x.Owner).Where(x => x.SourceId == id).ToListAsync())
            .Select(x => new RepositoryNodeModel(x.ToDto()!, RepositoryNode.Size))
            .ToList();
        // Root Node
        _originalRootNode = new RepositoryNodeModel(rootRepo.ToDto()!, RepositoryNode.Size);
        allNodes.Add(_originalRootNode);

        return ConnectNodes(allNodes);
    }

    private bool ConnectNodes(List<RepositoryNodeModel> nodes)
    {
        var parentChildNodes = nodes.GroupBy(x => x.Item.ParentId);
        foreach (var parentChildNode in parentChildNodes)
        {
            if (parentChildNode.Key is null)
                continue;

            var parent = nodes.FirstOrDefault(x => x.Item.Id == parentChildNode.Key || (parentChildNode.Key is null && x.Item.ParentId is null));
            // Found Children without parent...
            if (parent is null)
            {
                // I guess we fail?
                return false;
            }
            foreach (var child in parentChildNode)
            {
                child.Parent = parent;
                parent.Children.Add(child);
            }
        }
        return true;
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
