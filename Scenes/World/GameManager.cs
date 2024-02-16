using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class GameManager : Node
{
	Main _main;
	UIPanel _panel;
	Creature selectedCreature;

	List<Creature> population = new();

	SimulationParameters _parameters;
	double currentGenerationTime = 0;

	public override void _Ready()
	{
		base._Ready();
		InitializeVariables();

		StartSimulationCycle();
	}

	private void InitializeVariables()
	{
		_main = GetParent<Main>();
		_panel = _main.GetNode<Control>("Panel") as UIPanel;
		_panel.Visible = false;
		_parameters = GetNode<SimulationParameters>("SimulationParameters");
	}

	void StartSimulationCycle()
	{

		if (population.Count == 0)
		{
			// Generate initial population
			GenerateInitialPopulation(_parameters.InitialPopulationSize);
		}

		// Run simulation for a defined amount of time
		currentGenerationTime = 0;
		// You might use a timer or check for a condition within _Process or _PhysicsProcess

	}

	void GenerateInitialPopulation(int size)
	{
		GD.Print("Generating initial population");
		for (int i = 0; i < size; i++)
		{
			// Generate and add new Creature based on parameters
			Creature newCreature = GenerateCreatureBasedOnParameters();
			population.Add(newCreature);
		}
	}

	Creature GenerateCreatureBasedOnParameters()
	{
		// Implement creature generation based on your parameters
		return new Creature(); // Placeholder
	}

	void CalculateFitnessForPopulation()
	{
		GD.Print("Calculating fitness for population");
		// Implement fitness calculation for each creature
	}

	void PrepareNextGeneration()
	{
		GD.Print("Preparing next generation");
		// Sort population based on fitness
		var sortedByFitness = population.OrderByDescending(creature => creature.Fitness).ToList();

		// Take top entities
		var nextGeneration = sortedByFitness.Take(10).ToList();

		// Mutate and add new entities based on top entities
		for (int i = 0; i < 60; i++)
		{
			Creature mutated = MutateCreature(nextGeneration[i % nextGeneration.Count]);
			nextGeneration.Add(mutated);
		}

		// Generate new entities
		for (int i = 0; i < 30; i++)
		{
			Creature newCreature = GenerateCreatureBasedOnParameters();
			nextGeneration.Add(newCreature);
		}

		// Set population to next generation
		population = nextGeneration;
	}

	Creature MutateCreature(Creature baseCreature)
	{
		// Implement mutation based on a creature
		return new Creature(); // Placeholder
	}

	public void SetSelectedCreature(Creature creature)
	{
		selectedCreature = creature;
		_panel.SetCreature(selectedCreature);
		// GD.Print($"Creature: {selectedCreature.Name} selected.");
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		currentGenerationTime += delta;
		if (currentGenerationTime > _parameters.TimePerGenerationSeconds)
		{
			// Stop simulation and calculate fitness
			CalculateFitnessForPopulation();

			// Define population for the next cycle
			PrepareNextGeneration();

			// Start next simulation cycle
			StartSimulationCycle();
		}

		#region InputManagement

		if (Input.IsActionJustPressed("spawn_creature"))
		{
			_main.CreateCreature();
			GD.Print("spawn_creature");
		}
		if (Input.IsActionJustPressed("pause"))
		{
			// Engine.TimeScale
			GetTree().Paused = !GetTree().Paused;
			GD.Print("pause");
		}
		if (Input.IsActionJustPressed("show_panel"))
		{
			_panel.Visible = !_panel.Visible;
		}
		if (Input.IsActionJustPressed("increase_time_scale"))
		{
			Engine.TimeScale *= 2;
		}
		if (Input.IsActionJustPressed("decrease_time_scale"))
		{
			Engine.TimeScale /= 2;
		}
		if (Input.IsActionJustPressed("Action1"))
		{

		}
		if (Input.IsActionJustPressed("Action2"))
		{

		}
		if (Input.IsActionJustPressed("Action3"))
		{
			_main.CreateFood();
		}
		if (Input.IsActionJustPressed("Action4"))
		{

		}

		#endregion InputManagement

	}

}
