using Blazor.Diagrams.Core.Models;

namespace ForkHierarchy.Core.Models;

public class NodePort : PortModel
{
    public RepositoryNodeModel Node { get; set; }

    public NodePort(RepositoryNodeModel node, PortAlignment alignment = PortAlignment.Top)
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
