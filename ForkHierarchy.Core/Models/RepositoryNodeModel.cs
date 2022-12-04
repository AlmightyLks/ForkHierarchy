
#nullable disable

using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;

namespace ForkHierarchy.Core.Models;

public class RepositoryNodeModel : NodeModel
{
    public RepositoryNodeModel Parent { get; set; }

    public GitHubRepository Item { get; set; }
    public float Opacity { get; set; }

    public List<RepositoryNodeModel> Children { get; }

    public double X { get => Position.X; set => Position = new Point(value, Position.Y); }
    public double Y { get => Position.Y; set => Position = new Point(Position.X, value); }
    public double Mod { get; set; }

    public RepositoryNodeModel(GitHubRepository item)
    {
        this.Item = item;
        this.Parent = null;
        Children = new List<RepositoryNodeModel>();
    }
    public RepositoryNodeModel(GitHubRepository item, RepositoryNodeModel parent, List<RepositoryNodeModel> children)
    {
        this.Item = item;
        this.Parent = parent;
        Children = children;
    }

    public bool IsLeaf()
    {
        return Children.Count == 0;
    }

    public bool IsLeftMost()
    {
        if (this.Parent == null)
            return true;

        return Parent.Children[0] == this;
    }

    public bool IsRightMostAsync()
    {
        if (this.Parent == null)
            return true;

        return Children[^1] == this;
    }

    public RepositoryNodeModel GetPreviousSibling()
    {
        if (this.Parent == null || IsLeftMost())
            return null;

        var children = Parent?.Children;

        return children[children.FindIndex(x => x.Item == this.Item) - 1];
    }

    public RepositoryNodeModel GetNextSibling()
    {
        if (this.Parent == null || IsRightMostAsync())
            return null;

        var children = Parent.Children;

        return children[children.FindIndex(x => x.Item == this.Item) + 1];
    }

    public RepositoryNodeModel GetLeftMostSibling()
    {
        if (this.Parent == null)
            return null;

        if (IsLeftMost())
            return this;

        return Parent.Children[0];
    }

    public RepositoryNodeModel GetLeftMostChild()
    {
        if (Children.Count == 0)
            return null;

        return Children[0];
    }

    public RepositoryNodeModel GetRightMostChild()
    {
        if (Children.Count == 0)
            return null;

        return Children[^1];
    }

    public override string ToString()
    {
        return Item.ToString();
    }
}
