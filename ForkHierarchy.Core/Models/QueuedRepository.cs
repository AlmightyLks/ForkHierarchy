namespace ForkHierarchy.Core.Models;

public class QueuedRepository
{
    public int Id { get; set; }
    public string Owner { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime AddedAt { get; set; }
    public DateTime ETA { get; set; }
}