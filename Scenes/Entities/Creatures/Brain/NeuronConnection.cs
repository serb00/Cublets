/// <summary>
/// Represents a connection between two neurons in a neural network.
/// </summary>
public class NeuronConnection
{

    /// <summary>
    /// Gets or sets source neuron of the connection.
    /// </summary>
    public int SourceNeuronID { get; set; }
    /// <summary>
    /// Gets or sets target neuron of the connection.
    /// </summary>
    public int TargetNeuronID { get; set; }

    /// <summary>
    /// Gets or sets the weight of the connection.
    /// </summary>
    public float Weight { get; set; }

    public int ConnectionFromLayer { get; set; }

    public NeuronConnection() { }

    public NeuronConnection(int source, int target, float weight, int connectionFromLayer)
    {
        SourceNeuronID = source;
        TargetNeuronID = target;
        Weight = weight;
        ConnectionFromLayer = connectionFromLayer;
    }
}
