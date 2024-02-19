using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class GameManager : Node
{
	Main _main;
	UIPanel _panel;
	Creature selectedCreature;

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
