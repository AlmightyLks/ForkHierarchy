
#nullable disable

using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using System.Linq;

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

    public RepositoryNodeModel(RepositoryNodeModel nodeModel)
    {
        Parent = nodeModel.Parent;
        Item = nodeModel.Item;
        Opacity = nodeModel.Opacity;
        X = nodeModel.X;
        Y = nodeModel.Y;
        Mod = nodeModel.Mod;
        Children = nodeModel.Children.Select(x => new RepositoryNodeModel(x)).ToList();
        foreach (var port in nodeModel.Ports)
            AddPort(port);
        Size = nodeModel.Size;
    }

    public RepositoryNodeModel(GitHubRepository item, Size size)
    {
        this.Item = item;
        this.Parent = null;
        Children = new List<RepositoryNodeModel>();
        AddPort(new NodePort(this, PortAlignment.Top));
        AddPort(new NodePort(this, PortAlignment.Bottom));
        Opacity = 1;
        Size = size;
    }
    public RepositoryNodeModel(GitHubRepository item, RepositoryNodeModel parent, List<RepositoryNodeModel> children, Size size)
    {
        this.Item = item;
        this.Parent = parent;
        Children = children;
        AddPort(new NodePort(this, PortAlignment.Top));
        AddPort(new NodePort(this, PortAlignment.Bottom));
        Opacity = 1;
        Size = size;
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

    public bool IsRightMost()
    {
        if (this.Parent == null)
            return true;

        return Parent.Children[^1] == this;
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
        if (this.Parent == null || IsRightMost())
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
