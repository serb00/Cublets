using Godot;
using System;
using System.Collections.Generic;

public partial class BrainVisualizer : Panel
{
	[Export] public PackedScene NeuronScene;
	Vector2 NeuronSize = new(20, 20);
	[Export] public PackedScene ConnectionScene;
	[Export] public PackedScene SelfConnectionScene;

	private float panelWidth;
	private float panelHeight;
	private Vector2 panelOffset;

	private List<TextureRect> neuronNodes;
	private List<TextureRect> connectionNodes;

	public float connectionLenghtOffset = 20;

	private float Margin;
	public float InputOutputSpacing;
	public Brain BrainInstance;

	private readonly Color ColorMin = Colors.Red;
	private readonly Color ColorMid = Colors.White;
	private readonly Color ColorMax = Colors.Green;
	double timeSinceLastUpdate = 0f;

	public void SetBrainInstance(Brain brain)
	{
		BrainInstance = brain;
		ClearGraph();
		CreateGraph();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		panelWidth = 558;
		panelHeight = 720;
		Margin = panelWidth / 10;
	}

	private void ClearGraph()
	{
		neuronNodes ??= new List<TextureRect>();
		neuronNodes.Clear();
		connectionNodes ??= new List<TextureRect>();
		connectionNodes.Clear();
		foreach (var child in GetChildren())
		{
			child.QueueFree();
		}
	}

	private void CreateGraph()
	{
		// Access the neurons from the Brain's NeuralNetwork
		foreach (var neuron in BrainInstance.GetAllNeurons())
		{
			NeuronType type = DetermineNeuronType(neuron.ID);
			var neuronNode = NeuronScene.Instantiate<TextureRect>();
			AddChild(neuronNode);
			neuronNode.Position = GetNeuronPosition(neuron.ID, type) - NeuronSize / 2; // Assuming 2D and GetNeuronPosition returns a Vector2
			neuronNode.Name = $"Neuron {neuron.ID} - {type}";
			neuronNodes.Insert(neuron.ID, neuronNode); // Ensure neuronGameObjects is initialized
		}


		// Instantiate and setup connections
		foreach (var neuron in BrainInstance.GetAllNeurons())
		{
			foreach (var connection in neuron.Connections)
			{
				if (connection.SourceNeuron == connection.TargetNeuron)
				{
					// Create a self-connection
					CreateSelfConnection(neuron, connection.Weight);
				}
				else
				{
					// Create a linear connection
					CreateLinearConnection(connection);
				}
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		// Update the Brain every 0.5 seconds
		if (BrainInstance is not null)
		{
			timeSinceLastUpdate += delta;
			if (timeSinceLastUpdate > 0.5f)
			{
				UpdateGraph();
				timeSinceLastUpdate = 0f;
			}
		}
	}

	#region DrawingConnections
	private void CreateLinearConnection(Connection connection)
	{
		//TODO: fix
		var sourcePos = neuronNodes[connection.SourceNeuron.ID].Position;
		var targetPos = neuronNodes[connection.TargetNeuron.ID].Position;

		var connectionNode = ConnectionScene.Instantiate<TextureRect>();
		connectionNode.Name = $"Connection {connection.SourceNeuron.ID} -> {connection.TargetNeuron.ID}";
		if (connection.Weight < 0)
		{
			// If weight is between -1 and 0, interpolate between red and white
			connectionNode.SelfModulate = ColorMin; //colorMin.Lerp(colorMid, connection.Weight);
		}
		else
		{
			// If weight is between 0 and 1, interpolate between white and green
			connectionNode.SelfModulate = ColorMax; //colorMid.Lerp(colorMax, connection.Weight);
		}

		// Position
		connectionNode.Position = sourcePos + NeuronSize / 2;

		// Rotation
		var direction = targetPos - sourcePos;
		connectionNode.Rotation = Mathf.Atan2(direction.Y, direction.X) - Mathf.Pi / 2;

		// Scale
		float length = sourcePos.DistanceTo(targetPos);
		var thickness = Math.Clamp(connection.Weight, 4, 10);
		connectionNode.SetSize(new Vector2(thickness, length - connectionLenghtOffset)); // TODO: Adjust x for thickness based on weight

		connectionNodes.Add(connectionNode);
		AddChild(connectionNode);
	}

	private void CreateSelfConnection(Neuron neuron, float weight)
	{
		var neuronPos = neuronNodes[neuron.ID].Position;

		var selfConnectionNode = SelfConnectionScene.Instantiate<TextureRect>();
		selfConnectionNode.Name = string.Format("Self Connection {0}", neuron.ID);
		if (weight < 0)
		{
			// If weight is between -1 and 0, interpolate between red and white
			selfConnectionNode.SelfModulate = ColorMin.Lerp(ColorMid, weight + 1);
		}
		else
		{
			// If weight is between 0 and 1, interpolate between white and green
			selfConnectionNode.SelfModulate = ColorMid.Lerp(ColorMax, weight + 1);
		}
		// Position
		Vector2 offset = new(12, 12);
		selfConnectionNode.Position = neuronPos + offset;


		connectionNodes.Add(selfConnectionNode);
		AddChild(selfConnectionNode);
	}

	#endregion DrawingConnections

	#region DrawingNeurons
	private NeuronType DetermineNeuronType(int index)
	{
		if (index < BrainInstance.numInputNeurons)
		{
			return NeuronType.Input;
		}
		else if (index >= BrainInstance.NeuronsCount() - BrainInstance.numOutputNeurons)
		{
			return NeuronType.Output;
		}
		else
		{
			return NeuronType.Hidden;
		}
	}

	private Vector2 GetNeuronPosition(int index, NeuronType type)
	{
		float x, y;
		switch (type)
		{
			case NeuronType.Input:
				x = Margin;
				y = CalculateVerticalPosition(index, BrainInstance.numInputNeurons, panelHeight);
				break;
			case NeuronType.Output:
				x = panelWidth - Margin;
				y = CalculateVerticalPosition(index - BrainInstance.NeuronsCount() + BrainInstance.numOutputNeurons, BrainInstance.numOutputNeurons, panelHeight);
				break;
			case NeuronType.Hidden:
				x = (float)GD.RandRange(Margin + InputOutputSpacing, panelWidth - Margin - InputOutputSpacing);
				y = (float)GD.RandRange(Margin, panelHeight - Margin);
				break;
			default:
				x = 0; y = 0;
				GD.Print((nameof(type), "Invalid neuron type."));
				break;
		}
		//Debug.Log($"Neuron {index} position: {x}, {y}");
		return new Vector2(x, y);
	}

	private float CalculateVerticalPosition(int index, int totalNeuronsOfType, float panelHeight)
	{
		float spacing = (panelHeight - 2 * Margin) / (totalNeuronsOfType - 1);
		return Margin + spacing * index;
	}

	private void UpdateNeuronColor(TextureRect neuronNode, float outputValue)
	{
		// Set the color based on the output value: red-white-green gradient
		if (outputValue < 0)
		{
			// If outputValue is between -1 and 0, interpolate between red and white
			neuronNode.SelfModulate = ColorMin.Lerp(ColorMid, outputValue + 1);
		}
		else
		{
			// If outputValue is between 0 and 1, interpolate between white and green
			neuronNode.SelfModulate = ColorMid.Lerp(ColorMax, outputValue + 1);
		}
	}

	#endregion DrawingNeurons

	private void UpdateGraph()
	{
		// Update neurons' colors based on their output values
		for (int i = 0; i < BrainInstance.NeuronsCount(); i++)
		{
			UpdateNeuronColor(neuronNodes[i], BrainInstance.GetNeuron(i));
		}
	}
}
