
#nullable disable

using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using ForkHierarchy.Components;

namespace ForkHierarchy.Models;

public class TreeNodeModel<T> : NodeModel
    where T : class
{
    public double X { get => Position.X; set => Position = new Point(value, Position.Y); }
    public double Y { get => Position.Y; set => Position = new Point(Position.X, value); }
    public double Mod { get; set; }
    public TreeNodeModel<T> Parent { get; set; }
    public RepositoryNode RepositoryNode { get; set; }

    private List<TreeNodeModel<T>> _children;
    private Func<Task<List<TreeNodeModel<T>>>> _gatherChildrenFunc;

    public T Item { get; set; }

    public TreeNodeModel(T item, TreeNodeModel<T> parent, Func<Task<List<TreeNodeModel<T>>>> gatherChildrenFunc)
    {
        this.Item = item;
        this.Parent = parent;
        _gatherChildrenFunc = gatherChildrenFunc;
    }

    public async Task<bool> IsLeaf()
    {
        return (await this.GetChildrenAsync()).Count == 0;
    }

    public async Task<bool> IsLeftMost()
    {
        if (this.Parent == null)
            return true;

        return (await this.Parent.GetChildrenAsync())[0] == this;
    }

    public async Task<bool> IsRightMost()
    {
        if (this.Parent == null)
            return true;

        var children = await this.Parent.GetChildrenAsync();

        return children[children.Count - 1] == this;
    }

    public async Task<TreeNodeModel<T>> GetPreviousSibling()
    {
        if (this.Parent == null || await this.IsLeftMost())
            return null;

        var children = await this.Parent?.GetChildrenAsync();

        return children[children.FindIndex(x => x.Item == this.Item) - 1];
    }

    public async Task<TreeNodeModel<T>> GetNextSibling()
    {
        var children = await this.Parent.GetChildrenAsync();

        if (this.Parent == null || await this.IsRightMost())
            return null;

        return children[children.FindIndex(x => x.Item == this.Item) + 1];
    }

    public async Task<TreeNodeModel<T>> GetLeftMostSibling()
    {
        if (this.Parent == null)
            return null;

        if (await this.IsLeftMost())
            return this;

        return (await this.Parent.GetChildrenAsync())[0];
    }

    public async Task<TreeNodeModel<T>> GetLeftMostChild()
    {
        var children = await this.GetChildrenAsync();

        if (children.Count == 0)
            return null;

        return children[0];
    }

    public async Task<TreeNodeModel<T>> GetRightMostChild()
    {
        var children = await this.GetChildrenAsync();
        if (children.Count == 0)
            return null;

        return children[children.Count - 1];
    }

    public async Task<List<TreeNodeModel<T>>> GetChildrenAsync()
    {
        if (_children is null)
        {
            _children = await _gatherChildrenFunc();

            foreach (var child in _children)
                child.Parent = this;
        }

        return _children;
    }

    public override string ToString()
    {
        return Item.ToString();
    }
}
