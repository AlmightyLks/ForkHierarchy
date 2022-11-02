using ForkHierarchy.Models;
using Microsoft.AspNetCore.Components;

namespace ForkHierarchy.Components;

public partial class RepositoryNode
{
    [Parameter]
    public RepositoryNodeModel Node { get; set; } = null!;
}
