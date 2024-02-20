
using Godot;

public partial class SimulationParameters : Node
{
    [Export] public int InitialPopulationSize { get; set; } = 100;
    [Export] public int KeepFromPopulationPercent { get; set; } = 10;
    [Export] public int MutateFromPopulationPercent { get; set; } = 30;
    [Export] public int ChangeBrainFromPopulationPercent { get; set; } = 30;
    [Export] public int RandomInPopulationPercent { get; set; } = 30;
    [Export] public int TimePerGenerationSeconds { get; set; } = 20;
    [Export] public int MaxGenerations { get; set; } = 10;
}