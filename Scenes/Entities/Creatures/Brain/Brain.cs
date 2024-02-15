using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Brain
{

    #region Attributes
    private NeuralNetwork NeuralNetwork { get; set; }
    public int NumInputNeurons { get; set; }
    public int NumOutputNeurons { get; set; }
    public int NumTotalNeurons { get; set; }
    public List<NeuronsMapItem> NeuronsMap { get; set; } = new();

    private Dictionary<int, Action> outputMappings = new();

    #endregion Attributes

    #region Constructors

    public void Initialize(
        List<BrainZone> inputNeuronsList,
        List<BrainZone> outputNeuronsList,
        int brainHiddenLayers, int signalPasses)
    {
        NumInputNeurons = inputNeuronsList.Sum(x => x.NeuronsCount);
        NumOutputNeurons = outputNeuronsList.Sum(x => x.NeuronsCount);
        int[] NumHiddenNeurons = new int[brainHiddenLayers];
        for (int i = 0; i < brainHiddenLayers; i++)
        {
            NumHiddenNeurons[i] += GD.RandRange(NumInputNeurons, NumInputNeurons + NumOutputNeurons);
        }
        NumTotalNeurons = NumInputNeurons + NumOutputNeurons + NumHiddenNeurons.Sum();
        int lastIndex = MapInputNeurons(inputNeuronsList);
        lastIndex = MapHiddenNeurons(NumHiddenNeurons, lastIndex);
        MapOutputNeurons(outputNeuronsList, brainHiddenLayers + 1, lastIndex);

        NeuralNetwork = new NeuralNetwork(NeuronsMap, signalPasses);
        outputMappings = new();
        InitializeMappings();
    }

    private void MapOutputNeurons(List<BrainZone> outputNeuronsList, int outputLayerIndex, int idx)
    {
        foreach (var item in outputNeuronsList)
        {
            // movement neurons
            if (item.BodyPartLink == null)
            {
                NeuronsMap.Add(new NeuronsMapItem(idx++, BrainZoneType.Movement, NeuronActivationFunction.HyperbolicTangent, null, outputLayerIndex));
                NeuronsMap.Add(new NeuronsMapItem(idx++, BrainZoneType.Movement, NeuronActivationFunction.HyperbolicTangent, null, outputLayerIndex));
            }
            else
            {
                switch (item.BodyPartLink.Type)
                {
                    case BodyPartType.Mouth:
                        NeuronsMap.Add(new NeuronsMapItem(idx++, BrainZoneType.Movement, NeuronActivationFunction.BinaryStep, item.BodyPartLink, outputLayerIndex));
                        break;
                    default:
                        GD.Print("Unknown body part type in Brain.MapOutputNeurons method");
                        break;
                }
            }
        }
    }

    private int MapHiddenNeurons(int[] hiddenLayers, int idx)
    {
        int layer = 0;
        foreach (var neuronsInLayer in hiddenLayers)
        {
            layer++;
            for (int i = 0; i < neuronsInLayer; i++)
            {
                NeuronsMap.Add(new NeuronsMapItem(idx++, BrainZoneType.Internal, Utils.GetRandomEnumValue<NeuronActivationFunction>(), null, layer));
            }
        }
        return idx;
    }

    private int MapInputNeurons(List<BrainZone> inputNeuronsList)
    {
        int idx = 0;
        foreach (var item in inputNeuronsList)
        {
            if (item.BodyPartLink != null)
            {
                switch (item.BodyPartLink.Type)
                {
                    case BodyPartType.Eye:
                        Eye eye = item.BodyPartLink as Eye;
                        for (int i = 0; i < eye._eyeData.EyeComplexity; i++)
                        {
                            NeuronsMap.Add(new NeuronsMapItem(idx++, item.Type, NeuronActivationFunction.Sigmoid, item.BodyPartLink, 0));
                            NeuronsMap.Add(new NeuronsMapItem(idx++, item.Type, NeuronActivationFunction.HyperbolicTangent, item.BodyPartLink, 0));
                            NeuronsMap.Add(new NeuronsMapItem(idx++, item.Type, NeuronActivationFunction.Sigmoid, item.BodyPartLink, 0));
                        }
                        break;
                    case BodyPartType.Mouth:
                        NeuronsMap.Add(new NeuronsMapItem(idx++, item.Type, NeuronActivationFunction.BinaryStep, item.BodyPartLink, 0));
                        break;
                    default:
                        GD.Print("Unknown body part type in Brain.MapInputNeurons method");
                        break;
                }
            }
        }
        // GD.Print($"Input neurons mapped: {idx}. Total input neurons: {NumInputNeurons}");
        return idx;
    }

    #endregion Constructors

    private void InitializeMappings()
    {
        // Initialize inputMappings and outputMappings
        for (int i = 0; i < NumOutputNeurons; i++)
        {
            outputMappings[i] = new Action(i);
        }
    }

    public void UpdateBrain()
    {
        // Update the neural network
        // TODO: Update only the part of the network which has input neurons
        // that have been changed
        NeuralNetwork.UpdateNetwork();

        // Execute actions based on output neurons
        for (int i = 0; i < NumOutputNeurons; i++)
        {
            int outputIndex = NeuralNetwork.Neurons.Count - NumOutputNeurons + i;
            outputMappings[i].Execute(NeuralNetwork.Neurons[outputIndex].OutputValue);
        }
    }

    #region NeuronHelpers

    public IEnumerable<Neuron> GetAllNeurons()
    {
        return NeuralNetwork.Neurons;
    }

    public int NeuronsCount()
    {
        return NeuralNetwork.Neurons.Count;
    }

    public float GetNeuron(int index)
    {
        return NeuralNetwork.GetNeuronValue(index);
    }

    public void SetNeuronValue(int index, float val)
    {
        NeuralNetwork.SetNeuronValue(index, val);
    }

    public int ConnectionsCount()
    {
        return NeuralNetwork.NeuronConnections;
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
    readonly Delegate action;
    public Action(int i, Delegate delegateAction = null)
    {
        ID = i;
        action = delegateAction;
    }
    public void Execute(float signal)
    {
        // Perform the action based on the signal
        // Example: move a game object, change a state, etc.
        if (signal > 0.7f)
        {
            action?.DynamicInvoke();
        }
    }
}
