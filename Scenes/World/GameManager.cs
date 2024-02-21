using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class GameManager : Node
{
	Main _main;
	UIPanel _panel;
	Creature selectedCreature;

	string tempString;

	public override void _Ready()
	{
		base._Ready();
		InitializeVariables();
	}

	private void InitializeVariables()
	{
		_main = GetParent<Main>();
		_panel = _main.GetNode<Control>("Panel") as UIPanel;
		_panel.Visible = false;
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
			GD.Print($"TimeScale: {Engine.TimeScale}");
		}
		if (Input.IsActionJustPressed("decrease_time_scale"))
		{
			Engine.TimeScale /= 2;
			GD.Print($"TimeScale: {Engine.TimeScale}");
		}
		if (Input.IsActionJustPressed("Action1"))
		{
			if (selectedCreature == null)
			{
				GD.Print("No creature selected to copy the brain.");
				return;
			}
			tempString = Utils.EncodeObject(selectedCreature.GetBrain());
			GD.Print("Brain encoded and copied.");
		}
		if (Input.IsActionJustPressed("Action2"))
		{
			if (tempString == null)
			{
				GD.Print("No brain to paste.");
				return;
			}
			var brain = Utils.DecodeObject<Brain>(tempString);
			selectedCreature.SetBrain(brain);
			GD.Print($"Brain of {selectedCreature.Name} changed from a copy.");
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
