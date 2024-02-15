using Godot;

/// <summary>
/// Represents an MouthData object that contains information about the mouth.
/// </summary>
/// <remarks>
/// This class is used to store properties related to the eye, such as its name, scale, and diet type.
/// </remarks>
/// <seealso cref="Godot.Resource">Godot.Resource</seealso>
[GlobalClass]
public partial class MouthData : Resource
{
	[ExportCategory("Main Category")]
	[Export] public string Name { get; set; }
	[Export] public DietType Diet { get; set; }
	[ExportCategory("Model Category")]
	[Export] public PackedScene Scene;
	[Export] public Vector3 Rotation { get; set; }
	[Export] public Vector3 Scale { get; set; }
	[ExportCategory("Area Category")]
	[Export] public Vector3 Size { get; set; }
	[Export] public int OutputNeurons { get; set; }

}
