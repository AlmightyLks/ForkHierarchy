using Blazor.Diagrams.Core.Models;
using Octokit;

namespace ForkHierarchy.Models;

public class NodePort : PortModel
{
    public TreeNodeModel<Repository> Node { get; set; }

    public NodePort(TreeNodeModel<Repository> node, PortAlignment alignment = PortAlignment.Top)
        : base(node, alignment, null, null)
    {
        Node = node;
    }

    public override bool CanAttachTo(PortModel port)
    {
        // Avoid attaching to self port/node
        if (!base.CanAttachTo(port))
            return false;

        var targetPort = (NodePort)port;
        var targetRepository = targetPort.Node;

        if (Node.Parent.Item.FullName != targetRepository.Item.FullName)
            return false;
        if (Node.Item.FullName != targetRepository.Parent.Item.FullName)
            return false;

        return true;
    }
}
