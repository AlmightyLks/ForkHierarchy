using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using ForkHierarchy.Components;
using ForkHierarchy.Helpers;
using ForkHierarchy.Models;
using ForkHierarchy.Services;
using Microsoft.AspNetCore.Components;
using Octokit;

#nullable disable

namespace ForkHierarchy.Pages
{
    public partial class Index
    {
        [Inject]
        public HierachyBuilder HierachyBuilder { get; set; } = null!;

        public Diagram Diagram { get; set; } = null!;
        public bool Rendering { get; set; }
        public bool FromSource { get; set; } = true;
        public string RepositoryUrl { get; set; } = "https://github.com/jaywalnut310/vits/";
        //public string RepositoryUrl { get; set; } = "https://github.com/SynapseSL/Synapse/";

        private TreeNodeModel<Repository> _rootNode;

        private string _lastRepository;

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
                    ScaleFactor = 1.45,
                    Inverse = true, // Whether to inverse the direction of the zoom when using the wheel
                                    // Other
                }
            };
            Diagram = new Diagram(options);
            Diagram.RegisterModelComponent<TreeNodeModel<Repository>, RepositoryNode>();

            StateHasChanged();
        }

        public async Task UserRender()
        {
            if (String.IsNullOrWhiteSpace(RepositoryUrl))
                return;
            RepositoryUrl = RepositoryUrl.Replace(".git", "").Trim('/');

            Rendering = true;
            if (!String.IsNullOrWhiteSpace(RepositoryUrl) && RepositoryUrl != _lastRepository)
            {
                var vals = RepositoryUrl!.Split('/').TakeLast(2).ToArray();

                _rootNode = await HierachyBuilder.GetRepositoryAsync(vals[0], vals[1], FromSource);
                _rootNode.Position = new Point(Diagram.Pan.X, Diagram.Pan.Y);
                _rootNode.Size = new Size(100, 100);
                _lastRepository = RepositoryUrl;
            }

            Diagram.Nodes.Clear();

            await TreeHelpers<Repository>.CalculateNodePositions(_rootNode);

            await RenderNodeAsync(_rootNode, null);

            Rendering = false;
        }

        /// <summary>
        /// Renders a Node-Circle based around parents of forks
        /// </summary>
        /// <param name="current">Root Node</param>
        /// <param name="parent">Root Node's parent node</param>
        public async Task RenderNodeAsync(TreeNodeModel<Repository> current, TreeNodeModel<Repository> parent)
        {
            Diagram.Nodes.Add(current);

            if (parent is not null)
            {
                var link = new LinkModel(parent, current)
                {
                    TargetMarker = LinkMarker.Arrow
                };
                link.SetSourcePort(parent.GetPort(PortAlignment.Bottom));
                link.SetTargetPort(current.GetPort(PortAlignment.Top));
                Diagram.Links.Add(link);
            }

            foreach (var child in await current.GetChildrenAsync())
            {
                child.Size = new Size(100, 100);
                await RenderNodeAsync(child, current);
            }
        }

        // TODO:
        // For an actual hierachy I need to write an algorithm for the positions.
        // For whatever reason, I cannot find any implementations or hints for it online

        //public void RenderNodes()
        //{
        //    RepositoryNodeGenerations.Clear();

        //    /*
        //    for (int i = 0; i < RepositoryNodeGenerations.Count; i++)
        //    {
        //        var generation = RepositoryNodeGenerations[i];
        //        for (int j = 0; j < generation.Count; j++)
        //        {
        //            var node = generation[i];

        //            node.SetPosition();

        //            Diagram.Nodes.Add(node);

        //            if (node.Parent is not null)
        //            {
        //                Diagram.Links.Add(new LinkModel(node.Parent, node)
        //                {
        //                    TargetMarker = LinkMarker.Arrow
        //                });
        //            }


        //        }
        //    }
        //    */

        //    /*
        //    const int NodeWidth = 300;
        //    const int Spacer = 300;

        //    var maxElementsGen = RepositoryNodeGenerations.MaxBy(x => x.Count)!;
        //    var genIndex = RepositoryNodeGenerations.IndexOf(maxElementsGen);

        //    //var maxRowSize = (maxElementsFromGen * NodeWidth) + (maxElementsFromGen - 1 * Spacer);

        //    var root = RepositoryNodeGenerations[0][0];
        //    foreach (var generation in Enumerable.Reverse(RepositoryNodeGenerations))
        //    {

        //    }
        //    */
        //}
    }
}
