
using Godot;

public partial class SimulationParameters : Node
{
    [Export] public int InitialPopulationSize { get; set; } = 100;
    [Export] public int TimePerGenerationSeconds { get; set; } = 20;
}