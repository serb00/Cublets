using Godot;
using System.Collections.Generic;

public class Brain
{

    #region Attributes
    private NeuralNetwork neuralNetwork { get; set; }
    public int numInputNeurons { get; set; }
    public int numOutputNeurons { get; set; }

    public int numTotalNeurons { get; set; }
    private Dictionary<int, Sensor> inputMappings;
    private Dictionary<int, Action> outputMappings;

    #endregion Attributes

    #region Constructors

    public Brain(int totalNeurons, int numInputs, int numOutputs, int minConnections, int maxConnections, int signalPasses)
    {
        numInputNeurons = numInputs;
        numOutputNeurons = numOutputs;
        numTotalNeurons = totalNeurons;
        neuralNetwork = new NeuralNetwork(totalNeurons, minConnections, maxConnections, signalPasses);
        inputMappings = new Dictionary<int, Sensor>();
        outputMappings = new Dictionary<int, Action>();
        InitializeMappings();
    }

    #endregion Constructors

    private void InitializeMappings()
    {
        // Initialize inputMappings and outputMappings
        for (int i = 0; i < numInputNeurons; i++)
        {
            inputMappings[i] = new Sensor(i);
        }
        for (int i = 0; i < numOutputNeurons; i++)
        {
            outputMappings[i] = new Action(i);
        }
    }

    public void UpdateBrain()
    {
        // Update input neurons based on sensor data
        for (int i = 0; i < numInputNeurons; i++)
        {
            // TODO: Only update the neuron if the sensor has changed
            neuralNetwork.SetNeuronValue(i, inputMappings[i].GetValue());
        }

        // Update the neural network
        // TODO: Update only the part of the network which has input neurons
        // that have been changed
        neuralNetwork.UpdateNetwork();

        // Execute actions based on output neurons
        for (int i = 0; i < numOutputNeurons; i++)
        {
            int outputIndex = neuralNetwork.Neurons.Count - numOutputNeurons + i;
            outputMappings[i].Execute(neuralNetwork.Neurons[outputIndex].OutputValue);
        }
    }

    #region NeuronHelpers

    public IEnumerable<Neuron> GetAllNeurons()
    {
        return neuralNetwork.Neurons;
    }

    public int NeuronsCount()
    {
        return neuralNetwork.Neurons.Count;
    }

    public float GetNeuron(int index)
    {
        float val;
        val = neuralNetwork.GetNeuronValue(index);
        return val;
    }

    public void SetNeuronValue(int index, float value)
    {
        neuralNetwork.SetNeuronValue(index, value);
    }

    public int ConnectionsCount()
    {
        return neuralNetwork.NeuronConnections;
    }

    #endregion NeuronHelpers
}


// Dummy Sensor and Action classes for demonstration purposes
public class Sensor
{
    readonly int ID;

    public Sensor(int i)
    {
        ID = i;
    }
    public float GetValue()
    {
        // Get the sensor value (e.g., from a game object or an external source)
        float val = (float)GD.RandRange(-1.0, 1.0);
        return val;
    }
}

public class Action
{
    readonly int ID;
    public Action(int i)
    {
        ID = i;
    }
    public void Execute(float signal)
    {
        // Perform the action based on the signal
        // Example: move a game object, change a state, etc.
        if (signal > 0.7f)
        {
            //GD.Print($"Action executed by #{ID}");
        }
    }
}
