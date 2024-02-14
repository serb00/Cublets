public class Connection {
    public Neuron SourceNeuron { get; set; }
    public Neuron TargetNeuron { get; set; }
    public float Weight { get; set; }

    public Connection(Neuron source, Neuron target, float weight) {
        SourceNeuron = source;
        TargetNeuron = target;
        Weight = weight;
    }
}
