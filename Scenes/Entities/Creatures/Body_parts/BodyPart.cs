using Godot;

public partial class BodyPart : Node3D
{
	[Export] public BodyPartType Type { get; set; }
	[Export] public Vector3 Angle { get; set; }

	/// <summary>
	/// Reference to the Brain of the creature to which the Eye belongs.
	/// </summary>
	public Brain _brainRef;


	public Node3D AddModel(string name, PackedScene scene, Vector3 position, Vector3 rotation, Vector3 scale, Node3D parent)
	{
		Node3D Model = scene.Instantiate() as Node3D;
		Model.Name = $"{name}_Model_{Model.GetInstanceId()}";
		parent.AddChild(Model);

		Model.Position = position;
		Model.Scale = scale;
		Model.RotationDegrees = rotation;
		return Model;
	}

	public virtual void Initialize(Vector3 position, Vector3 rotation)
	{

	}

	public virtual void SpendEnergy()
	{

	}

}


