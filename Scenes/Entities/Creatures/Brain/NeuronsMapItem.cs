public struct NeuronsMapItem
{
    public readonly int ID;
    public readonly BrainZoneType Type;
    public readonly NeuronActivationFunction ActivationFunction;
    public readonly BodyPart BodyPartLink;
    public readonly int Layer;

    public NeuronsMapItem(
        int id, BrainZoneType type,
        NeuronActivationFunction activationFunction,
        BodyPart bodyPartLink, int layer)
    {
        ID = id;
        Type = type;
        ActivationFunction = activationFunction;
        BodyPartLink = bodyPartLink;
        Layer = layer;
    }
}