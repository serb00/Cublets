using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class SimulationManager : Node
{

    List<Creature> population = new();
    readonly BodyPartsCollection bodyPartsCollection = new();

    SimulationParameters _parameters;
    double currentGenerationTime = 0;
    int currentSimulationCycle = 0;
    bool simulationRunning = false;

    public override void _Ready()
    {
        base._Ready();
        InitializeVariables();
        StartSimulationCycle();
    }

    private void InitializeVariables()
    {
        _parameters = GetNode<SimulationParameters>("SimulationParameters");
    }

    void StartSimulationCycle()
    {
        GD.Print($"Start Simulation cycle {currentSimulationCycle++}");
        if (population.Count == 0)
        {
            // Generate initial population
            GenerateInitialPopulation(_parameters.InitialPopulationSize);
        }

        // Run simulation
        currentGenerationTime = 0;
        simulationRunning = true;
    }

    void FinishSimulationCycle()
    {

        // Stop simulation
        simulationRunning = false;

        // Calculate fitness
        CalculateFitnessForPopulation();

        if (currentSimulationCycle < _parameters.MaxGenerations)
        {
            // Define population for the next cycle
            PrepareNextGeneration();

            // Start next simulation cycle
            StartSimulationCycle();
        }
        else
        {
            GD.Print("Simulation finished");
        }
    }

    void GenerateInitialPopulation(int size)
    {
        GD.Print("Generating initial population");
        for (int i = 0; i < size; i++)
        {
            // Generate and add new Creature based on parameters
            Creature newCreature = GenerateCreature();
            population.Add(newCreature);
        }
    }

    Creature GenerateCreature(DNA dna = null)
    {
        // TODO: Implement creature generation based on your parameters
        if (dna == null)
        {
            DNA.BodyStruct bodyData = new() { ID = 1, Type = "Body", Size = 1.0f };
            DNA.BrainStruct brainData = new() { Complexity = 1, NumLayers = 1 };
            List<DNA.EyeStruct> eyesData = new() {
                new() { ID = 1, Angle = new(0, 0.5f, -1) }
                // , new() { ID = 2, Angle = new(-1, 0.25f, 0) }
            };
            List<DNA.MouthStruct> mouthsData = new() {
                new() { ID = 1, Angle = new(0, -0.5f, -1) }
            };

            dna = new DNA(
                EntityType.Creature,
                bodyData,
                brainData,
                eyesData,
                mouthsData
            );
        }

        PackedScene creatureScene = (PackedScene)ResourceLoader.Load("res://Scenes/Entities/Creatures/Creature.tscn");
        Creature creature = creatureScene.Instantiate<Creature>();

        // Choose a random location on the SpawnPath.
        // We store the reference to the SpawnLocation node.
        var mobSpawnLocation = GetNode<PathFollow3D>("/root/Main/Creatures/SpawnPath/SpawnLocation");
        // And give it a random offset.
        mobSpawnLocation.ProgressRatio = GD.Randf();
        creature.Initialize(mobSpawnLocation.Position, dna);

        var parentCreatures = GetNode<Node>("/root/Main/Creatures");

        // Spawn the creature by adding it to the Main scene.
        parentCreatures.AddChild(creature);
        return creature;
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

        var nextGeneration = sortedByFitness.Take(10).ToList();

        // Mutate and add new entities based on top entities
        for (int i = 0; i < 30; i++)
        {
            Creature mutated = MutateCreature(
                nextGeneration[GD.RandRange(0, 10 - 1)],
                nextGeneration[GD.RandRange(0, 10 - 1)]);
            nextGeneration.Add(mutated);
        }

        // Randomize creatures brains based on top entities
        for (int i = 0; i < 30; i++)
        {
            Creature mutated = RandomizeBrain(
                nextGeneration[GD.RandRange(0, 10 - 1)]
            );
            nextGeneration.Add(mutated);
        }

        // Generate new entities
        for (int i = 0; i < 30; i++)
        {
            Creature newCreature = GenerateCreature();
            nextGeneration.Add(newCreature);
        }

        // Remove old population to free memory
        for (int i = 10; i < sortedByFitness.Count; i++)
        {
            sortedByFitness[i].QueueFree();
        }
        population.Clear();

        // Set population to next generation
        population = nextGeneration;
    }

    Creature MutateCreature(Creature firstParent, Creature secondParent)
    {
        // TODO: Implement mutation based on two parents
        return GenerateCreature(); // Placeholder
    }

    Creature RandomizeBrain(Creature creature)
    {
        // TODO: Implement randomization of brain
        return GenerateCreature(); // Placeholder
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (simulationRunning)
            currentGenerationTime += delta;

        if (simulationRunning && currentGenerationTime > _parameters.TimePerGenerationSeconds)
        {
            FinishSimulationCycle();
        }
    }
}
