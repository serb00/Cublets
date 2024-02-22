
using Godot;

public partial class SimulationParameters : Node
{
    [ExportGroup("Population")]
    [Export] public int InitialPopulationSize { get; set; } = 100;
    [Export] public int KeepFromPopulationPercent { get; set; } = 10;
    [Export] public int MutateFromPopulationPercent { get; set; } = 30;
    [Export] public int ChangeBrainFromPopulationPercent { get; set; } = 30;
    [Export] public int RandomInPopulationPercent { get; set; } = 30;

    [ExportGroup("Generations")]
    [Export] public int TimePerGenerationSeconds { get; set; } = 20;
    [Export] public int MaxGenerations { get; set; } = 10;

    [ExportGroup("Creature parameters")]
    [Export] public float MinCreatureSize { get; set; } = 0.5f;
    [Export] public float MaxCreatureSize { get; set; } = 2.0f;
    // TODO: to implement in the future
    // [Export] public float MinCreatureSpeed { get; set; } = 0.5f;
    // [Export] public float MaxCreatureSpeed { get; set; } = 2.0f;
    // [Export] public float MinCreatureRotationSpeed { get; set; } = 0.5f;
    // [Export] public float MaxCreatureRotationSpeed { get; set; } = 2.0f;
    [Export] public int MinBrainHiddenLayers { get; set; } = 0;
    [Export] public int MaxBrainHiddenLayers { get; set; } = 5;
}