using ForkHierarchy.Models;
using Microsoft.AspNetCore.Components;
using Octokit;

namespace ForkHierarchy.Components;

public partial class RepositoryNode
{
    [Parameter]
    public TreeNodeModel<Repository> Node { get; set; } = null!;
}
