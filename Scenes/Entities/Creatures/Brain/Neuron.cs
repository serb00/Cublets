using System.Collections.Generic;
using Godot;

public class Neuron
{
    public List<Connection> Connections { get; set; }
    public float OutputValue { get; private set; }
    public int ID { get; set; }
    public NeuronActivationFunction ActivationFunction { get; set; }
    float _bias;

    public Neuron(int id)
    {
        Connections = new List<Connection>();
        ID = id;
        OutputValue = (float)GD.RandRange(-1f, 1f);
        _bias = (float)GD.RandRange(-1f, 1f);
        
        ActivationFunction = Utils.GetRandomEnumValue<NeuronActivationFunction>();
    }


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

    public void SetValue(float val)
    {
        OutputValue = val;
    }

    public void SetValue(float val, float bottomBoundary, float topBoundary)
    {
        OutputValue = ScaleSignal(val, bottomBoundary, topBoundary);
    }

    public static float ScaleSignal(float val, float bottomBoundary, float topBoundary)
    {
        return (val - bottomBoundary) / (topBoundary - bottomBoundary);
    }
}
