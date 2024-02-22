using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class SimulationManager : Node
{

    List<Creature> population = new();
    BodyPartsCollection bodyPartsCollection;
    PathFollow3D spawnPath;

    SimulationParameters _parameters;
    int keepInPopulation;
    int mutateInPopulation;
    int changeBrainInPopulation;
    int randomInPopulation;
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
        spawnPath = GetNode<PathFollow3D>("/root/Main/Creatures/SpawnPath/SpawnLocation");
        bodyPartsCollection = GetNode<BodyPartsCollection>("/root/BodyPartsCollection");
        Engine.RegisterSingleton("BodyPartsCollection", bodyPartsCollection);
        keepInPopulation = _parameters.KeepFromPopulationPercent * _parameters.InitialPopulationSize / 100;
        mutateInPopulation = _parameters.MutateFromPopulationPercent * _parameters.InitialPopulationSize / 100;
        changeBrainInPopulation = _parameters.ChangeBrainFromPopulationPercent * _parameters.InitialPopulationSize / 100;
        randomInPopulation = _parameters.RandomInPopulationPercent * _parameters.InitialPopulationSize / 100;
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
            dna = CreateNewDNA();
        }

        PackedScene creatureScene = (PackedScene)ResourceLoader.Load("res://Scenes/Entities/Creatures/Creature.tscn");
        Creature creature = creatureScene.Instantiate<Creature>();

        creature.Initialize(GetRandomSpawnPosition(), dna);

        var parentCreatures = GetNode<Node>("/root/Main/Creatures");

        // Spawn the creature by adding it to the Main scene.
        parentCreatures.AddChild(creature);
        return creature;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        spawnPath.ProgressRatio = GD.Randf();
        return spawnPath.Position;
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

        var nextGeneration = sortedByFitness.Take(keepInPopulation).ToList();

        // Mutate and add new entities based on top entities
        for (int i = 0; i < mutateInPopulation; i++)
        {
            Creature mutated = MutateCreature(
                nextGeneration[GD.RandRange(0, keepInPopulation - 1)],
                nextGeneration[GD.RandRange(0, keepInPopulation - 1)]);
            nextGeneration.Add(mutated);
        }

        // Randomize creatures brains based on top entities
        for (int i = 0; i < changeBrainInPopulation; i++)
        {
            Creature mutated = RandomizeBrain(
                nextGeneration[GD.RandRange(0, keepInPopulation - 1)]
            );
            nextGeneration.Add(mutated);
        }

        // Generate new entities
        for (int i = 0; i < randomInPopulation; i++)
        {
            Creature newCreature = GenerateCreature();
            nextGeneration.Add(newCreature);
        }

        // Re-enable population that stays in the next generation
        for (int i = 0; i < keepInPopulation; i++)
        {
            sortedByFitness[i].EnableCharacter(GetRandomSpawnPosition());
        }

        // Remove old population to free memory
        for (int i = keepInPopulation; i < sortedByFitness.Count; i++)
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

    private DNA CreateNewDNA()
    {
        DNA.BodyGenes bodyGene = new()
        {
            ID = 1,
            Type = BodyPartType.Body,
            Size = (float)GD.RandRange(0.5f, 2f),
        };
        DNA.BrainGenes brainGene = new()
        {
            NumHiddenLayers = GD.RandRange(0, 5),
            ConnectionsMethod = Utils.GetRandomEnumValue<NNConnectionsMethod>()
        };
        List<DNA.EyeGenes> eyesGene = new() {
                new() { ID = 1, Angle = new(0, 0.5f, -1) }
                , new() { ID = 3, Angle = new(-1, 0.25f, 0) }
                , new() { ID = 3, Angle = new(1, 0.25f, 0) }
            };
        List<DNA.MouthGenes> mouthsGene = new() {
                new() { ID = 1, Angle = new(0, -0.5f, -1) }
            };

        return new DNA(
            EntityType.Creature,
            bodyGene,
            brainGene,
            eyesGene,
            mouthsGene
        );
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
