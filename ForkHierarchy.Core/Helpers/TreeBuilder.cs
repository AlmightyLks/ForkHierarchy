#nullable disable

using ForkHierarchy.Core.Models;

namespace ForkHierarchy.Core.Helpers;

public class TreeBuilder<T>
        where T : class
{
    private const int nodeSize = 500;
    private const double depthDistance = 1250;
    private const double siblingDistance = 25;
    private const double treeDistance = 150;

    private Action<RepositoryNodeModel> _positionSet;

    public TreeBuilder(Action<RepositoryNodeModel> positionSet)
    {
        _positionSet = positionSet;
    }

    public void CalculateNodePositions(RepositoryNodeModel rootNode)
    {
        // initialize node x, y, and mod values
        InitializeNodes(rootNode, 0);

        // assign initial X and Mod values for nodes
        CalculateInitialX(rootNode);

        // ensure no node is being drawn off screen
        CheckAllChildrenOnScreen(rootNode);

        // assign final X values to nodes
        CalculateFinalPositions(rootNode, 0);
    }

    // recusrively initialize x, y, and mod values of nodes
    private void InitializeNodes(RepositoryNodeModel node, int depth)
    {
        //node.Size = new Size();
        node.X = -1;
        node.Y = depth * depthDistance;
        node.Mod = 0;

        foreach (var child in node.Children)
            InitializeNodes(child, depth + 1);
    }

    private void CalculateFinalPositions(RepositoryNodeModel node, double modSum)
    {
        node.X += modSum;
        modSum += node.Mod;

        var children = node.Children;

        foreach (var child in children)
            CalculateFinalPositions(child, modSum);

        _positionSet(node);

        /*
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
        */
    }

    private void CalculateInitialX(RepositoryNodeModel node)
    {
        var children = node.Children;

        foreach (var child in children)
            CalculateInitialX(child);

        // if no children
        if (node.IsLeaf())
        {
            // if there is a previous sibling in this set, set X to prevous sibling + designated distance
            if (!node.IsLeftMost())
                node.X = (node.GetPreviousSibling()).X + nodeSize + siblingDistance;
            else
                // if this is the first node in a set, set X to 0
                node.X = 0;
        }
        // if there is only one child
        else if (children.Count == 1)
        {
            // if this is the first node in a set, set it's X value equal to it's child's X value
            if (node.IsLeftMost())
            {
                node.X = children[0].X;
            }
            else
            {
                node.X = (node.GetPreviousSibling()).X + nodeSize + siblingDistance;
                node.Mod = node.X - children[0].X;
            }
        }
        else
        {
            var leftChild = node.GetLeftMostChild();
            var rightChild = node.GetRightMostChild();
            var mid = (leftChild.X + rightChild.X) / 2;

            if (node.IsLeftMost())
            {
                node.X = mid;
            }
            else
            {
                node.X = (node.GetPreviousSibling()).X + nodeSize + siblingDistance;
                node.Mod = node.X - mid;
            }
        }

        if (children.Count > 0 && !node.IsLeftMost())
        {
            // Since subtrees can overlap, check for conflicts and shift tree right if needed
            CheckForConflicts(node);
        }

    }

    private void CheckForConflicts(RepositoryNodeModel node)
    {
        var minDistance = treeDistance + nodeSize;
        var shiftValue = 0D;

        var nodeContour = new Dictionary<double, double>();
        GetLeftContour(node, 0, nodeContour);

        var sibling = node.GetLeftMostSibling();
        while (sibling != null && sibling != node)
        {
            var siblingContour = new Dictionary<double, double>();
            GetRightContour(sibling, 0, siblingContour);

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

                CenterNodesBetween(node, sibling);

                shiftValue = 0;
            }

            sibling = sibling.GetNextSibling();
        }
    }

    private void CenterNodesBetween(RepositoryNodeModel leftNode, RepositoryNodeModel rightNode)
    {
        var children = leftNode.Parent.Children;

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

            CheckForConflicts(leftNode);
        }
    }

    private void CheckAllChildrenOnScreen(RepositoryNodeModel node)
    {
        var nodeContour = new Dictionary<double, double>();
        GetLeftContour(node, 0, nodeContour);

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

    private void GetLeftContour(RepositoryNodeModel node, double modSum, Dictionary<double, double> values)
    {
        if (!values.ContainsKey(node.Y))
            values.Add(node.Y, node.X + modSum);
        else
            values[node.Y] = Math.Min(values[node.Y], node.X + modSum);

        modSum += node.Mod;
        foreach (var child in node.Children)
        {
            GetLeftContour(child, modSum, values);
        }
    }

    private void GetRightContour(RepositoryNodeModel node, double modSum, Dictionary<double, double> values)
    {
        if (!values.ContainsKey(node.Y))
            values.Add(node.Y, node.X + modSum);
        else
            values[node.Y] = Math.Max(values[node.Y], node.X + modSum);

        modSum += node.Mod;
        foreach (var child in (node.Children))
        {
            GetRightContour(child, modSum, values);
        }
    }
}
