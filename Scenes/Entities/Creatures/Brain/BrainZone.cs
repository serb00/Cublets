/// <summary>
/// Represents a brain zone in a creature's brain.
/// </summary>
public class BrainZone
{
    public BrainZoneType Type { get; set; }
    public int NeuronsCount { get; set; }
    public BodyPart BodyPartLink { get; set; }

    /// <summary>
    /// Represents a brain zone in a creature's brain.
    /// </summary>
    /// <param name="type">The type of the brain zone.</param>
    /// <param name="count">The number of neurons in the brain zone.</param>
    /// <param name="link">The optional body part linked to the brain zone.</param>
    public BrainZone(BrainZoneType type, int count, BodyPart link = null)
    {
        Type = type;
        NeuronsCount = count;
        BodyPartLink = link;
    }
}