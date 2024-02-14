using System.Collections.Generic;
using Godot;

public class NeuralNetwork
{

    #region Attributes
    public List<Neuron> Neurons { get; private set; }
    public int NeuronConnections { get; private set; }
    public int SignalPasses { get; private set; }

    #endregion Attributes

    #region Constructors

    public NeuralNetwork(int numNeurons, int minConnections, int maxConnections, int signalPasses)
    {
        Neurons = new List<Neuron>();
        SignalPasses = signalPasses;
        NeuronConnections = 0;
        InitializeNetwork(numNeurons, minConnections, maxConnections);
    }

    #endregion Constructors

    private void InitializeNetwork(int numNeurons, int minConnections, int maxConnections)
    {
        // Create Neurons
        for (int i = 0; i < numNeurons; i++)
        {
            Neurons.Add(new Neuron(i));
        }

        // Create Random Connections
        foreach (var neuron in Neurons)
        {
            int numConnections = GD.RandRange(minConnections, maxConnections);
            for (int i = 0; i < numConnections; i++)
            {
                var targetNeuron = Neurons[GD.RandRange(0, Neurons.Count - 1)];
                var weight = (float)GD.RandRange(-1f, 1f); // Random weight between -1 and 1
                neuron.Connections.Add(new Connection(neuron, targetNeuron, weight));
            }
            NeuronConnections += numConnections;
        }
    }

    public void UpdateNetwork()
    {
        for (int i = 0; i < SignalPasses; i++)
        {
            foreach (var neuron in Neurons)
            {
                neuron.CalculateOutput();
            }
        }
    }

    #region NeuronHelpers

    public float GetNeuronValue(int neuronID)
    {
        return Neurons[neuronID].OutputValue;
    }

    public void SetNeuronValue(int neuronIndex, float val)
    {
        Neurons[neuronIndex].SetValue(val);
    }

    #endregion NeuronHelpers
}

