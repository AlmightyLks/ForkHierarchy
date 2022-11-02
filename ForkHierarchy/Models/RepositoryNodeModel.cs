using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;

namespace ForkHierarchy.Models;

public class RepositoryNodeModel : NodeModel
{
    public RepositoryObject RepositoryObject { get; set; } = null!;

    public RepositoryNodeModel(RepositoryObject repositoryObject, Point? position = null, RenderLayer layer = RenderLayer.HTML, ShapeDefiner? shape = null)
        : base(position, layer, shape)
    {
        RepositoryObject = repositoryObject;
    }
}
