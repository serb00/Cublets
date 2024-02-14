using System.Collections.Generic;
using Godot;

/// <summary>
/// Represents a neuron in a neural network.
/// </summary>
public class Neuron
{
    /// <summary>
    /// The list of connections associated with this neuron.
    /// </summary>
    public List<Connection> Connections { get; set; }

    /// <summary>
    /// The output value of the neuron.
    /// </summary>
    public float OutputValue { get; private set; }

    /// <summary>
    /// The ID of the neuron.
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets the activation function used by the neuron.
    /// </summary>
    public NeuronActivationFunction ActivationFunction { get; set; }

    /// <summary>
    /// The bias value of the neuron.
    /// </summary>
    readonly float _bias;

    public Neuron(int id)
    {
        Connections = new List<Connection>();
        ID = id;
        OutputValue = (float)GD.RandRange(-1f, 1f);
        _bias = (float)GD.RandRange(-1f, 1f);
        
        ActivationFunction = Utils.GetRandomEnumValue<NeuronActivationFunction>();
    }


    /// <summary>
    /// Calculates the output value of the neuron based on the input connections and weights.
    /// </summary>
    public void CalculateOutput()
    {
        float tempValue = 0f;
        foreach (var conn in Connections)
        {
            tempValue += conn.SourceNeuron.OutputValue * conn.Weight;
        }
        OutputValue = ScaleSignal(tempValue + _bias, -Connections.Count - 1, Connections.Count + 1);

        Activate();
    }

    /// <summary>
    /// Activates the neuron based on the specified activation function.
    /// </summary>
    private void Activate()
    {
        switch (ActivationFunction)
        {
            case NeuronActivationFunction.BinaryStep:
                OutputValue = OutputValue > 0 ? 1 : 0;
                break;
            case NeuronActivationFunction.Sigmoid:
                OutputValue = 1f / (1f + Mathf.Exp(-OutputValue));
                break;
            case NeuronActivationFunction.HyperbolicTangent:
                OutputValue = Mathf.Tanh(OutputValue);
                break;
            case NeuronActivationFunction.Sign:
                OutputValue = Mathf.Sign(OutputValue);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Sets the value of the neuron based on the given input value and boundaries.
    /// </summary>
    /// <param name="val">The input value.</param>
    /// <param name="bottomBoundary">The bottom boundary of the neuron's output value.</param>
    /// <param name="topBoundary">The top boundary of the neuron's output value.</param>
    public void SetValue(float val, float bottomBoundary = -1, float topBoundary = -1)
    {
        OutputValue = ScaleSignal(val, bottomBoundary, topBoundary);
    }

    /// <summary>
    /// Scales a value between a specified bottom and top boundary.
    /// </summary>
    /// <param name="val">The value to be scaled.</param>
    /// <param name="bottomBoundary">The bottom boundary of the scaling range.</param>
    /// <param name="topBoundary">The top boundary of the scaling range.</param>
    /// <returns>The scaled value.</returns>
    public static float ScaleSignal(float val, float bottomBoundary, float topBoundary)
    {
        return (val - bottomBoundary) / (topBoundary - bottomBoundary);
    }
}
