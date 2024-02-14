using System.Collections.Generic;
using Godot;

public partial class GameManager : Node
{
	Main _main;
	Control _panel;
	Creature selectedCreature;

	public void SetSelectedCreature(Creature creature)
	{
		selectedCreature = creature;
		GD.Print($"Creature: {selectedCreature.Name} selected.");
	}

	public override void _Ready()
	{
		base._Ready();
		_main = GetParent<Main>();
		_panel = _main.GetNode<Control>("Panel");
		_panel.Visible = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

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
			// Engine.TimeScale *= 2;
		}
		if (Input.IsActionJustPressed("decrease_time_scale"))
		{
			// Engine.TimeScale /= 2;
		}
		if (Input.IsActionJustPressed("Action1"))
		{
			DNA myDna = new()
			{
				BodyData = new DNA.BodyStruct { Type = "Type1", Size = 1.0f },
				BrainData = new DNA.BrainStruct { Complexity = 5, NumLayers = 3 },
				EyesData = new List<DNA.EyeStruct>
					{
						new() { ID = 1, Angle = new Vector3(0, 1, 2) }
					},
				MouthsData = new List<DNA.MouthStruct>
					{
						new() { ID = 1, Angle = new Vector3(0, 1, 2) }
					},
				Brain = selectedCreature.GetBrain()
			};

			string encodedDna = DNASerializer.EncodeDNA(myDna);
			GD.Print($"Encoded DNA: {encodedDna}");

		}
		if (Input.IsActionJustPressed("Action2"))
		{
			int i = -180;
			var s = DNASerializer.IntToBinaryString(i);
			GD.Print($" {i} = {s} in binary");
			i = DNASerializer.BinaryStringToInt(s);
			GD.Print($" {s} = {i} as int");
			i = 18000;
			s = DNASerializer.IntToBinaryString(i);
			GD.Print($" {i} = {s} in binary");
			i = DNASerializer.BinaryStringToInt(s);
			GD.Print($" {s} = {i} as int");
			float f = -1.12311231231234f;
			s = DNASerializer.FloatToBinaryString(f, 8);
			GD.Print($" {f} = {s} in binary");
			var nf = DNASerializer.BinaryStringToFloat(s);
			GD.Print($" {s} = {nf} as float");
			GD.Print($" difference = {nf - f} as float");
			f = 0.12311231231234f;
			s = DNASerializer.FloatToBinaryString(f, 8);
			GD.Print($" {f} = {s} in binary");
			nf = DNASerializer.BinaryStringToFloat(s);
			GD.Print($" {s} = {f} as float");
			GD.Print($" difference = {nf - f} as float");

		}
		if (Input.IsActionJustPressed("Action3"))
		{

		}
		if (Input.IsActionJustPressed("Action4"))
		{

		}

	}
}
