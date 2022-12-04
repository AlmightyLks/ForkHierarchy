using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Models;

namespace ForkHierarchy.Core.Helpers;

public static class DiagramHelper
{
    public static void CenterOnNode(this Diagram diagram, NodeModel node, double margin = 0)
    {
        diagram.SelectModel(node, unselectOthers: true);
        diagram.ZoomToFit(margin);

        var width = node.Size!.Width * diagram.Zoom;
        var scaledMargin = margin * diagram.Zoom;
        var deltaX = (diagram.Container!.Width / 2) - (width / 2) - scaledMargin;
        diagram.UpdatePan(deltaX, 0);
    }
}
