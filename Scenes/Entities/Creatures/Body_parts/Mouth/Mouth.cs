using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents a mouth body part of a creature.
/// </summary>
public partial class Mouth : BodyPart
{
	/// <summary>
	/// The data associated with the mouth.
	/// </summary>
	[Export] public MouthData _mouthData;

	private Area3D _mouthReachArea;

	/// <summary>
	/// The indexes of the neurons that are activated by the Mouth.
	/// </summary>
	public int[] _neuronInputIndexes;

	#region Initialization

	/// <summary>
	/// Initializes the mouth body part.
	/// </summary>
	/// <param name="position">The position of the mouth.</param>
	/// <param name="rotation">The rotation of the mouth.</param>
	public override void Initialize(Vector3 position, Vector3 rotation)
	{
		base.Type = BodyPartType.Mouth;
		Creature creature = Utils.GetFirstParentOfType<Creature>(this);
		_brainRef = creature.GetBrain();
		_energyManager = creature._energyManager;
		creature._energyManager.AdjustMaxEnergy(_mouthData.OutputNeurons * _mouthData.BaseEnergyMultiplaier);

		rotation += _mouthData.Rotation;
		AddModel(position, rotation);
		AddMouthReachArea(position);

	}

	/// <summary>
	/// Adds the mouth reach area to the mouth body part.
	/// </summary>
	/// <param name="position">The position of the Area3D node, defining pivot point of mouth reach area.</param>
	private void AddMouthReachArea(Vector3 position)
	{
		_mouthReachArea = new()
		{
			Name = "MouthReachArea",
			Monitoring = true,
			// Set collision layer to 2 (creature) only TODO: Make this a setting in a better way
			CollisionLayer = 2,
			// Set collision mask to 1 (food) TODO: Make this a setting in a better way
			CollisionMask = 1
		};

		Vector3[] points = GenerateMouthReachMeshPoints();
		var shape = CreateConvexCollisionShapeForMesh(points, _mouthReachArea);
		shape.Position += position;

		_mouthReachArea.AreaEntered += OnMouthReachAreaEntered;
		// _mouthReachArea.AreaExited += OnMouthReachAreaExited;

		AddChild(_mouthReachArea);
	}

	/// <summary>
	/// Adds a model to the mouth body part.
	/// </summary>
	/// <param name="position">The position of the model.</param>
	/// <param name="rotation">The rotation of the model.</param>
	private void AddModel(Vector3 position, Vector3 rotation)
	{
		AddModel(_mouthData.Name, _mouthData.Scene, position, rotation, _mouthData.Scale, this);
	}

	/// <summary>
	/// Generates the points for the mouth reach mesh.
	/// </summary>
	/// <returns>The generated points.</returns>
	private Vector3[] GenerateMouthReachMeshPoints()
	{
		List<Vector3> points = new List<Vector3>();

		// Half sizes
		float halfX = _mouthData.Size.X / 2.0f;
		float halfY = _mouthData.Size.Y / 2.0f;
		// For Z, since the pivot is at the center of the back side, the back vertices are at Z = 0
		float backZ = 0;
		// And the front vertices are offset by the full size.z from the back
		float frontZ = -_mouthData.Size.Z;

		// Calculate the positions of the 8 vertices of the cube
		points.Add(new Vector3(-halfX, -halfY, backZ)); // Bottom-back-left
		points.Add(new Vector3(halfX, -halfY, backZ));  // Bottom-back-right
		points.Add(new Vector3(halfX, -halfY, frontZ)); // Bottom-front-right
		points.Add(new Vector3(-halfX, -halfY, frontZ)); // Bottom-front-left
		points.Add(new Vector3(-halfX, halfY, backZ));  // Top-back-left
		points.Add(new Vector3(halfX, halfY, backZ));   // Top-back-right
		points.Add(new Vector3(halfX, halfY, frontZ));  // Top-front-right
		points.Add(new Vector3(-halfX, halfY, frontZ)); // Top-front-left

		return points.ToArray();

	}

	/// <summary>
	/// Creates a convex collision shape for the mouth reach mesh.
	/// </summary>
	/// <param name="points">The points of the mesh.</param>
	/// <param name="mouthReachArea">The mouth reach area.</param>
	/// <returns>The created collision shape.</returns>
	private CollisionShape3D CreateConvexCollisionShapeForMesh(Vector3[] points, Area3D mouthReachArea)
	{
		// First, create a convex collision shape from points
		var convexShape = new ConvexPolygonShape3D();
		convexShape.Points = points;

		// Now, create a CollisionShape3D node and set the convex shape
		CollisionShape3D collisionShape = new CollisionShape3D();
		collisionShape.Shape = convexShape;

		// Add the CollisionShape3D to the VisionArea
		mouthReachArea.AddChild(collisionShape);
		return collisionShape;
	}

	#endregion Initialization

	#region Logic

	/// <summary>
	/// Handles the event when an Area3D object enters the mouth reach area.
	/// </summary>
	/// <param name="area">The entered area.</param>
	private void OnMouthReachAreaEntered(Area3D area)
	{
		if (area is IVisible visible)
		{
			var data = visible.GetEntityData();
			if (_mouthData.Diet == DietType.Herbivore)
			{
				if (visible is IConsumable consumable)
				{
					var energy = consumable.Consume();
					_energyManager.AddEnergy(energy);
					// GD.Print($"Creature {creature.Name} consumed {energy} energy from {data.entityType} {area.Name}");
				}
			}
			else if (_mouthData.Diet == DietType.Carnivore)
			{
				// TODO: Implement carnivore mouth
			}
		}
	}

	#endregion Logic


}
