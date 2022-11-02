using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using ForkHierarchy.Components;
using ForkHierarchy.Models;
using ForkHierarchy.Services;
using Microsoft.AspNetCore.Components;

namespace ForkHierarchy.Pages
{
    public partial class Index
    {
        public Diagram Diagram { get; set; } = null!;

        public bool Rendering { get; set; }
        public bool FromSource { get; set; } = true;
        public string? RepositoryUrl { get; set; } = "https://github.com/DukeFloppa/SynapseNMG";

        [Inject]
        public HierachyBuilder HierachyBuilder { get; set; } = null!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var options = new DiagramOptions
            {
                DefaultNodeComponent = null, // Default component for nodes
                EnableVirtualization = false,
                Zoom = new DiagramZoomOptions
                {
                    Minimum = 0.1, // Minimum zoom value
                    Maximum = 5_000, // Minimum zoom value
                    ScaleFactor = 2,
                    Inverse = true, // Whether to inverse the direction of the zoom when using the wheel
                                    // Other
                }
            };
            Diagram = new Diagram(options);
            Diagram.RegisterModelComponent<RepositoryNodeModel, RepositoryNode>();

            StateHasChanged();
        }
        public async Task RenderGraph()
        {
            if (String.IsNullOrWhiteSpace(RepositoryUrl))
                return;

            Diagram.Nodes.Clear();

            Rendering = true;

            RepositoryUrl = RepositoryUrl.Replace(".git", "");
            var vals = RepositoryUrl.Split('/').TakeLast(2).ToArray();

            var root = await HierachyBuilder.Gather(vals[0], vals[1], FromSource);
            var curRoot = new RepositoryNodeModel(root, new Point(100, 100));
            curRoot.Size = new Size(100, 100);
            RenderNode(curRoot, null);

            Rendering = false;
        }
        public void RenderNode(RepositoryNodeModel current, RepositoryNodeModel? parent, int gen = 1)
        {
            Diagram.Nodes.Add(current);

            if (parent is not null)
            {
                Diagram.Links.Add(new LinkModel(parent, current)
                {
                    TargetMarker = LinkMarker.Arrow
                });
            }

            double angle = 360.0 / current.RepositoryObject.Forks.Count * Math.PI / 180.0;

            for (int i = 0; i < current.RepositoryObject.Forks.Count; i++)
            {
                var fork = current.RepositoryObject.Forks[i];

                var x = current!.Position.X + Math.Cos(angle * i) * (100 * current.RepositoryObject.Forks.Count);
                var y = current!.Position.Y + Math.Sin(angle * i) * (100 * current.RepositoryObject.Forks.Count);
                var newCur = new RepositoryNodeModel(fork, new Point(x, y));
                newCur.Size = new Size(100, 100);
                RenderNode(newCur, current, ++gen);
            }
        }
    }
}