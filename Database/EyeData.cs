using Godot;

/// <summary>
/// Represents an EyeData object that contains information about the eye.
/// </summary>
/// <remarks>
/// This class is used to store properties related to the eye, such as its name, view distance, field of view, scale, and update interval.
/// </remarks>
/// <seealso cref="Godot.Resource">Godot.Resource</seealso>
[GlobalClass]
public partial class EyeData : Resource
{
	[Export] public string Name { get; set; }
	[Export] public PackedScene Scene;
	[Export] public float ViewDistance { get; set; }
	[Export] public float FOVHorizontal { get; set; }
	[Export] public float FOVVertical { get; set; }
	[Export] public Vector3 Scale { get; set; }

	/// <summary>
	/// Interval in seconds between each data retrieval for the eye.
	/// </summary>
	[Export] public double UpdateIntervalSeconds { get; set; } = 1;

	/// <summary>
	/// The complexity of the Eye, which affect number of objects that the Eye can process at the same time.
	/// </summary>
	[Export] public int EyeComplexity { get; set; } = 10;

	/// <summary>
	/// The number of activators the Eye can process for each entity that it detects.
	/// </summary>
	[Export] public int ActivatorPerEntity { get; set; } = 3;

	[Export] public float BaseEnergyMultiplaier { get; set; } = 100;

}
