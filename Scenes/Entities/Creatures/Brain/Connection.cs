/// <summary>
/// Represents a connection between two neurons in a neural network.
/// </summary>
public class Connection {

    /// <summary>
    /// Gets or sets source neuron of the connection.
    /// </summary>
    public Neuron SourceNeuron { get; set; }
    /// <summary>
    /// Gets or sets target neuron of the connection.
    /// </summary>
    public Neuron TargetNeuron { get; set; }

    /// <summary>
    /// Gets or sets the weight of the connection.
    /// </summary>
    public float Weight { get; set; }

    public Connection(Neuron source, Neuron target, float weight) {
        SourceNeuron = source;
        TargetNeuron = target;
        Weight = weight;
    }
}
