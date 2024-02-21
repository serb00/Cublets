/// <summary>
/// Represents a neuron in the neurons map.
/// </summary>
public struct NeuronsMapItem
{
    public int ID { get; set; }

    /// <summary>
    /// Represents the type of the brain zone.
    /// </summary>
    public BrainZoneType Type { get; set; }
    /// <summary>
    /// Represents the activation function of the neuron.
    /// </summary>
    public NeuronActivationFunction ActivationFunction { get; set; }
    // public BodyPart BodyPartLink { get; set; }

    /// <summary>
    /// Represents the number of layer of the neuron in the neural network.
    /// 0 being the input layer, etc.
    /// </summary>
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