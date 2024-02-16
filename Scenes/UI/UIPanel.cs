using Godot;
using System;

public partial class UIPanel : Panel
{
    [Export] public Label _energyLabel;
    [Export] public Label _creatureLabel;
    Creature selectedCreature;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (selectedCreature != null)
        {
            UpdateEnergy();
        }
    }

    public void SetCreature(Creature creature)
    {
        selectedCreature = creature;
        UpdateCreature();
    }

    void UpdateEnergy()
    {
        _energyLabel.Text = $"Energy: {selectedCreature._energyManager.CurrentEnergy} / {selectedCreature._energyManager.MaxEnergy}";
    }

    void UpdateCreature()
    {
        _creatureLabel.Text = $"Selected: {selectedCreature.Name}";
    }
}
