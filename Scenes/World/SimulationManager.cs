using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class SimulationManager : Node
{

    List<Creature> population = new();

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
            Creature mutated = MutateCreature(
                nextGeneration[GD.RandRange(0, nextGeneration.Count - 1)],
                nextGeneration[GD.RandRange(0, nextGeneration.Count - 1)]);
            nextGeneration.Add(mutated);
        }

        // Generate new entities
        for (int i = 0; i < 30; i++)
        {
            Creature newCreature = GenerateCreatureBasedOnParameters();
            nextGeneration.Add(newCreature);
        }

        // TODO: Remove old population to free memory

        // Set population to next generation
        population = nextGeneration;
    }

    Creature MutateCreature(Creature firstParent, Creature secondParent)
    {
        // Implement mutation based on two parents
        return new Creature(); // Placeholder
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
