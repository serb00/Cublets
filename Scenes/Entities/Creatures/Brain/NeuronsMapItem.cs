public struct NeuronsMapItem
{
    public int ID { get; set; }
    public BrainZoneType Type { get; set; }
    public NeuronActivationFunction ActivationFunction { get; set; }
    // public BodyPart BodyPartLink { get; set; }
    public int Layer { get; set; }

    public NeuronsMapItem(
        int id, BrainZoneType type,
        NeuronActivationFunction activationFunction,
        BodyPart bodyPartLink, int layer)
    {
        ID = id;
        Type = type;
        ActivationFunction = activationFunction;
        // BodyPartLink = bodyPartLink;
        Layer = layer;
    }
}