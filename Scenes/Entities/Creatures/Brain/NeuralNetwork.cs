using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// Represents a neural network that consists of neurons and connections between them.
/// </summary>
public class NeuralNetwork
{

    #region Attributes
    /// <summary>
    /// Stores the list of neurons in the neural network.
    /// </summary>
    public List<Neuron> Neurons { get; set; }
    /// <summary>
    /// Stores the connections for all neurons in the neural network.
    /// </summary>
    public List<NeuronConnection> NeuronConnections { get; set; }
    /// <summary>
    /// Stores the number of signal passes in the neural network per network update.
    /// </summary>
    public int SignalPasses { get; set; }

    #endregion Attributes

    #region Initialization

    /// <summary>
    /// Empty constructor. For JSON deserialization.
    /// </summary>
    public NeuralNetwork() { }

    public NeuralNetwork(List<NeuronsMapItem> neuronsMap, int signalPasses, NNConnectionsMethod connectionsMethod)
    {
        Neurons = new List<Neuron>();
        NeuronConnections = new List<NeuronConnection>();
        SignalPasses = signalPasses;
        InitializeNetwork(neuronsMap, connectionsMethod);
    }

    /// <summary>
    /// Initializes the neural network by creating neurons and connections between them based on the specified neurons map.
    /// </summary>
    /// <param name="neuronsMap">Holds information about neurons</param>
    private void InitializeNetwork(List<NeuronsMapItem> neuronsMap, NNConnectionsMethod connectionsMethod)
    {
        // Create Neurons
        foreach (var neuron in neuronsMap)
        {
            Neurons.Add(new Neuron(neuron.ID, neuron.ActivationFunction));
        }
        int maxLayer = neuronsMap.Max(x => x.Layer);

        switch (connectionsMethod)
        {
            case NNConnectionsMethod.Random:
                CreateRandomConnections(0, maxLayer);
                break;
            case NNConnectionsMethod.Full:
                CreateFullForwardConnections(neuronsMap);
                break;
            case NNConnectionsMethod.Partial:
                CreatePartialForwardConnections(neuronsMap);
                break;
            default:
                CreatePartialForwardConnections(neuronsMap);
                break;
        }
    }

    /// <summary>
    /// Creates partial forward connections between neurons in the neural network.
    /// It connects each neuron in a layer to a random subset of neurons from the previous layer.
    /// </summary>
    /// <param name="neuronsMap"></param>
    private void CreatePartialForwardConnections(List<NeuronsMapItem> neuronsMap)
    {
        int maxLayer = neuronsMap.Max(x => x.Layer);
        for (int currentLayer = 0; currentLayer <= maxLayer; currentLayer++)
        {
            neuronsMap.FindAll(x => x.Layer == currentLayer).ForEach(fromNeuron =>
            {
                int neuronsOnLevel = neuronsMap.FindAll(x => x.Layer == currentLayer + 1).Count;
                var selectedNeurons = neuronsMap.FindAll(x => x.Layer == currentLayer + 1).Take(GD.RandRange(1, neuronsOnLevel));
                foreach (var toNeuron in selectedNeurons)
                {
                    //Neurons[toNeuron.ID].Connections.Add(new Connection(Neurons[fromNeuron.ID], Neurons[toNeuron.ID], (float)GD.RandRange(-1f, 1f)));
                    NeuronConnections.Add(new NeuronConnection(fromNeuron.ID, toNeuron.ID, (float)GD.RandRange(-1f, 1f)));
                }
            });
        }
    }

    /// <summary>
    /// Creates full forward connections between neurons in the neural network.
    /// It connects each neuron in a layer to all neurons in the previous layer.
    /// </summary>
    /// <param name="neuronsMap"></param>
    private void CreateFullForwardConnections(List<NeuronsMapItem> neuronsMap)
    {
        int maxLayer = neuronsMap.Max(x => x.Layer);

        for (int currentLayer = 1; currentLayer <= maxLayer; currentLayer++)
        {
            neuronsMap.FindAll(x => x.Layer == currentLayer).ForEach(fromNeuron =>
            {
                neuronsMap.FindAll(x => x.Layer == currentLayer - 1).ForEach(toNeuron =>
                {
                    // Neurons[fromNeuron.ID].Connections.Add(new Connection(Neurons[fromNeuron.ID], Neurons[toNeuron.ID], (float)GD.RandRange(-1f, 1f)));
                    NeuronConnections.Add(new NeuronConnection(fromNeuron.ID, toNeuron.ID, (float)GD.RandRange(-1f, 1f)));
                });
            });
        }

    }

    /// <summary>
    /// Creates random connections between neurons in the neural network.
    /// </summary>
    /// <param name="minConnections">The minimum number of connections for each neuron.</param>
    /// <param name="maxConnections">The maximum number of connections for each neuron.</param>
    private void CreateRandomConnections(int minConnections, int maxConnections)
    {
        foreach (var neuron in Neurons)
        {
            int numConnections = GD.RandRange(minConnections, maxConnections);
            for (int i = 0; i < numConnections; i++)
            {
                var targetNeuron = Neurons[GD.RandRange(0, Neurons.Count - 1)];
                var weight = (float)GD.RandRange(-1f, 1f); // Random weight between -1 and 1
                // neuron.Connections.Add(new Connection(neuron, targetNeuron, weight));
                NeuronConnections.Add(new NeuronConnection(neuron.ID, targetNeuron.ID, weight));
            }
        }
    }

    #endregion Initialization

    #region Logic

    /// <summary>
    /// Updates the neural network by performing the specified number of signal passes.
    /// </summary>
    public void UpdateNetwork()
    {
        for (int i = 0; i < SignalPasses; i++)
        {
            foreach (var neuron in Neurons)
            {
                // neuron.CalculateOutput();
                var connections = NeuronConnections.Where(x => x.TargetNeuronID == neuron.ID);
                float tempValue = neuron.OutputValue;
                foreach (var conn in connections)
                {
                    tempValue += Neurons[conn.SourceNeuronID].OutputValue * conn.Weight;
                }
                neuron.OutputValue = Utils.ScaleValue(tempValue + neuron.Bias, -connections.Count() - 1, connections.Count() + 1);

                neuron.Activate();
            }
        }
    }

    #endregion Logic

    #region NeuronHelpers

    /// <summary>
    /// Retrieves the value of a neuron in the neural network.
    /// </summary>
    /// <param name="neuronID">The ID of the neuron to retrieve the value from.</param>
    /// <returns>The value of the specified neuron.</returns>
    public float GetNeuronValue(int neuronID)
    {
        return Neurons[neuronID].OutputValue;
    }

    /// <summary>
    /// Sets the value of a neuron in the neural network.
    /// </summary>
    /// <param name="neuronIndex">The index of the neuron.</param>
    /// <param name="val">The value to set.</param>
    public void SetNeuronValue(int neuronIndex, float val)
    {
        Neurons[neuronIndex].SetValue(val);
    }

    #endregion NeuronHelpers
}

