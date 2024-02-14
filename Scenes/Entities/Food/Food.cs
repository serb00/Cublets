using System;
using Godot;

public partial class Food : Area3D, IConsumable, IVisible
{
	int MinEnergy = 10;
	int MaxEnergy = 100;
	public int Energy { get; private set; } = 10;
	[Export] public float foodScale = 1;

	public void Initialize(Vector3 SpawnPosition)
	{
		Position = SpawnPosition;
		Energy = (int)GD.RandRange(MinEnergy, MaxEnergy + 1);
		foodScale = Energy / MinEnergy;
		Scale = new Vector3(foodScale, 1, foodScale);
	}

	public int Consume()
	{
		QueueFree();
		return Energy;
	}

	public int GetEnergy()
	{
		return Energy;
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public EntityData GetEntityData()
	{
		return new EntityData
		{
			entityType = EntityType.Food,
			size = foodScale
		};
	}
}
