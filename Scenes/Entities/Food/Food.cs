using System;
using Godot;

public partial class Food : StaticBody3D, IConsumable, IVisible
{
	int MinEnergy = 10;
	int MaxEnergy = 100;
	public int Energy { get; private set; } = 10;
	[Export] public float foodScale = 1;
	int CalloriesMultiplier = 1000;

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
		return Energy * CalloriesMultiplier;
	}

	public int GetEnergy()
	{
		return Energy * CalloriesMultiplier;
	}

	public VisibleEntityData GetEntityData()
	{
		return new VisibleEntityData
		{
			entityType = EntityType.Food,
			size = foodScale
		};
	}
}
