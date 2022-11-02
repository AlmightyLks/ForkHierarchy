using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;

namespace ForkHierarchy.Pages
{
    public partial class Index
    {
        public Diagram Diagram { get; set; } = null!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var options = new DiagramOptions
            {
                DeleteKey = "Delete", // What key deletes the selected nodes/links
                DefaultNodeComponent = null, // Default component for nodes
                AllowMultiSelection = true, // Whether to allow multi selection using CTRL
                Links = new DiagramLinkOptions
                {
                    // Options related to links
                },
                Zoom = new DiagramZoomOptions
                {
                    Minimum = 0.5, // Minimum zoom value
                    Inverse = false, // Whether to inverse the direction of the zoom when using the wheel
                                     // Other
                }
            };
            Diagram = new Diagram(options);
            Setup();
            StateHasChanged();
        }

        private void Setup()
        {
            var node1 = NewNode(50, 50);
            var node2 = NewNode(300, 300);
            var node3 = NewNode(300, 50);
            Diagram.Nodes.Add(new[] { node1, node2, node3 });
            Diagram.Links.Add(new LinkModel(node1.GetPort(PortAlignment.Right), node2.GetPort(PortAlignment.Left)));
        }

        private NodeModel NewNode(double x, double y)
        {
            var node = new NodeModel(new Point(x, y));
            node.AddPort(PortAlignment.Bottom);
            node.AddPort(PortAlignment.Top);
            node.AddPort(PortAlignment.Left);
            node.AddPort(PortAlignment.Right);
            return node;
        }



        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

                //var foo = await HierachyBuilder.Gather("SynapseSL", "Synapse");
                //var foo = await HierachyBuilder.Gather("wind4000", "vits");
                //var foo = await HierachyBuilder.Gather("AlmightyLks", "ForkHierarchy").ToListAsync();
            }
        }
    }
}