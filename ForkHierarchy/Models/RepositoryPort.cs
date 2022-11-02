using Blazor.Diagrams.Core.Models;

namespace ForkHierarchy.Models;

public class RepositoryPort : PortModel
{
    public RepositoryObject RepositoryObject { get; set; }

    public RepositoryPort(NodeModel parent, RepositoryObject repositoryObject, PortAlignment alignment = PortAlignment.Bottom)
        : base(parent, alignment, null, null)
    {
        RepositoryObject = repositoryObject;
    }

    public override bool CanAttachTo(PortModel port)
    {
        // Avoid attaching to self port/node
        if (!base.CanAttachTo(port))
            return false;

        var targetPort = (RepositoryPort)port;
        var targetRepository = targetPort.RepositoryObject;

        if (RepositoryObject.Repository.Parent.FullName != targetRepository.Repository.FullName)
            return false;
        if (RepositoryObject.Repository.FullName != targetRepository.Repository.Parent.FullName)
            return false;

        return true;
    }
}
