using Godot;

public partial class Main : Node
{
	[Export] public PackedScene FoodScene { get; set; }
	[Export] Node parentFood;
	[Export] Node parentCreatures;
	[Export] PackedScene _creatureScene;

	private void OnSingleCreatureTimerTimeout()
	{
		AddChild(_creatureScene.Instantiate());
	}

	public void CreateCreature()
	{
		OnCreatureTimerTimeout();
	}

	public void CreateFood()
	{
		OnFoodTimerTimeout();
	}


	private void OnCreatureTimerTimeout()
	{
		// Create a new instance of the Mob scene.
		Creature creature = _creatureScene.Instantiate<Creature>();

		// Choose a random location on the SpawnPath.
		// We store the reference to the SpawnLocation node.
		var mobSpawnLocation = GetNode<PathFollow3D>("Creatures/SpawnPath/SpawnLocation");
		// And give it a random offset.
		mobSpawnLocation.ProgressRatio = GD.Randf();

		Vector3 playerPosition = Vector3.Zero;
		creature.Initialize(mobSpawnLocation.Position);

		// Spawn the mob by adding it to the Main scene.
		parentCreatures.AddChild(creature);
	}

	private void OnFoodTimerTimeout()
	{
		// Create a new instance of the Mob scene.
		Food food = FoodScene.Instantiate<Food>();

		// Choose a random location on the SpawnPath.
		// We store the reference to the SpawnLocation node.
		int spawnX = (int)GD.RandRange(-30, 30 + 1);
		int spawnZ = (int)GD.RandRange(-30, 30 + 1);
		Vector3 foodSpawnLocation = new(spawnX, 0.3f, spawnZ);

		food.Initialize(foodSpawnLocation);

		// Spawn the mob by adding it to the Main scene.
		parentFood.AddChild(food);
	}


}
