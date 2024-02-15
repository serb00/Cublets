using System;
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
    public List<Neuron> Neurons { get; private set; }
    /// <summary>
    /// Stores the number of connections for all neurons in the neural network.
    /// </summary>
    public int NeuronConnections { get; private set; }
    /// <summary>
    /// Stores the number of signal passes in the neural network per network update.
    /// </summary>
    public int SignalPasses { get; private set; }

    #endregion Attributes

    #region Initialization

    public NeuralNetwork(List<NeuronsMapItem> neuronsMap, int minConnections, int maxConnections, int signalPasses)
    {
        Neurons = new List<Neuron>();
        SignalPasses = signalPasses;
        NeuronConnections = 0;
        InitializeNetwork(neuronsMap, minConnections, maxConnections);
    }

    private void InitializeNetwork(List<NeuronsMapItem> neuronsMap, int minConnections, int maxConnections)
    {
        // Create Neurons
        foreach (var neuron in neuronsMap)
        {
            Neurons.Add(new Neuron(neuron.ID, neuron.ActivationFunction));
        }

        // CreateRandomConnections(minConnections, maxConnections);
        CreateFullForwardConnections(neuronsMap);
    }

    private void CreateFullForwardConnections(List<NeuronsMapItem> neuronsMap)
    {
        int maxLayer = neuronsMap.Max(x => x.Layer);

        for (int currentLayer = 1; currentLayer <= maxLayer; currentLayer++)
        {
            neuronsMap.FindAll(x => x.Layer == currentLayer).ForEach(toNeuron =>
            {
                neuronsMap.FindAll(x => x.Layer == currentLayer - 1).ForEach(fromNeuron =>
                {
                    Neurons[fromNeuron.ID].Connections.Add(new Connection(Neurons[fromNeuron.ID], Neurons[toNeuron.ID], (float)GD.RandRange(-1f, 1f)));
                    NeuronConnections++;
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
                neuron.Connections.Add(new Connection(neuron, targetNeuron, weight));
            }
            NeuronConnections += numConnections;
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
                neuron.CalculateOutput();
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

