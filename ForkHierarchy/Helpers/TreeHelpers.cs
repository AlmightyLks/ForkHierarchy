using Blazor.Diagrams.Core.Geometry;
using ForkHierarchy.Models;

#nullable disable

namespace ForkHierarchy.Helpers;

public static class TreeHelpers<T>
        where T : class
{
    private const int nodeSize = 300;
    private const double depthDistance = 500;
    private const double siblingDistance = 300;
    private const double treeDistance = 500;

    public static async Task CalculateNodePositions(TreeNodeModel<T> rootNode)
    {
        // initialize node x, y, and mod values
        await InitializeNodes(rootNode, 0);

        // assign initial X and Mod values for nodes
        await CalculateInitialX(rootNode);

        // ensure no node is being drawn off screen
        await CheckAllChildrenOnScreen(rootNode);

        // assign final X values to nodes
        await CalculateFinalPositions(rootNode, 0);
    }

    // recusrively initialize x, y, and mod values of nodes
    private static async Task InitializeNodes(TreeNodeModel<T> node, int depth)
    {
        node.Size = new Size();
        node.X = -1;
        node.Y = depth * depthDistance;
        node.Mod = 0;

        foreach (var child in await node.GetChildrenAsync())
            await InitializeNodes(child, depth + 1);
    }

    private static async Task CalculateFinalPositions(TreeNodeModel<T> node, double modSum)
    {
        node.X += modSum;
        modSum += node.Mod;

        var children = await node.GetChildrenAsync();

        foreach (var child in children)
            await CalculateFinalPositions(child, modSum);

        if (children.Count == 0)
        {
            node.Size.Width = node.X;
            node.Size.Height = node.Y;
        }
        else
        {
            node.Size.Width = children.Max(p => p.Size.Width);
            node.Size.Height = children.Max(p => p.Size.Height);
        }
    }

    private static async Task CalculateInitialX(TreeNodeModel<T> node)
    {
        var children = await node.GetChildrenAsync();

        foreach (var child in children)
            await CalculateInitialX(child);

        // if no children
        if (await node.IsLeaf())
        {
            // if there is a previous sibling in this set, set X to prevous sibling + designated distance
            if (!await node.IsLeftMost())
                node.X = (await node.GetPreviousSibling()).X + nodeSize + siblingDistance;
            else
                // if this is the first node in a set, set X to 0
                node.X = 0;
        }
        // if there is only one child
        else if (children.Count == 1)
        {
            // if this is the first node in a set, set it's X value equal to it's child's X value
            if (await node.IsLeftMost())
            {
                node.X = children[0].X;
            }
            else
            {
                node.X = (await node.GetPreviousSibling()).X + nodeSize + siblingDistance;
                node.Mod = node.X - children[0].X;
            }
        }
        else
        {
            var leftChild = await node.GetLeftMostChild();
            var rightChild = await node.GetRightMostChild();
            var mid = (leftChild.X + rightChild.X) / 2;

            if (await node.IsLeftMost())
            {
                node.X = mid;
            }
            else
            {
                node.X = (await node.GetPreviousSibling()).X + nodeSize + siblingDistance;
                node.Mod = node.X - mid;
            }
        }

        if (children.Count > 0 && !await node.IsLeftMost())
        {
            // Since subtrees can overlap, check for conflicts and shift tree right if needed
            await CheckForConflicts(node);
        }

    }

    private static async Task CheckForConflicts(TreeNodeModel<T> node)
    {
        var minDistance = treeDistance + nodeSize;
        var shiftValue = 0D;

        var nodeContour = new Dictionary<double, double>();
        await GetLeftContour(node, 0, nodeContour);

        var sibling = await node.GetLeftMostSibling();
        while (sibling != null && sibling != node)
        {
            var siblingContour = new Dictionary<double, double>();
            await GetRightContour(sibling, 0, siblingContour);

            for (double level = node.Y + depthDistance; level <= Math.Min(siblingContour.Keys.Max(), nodeContour.Keys.Max()); level += depthDistance)
            {
                var distance = nodeContour[level] - siblingContour[level];
                if (distance + shiftValue < minDistance)
                {
                    shiftValue = minDistance - distance;
                }
            }

            if (shiftValue > 0)
            {
                node.X += shiftValue;
                node.Mod += shiftValue;

                await CenterNodesBetween(node, sibling);

                shiftValue = 0;
            }

            sibling = await sibling.GetNextSibling();
        }
    }

    private static async Task CenterNodesBetween(TreeNodeModel<T> leftNode, TreeNodeModel<T> rightNode)
    {
        var children = await leftNode.Parent.GetChildrenAsync();

        var leftIndex = children.IndexOf(rightNode);
        var rightIndex = children.IndexOf(leftNode);

        var numNodesBetween = (rightIndex - leftIndex) - 1;

        if (numNodesBetween > 0)
        {
            var distanceBetweenNodes = (leftNode.X - rightNode.X) / (numNodesBetween + 1);

            int count = 1;
            for (int i = leftIndex + 1; i < rightIndex; i++)
            {
                var middleNode = children[i];

                var desiredX = rightNode.X + (distanceBetweenNodes * count);
                var offset = desiredX - middleNode.X;
                middleNode.X += offset;
                middleNode.Mod += offset;

                count++;
            }

            await CheckForConflicts(leftNode);
        }
    }

    private static async Task CheckAllChildrenOnScreen(TreeNodeModel<T> node)
    {
        var nodeContour = new Dictionary<double, double>();
        await GetLeftContour(node, 0, nodeContour);

        double shiftAmount = 0;
        foreach (var y in nodeContour.Keys)
        {
            if (nodeContour[y] + shiftAmount < 0)
                shiftAmount = (nodeContour[y] * -1);
        }

        if (shiftAmount > 0)
        {
            node.X += shiftAmount;
            node.Mod += shiftAmount;
        }
    }

    private static async Task GetLeftContour(TreeNodeModel<T> node, double modSum, Dictionary<double, double> values)
    {
        if (!values.ContainsKey(node.Y))
            values.Add(node.Y, node.X + modSum);
        else
            values[node.Y] = Math.Min(values[node.Y], node.X + modSum);

        modSum += node.Mod;
        foreach (var child in await node.GetChildrenAsync())
        {
            await GetLeftContour(child, modSum, values);
        }
    }

    private static async Task GetRightContour(TreeNodeModel<T> node, double modSum, Dictionary<double, double> values)
    {
        if (!values.ContainsKey(node.Y))
            values.Add(node.Y, node.X + modSum);
        else
            values[node.Y] = Math.Max(values[node.Y], node.X + modSum);

        modSum += node.Mod;
        foreach (var child in (await node.GetChildrenAsync()))
        {
            await GetRightContour(child, modSum, values);
        }
    }
}
