using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// Represents an Eye, a type of BodyPart that provides vision capabilities to a Creature.
/// </summary>
/// <remarks>
/// The Eye class is responsible for initializing and managing the vision area, based on the field of view (FOV), and detecting visible entities.
/// It also handles the activation of neurons based on the visible entities.
/// </remarks>
public partial class Eye : BodyPart
{

	#region Attributes

	/// <summary>
	/// The data related to the Eye, including its field of view, view distance, and other parameters.
	/// </summary>
	[Export]
	public EyeData _eyeData;

	/// <summary>
	/// The 3D area representing the field of view of the Eye.
	/// </summary>
	private Area3D _visionArea;

	/// <summary>
	/// A list of entities that are currently visible to the Eye.
	/// </summary>
	private List<VisibleEntityData> VisibleEntities = new();

	#endregion Attributes

	#region Parameters

	/// <summary>
	/// The time in seconds elapsed since the last update of the Eye.
	/// </summary>
	private double _sinceLastUpdate = 0;

	/// <summary>
	/// The vertical field of view of the Eye, in degrees.
	/// </summary>
	private float _FOVVertical;

	/// <summary>
	/// The horizontal field of view of the Eye, in degrees.
	/// </summary>
	private float _FOVHorizontal;

	/// <summary>
	/// The indexes of the neurons that are activated by the Eye.
	/// </summary>
	public int[] _neuronInputIndexes;


	#endregion Parameters

	#region Initialization

	public override void Initialize(Vector3 position, Vector3 rotation)
	{
		base.Type = BodyPartType.Eye;
		_FOVHorizontal = _eyeData.FOVHorizontal;
		_FOVVertical = _eyeData.FOVVertical;
		Creature creature = Utils.GetFirstParentOfType<Creature>(this);
		_brainRef = creature.GetBrain();
		_energyManager = creature._energyManager;
		creature._energyManager.AdjustMaxEnergy(_eyeData.EyeComplexity * _eyeData.ActivatorPerEntity * _eyeData.BaseEnergyMultiplaier);

		var eyeModel = AddModel(position, rotation);
		AddVision(eyeModel);

	}

	/// <summary>
	/// Sets up the vision capabilities for the eye.
	/// </summary>
	/// <param name="parent">The parent Node3D to which the vision area will be added.</param>
	/// <remarks>
	/// This method creates a new Area3D object representing the vision area, adds it as a child to the provided parent, and sets up event handlers for when an area enters or exits the vision area.
	/// It also calculates the optimal segment angle, verifies and fixes the field of view (FOV), generates points for the FOV mesh, and creates a convex collision shape for the mesh.
	/// </remarks>
	private void AddVision(Node3D parent)
	{
		_visionArea = new()
		{
			Name = "VisionArea",
			Monitoring = true,
			// Set collision layer to 2 (creature) only TODO: Make this a setting in a better way
			CollisionLayer = 2,
			// Set collision mask to 1 (food) TODO: Make this a setting in a better way
			CollisionMask = 1
		};
		parent.AddChild(_visionArea);

		float optimalSegmentAngle = GetOptimalSegmentAngle();
		bool needToTurn = VerifyAndFixFOV();
		Vector3[] points = GenerateFOVMeshPoints(optimalSegmentAngle);
		CreateConvexCollisionShapeForMesh(points, _visionArea, needToTurn);

		// _visionArea.AreaEntered += OnVisionAreaAreaEntered;
		// _visionArea.AreaExited += OnVisionAreaAreaExited;
	}

	private float GetOptimalSegmentAngle()
	{
		if (_FOVVertical < _FOVHorizontal)
		{
			return _FOVVertical / 2;
		}
		else
		{
			return _FOVHorizontal / 2;
		}
	}

	/// <summary>
	/// Verifies and adjusts the field of view (FOV) values for the Eye.
	/// </summary>
	/// <remarks>
	/// This method performs the following operations:
	/// 1) Clamps the FOV values between 1 and 180 degrees.
	/// 2) If the vertical FOV is larger than the horizontal FOV, it sets a flag indicating that the collision shape needs to be rotated around the Z-axis by 90 degrees.
	/// 3) If the vertical FOV is smaller or equal to the horizontal FOV, it swaps the vertical and horizontal FOV values.
	/// </remarks>
	/// <returns>
	/// A boolean value indicating whether the collision shape needs to be rotated around the Z-axis by 90 degrees.
	/// </returns>
	private bool VerifyAndFixFOV()
	{
		// in current version, Horizontal FOV sits in FOVVertical
		// and Vertical FOV is not calculated correctly, it limits both vertical and horizontal
		// happens due to the calculation of FOV, which starts from Vertical segments
		_FOVVertical = Mathf.Clamp(_FOVVertical, 1, 180);
		_FOVHorizontal = Mathf.Clamp(_FOVHorizontal, 1, 180);
		bool needToTurn = false;
		if (_FOVVertical > _FOVHorizontal)
		{
			needToTurn = true;
		}
		else
		{
			var tmp = _FOVVertical;
			_FOVVertical = _FOVHorizontal;
			_FOVHorizontal = tmp;
		}
		return needToTurn;
	}

	public void CreateConvexCollisionShapeForMesh(Vector3[] points, Area3D visionArea, bool turnZAxis)
	{
		// First, create a convex collision shape from points
		var convexShape = new ConvexPolygonShape3D();
		convexShape.Points = points;

		// Now, create a CollisionShape3D node and set the convex shape
		CollisionShape3D collisionShape = new CollisionShape3D();
		collisionShape.Shape = convexShape;

		// Optionally, rotate the collision shape around Z-axis
		if (turnZAxis)
		{
			collisionShape.RotationDegrees = new Vector3(0, 0, 90);
		}

		// Add the CollisionShape3D to the VisionArea
		visionArea.AddChild(collisionShape);
	}

	private Vector3[] GenerateFOVMeshPoints(float segmentAngle = 5)
	{
		List<Vector3> points = new List<Vector3>();
		points.Add(new Vector3(0, 0, 0)); // Center point

		// Calculate the number of segments based on the FOV and a reasonable angle step, like 5 degrees
		int verticalSegments = Mathf.CeilToInt(_FOVVertical / segmentAngle);
		int horizontalSegments = Mathf.CeilToInt(_FOVHorizontal / segmentAngle);

		// Iterate over the vertical and horizontal segments to generate points
		for (int v = 0; v <= verticalSegments; v++)
		{
			float verticalAngle = Mathf.DegToRad((v / (float)verticalSegments) * _FOVVertical - _FOVVertical / 2);
			for (int h = 0; h <= horizontalSegments; h++)
			{
				float horizontalAngle = Mathf.DegToRad((h / (float)horizontalSegments) * _FOVHorizontal - _FOVHorizontal / 2);

				// Calculate the point's position
				float x = _eyeData.ViewDistance * Mathf.Sin(verticalAngle) * Mathf.Cos(horizontalAngle);
				float y = _eyeData.ViewDistance * Mathf.Sin(verticalAngle) * Mathf.Sin(horizontalAngle);
				float z = _eyeData.ViewDistance * Mathf.Cos(verticalAngle);
				points.Add(new Vector3(x, y, -z)); // Note the negative z for forward direction
			}
		}
		return points.ToArray();
	}

	/// <summary>
	/// Adds a 3D model of the eye to the scene.
	/// </summary>
	/// <param name="position">The position where the model will be placed.</param>
	/// <param name="rotation">The rotation of the model.</param>
	/// <returns>Returns the Node3D instance of the model.</returns>
	/// <remarks>
	/// This method creates an instance of the model from the scene stored in _eyeData, sets its name, position, scale, and rotation, 
	/// and adds it as a child to the Eye instance. The model's name is generated by appending the instance ID to the name stored in _eyeData.
	/// </remarks>
	private Node3D AddModel(Vector3 position, Vector3 rotation)
	{
		return base.AddModel(_eyeData.Name, _eyeData.Scene, position, rotation, _eyeData.Scale, this);
	}

	#endregion Initialization

	#region Logic

	/// <summary>
	/// Processes the Eye's vision updates at each frame.
	/// </summary>
	/// <param name="delta">The time elapsed since the last frame.</param>
	/// <remarks>
	/// This method is called every frame and is responsible for updating the Eye's vision. 
	/// It checks if the time elapsed since the last update is greater than the specified update interval. 
	/// If so, it resets the timer, gets the visible entities, converts these entities into neuron activators, and activates the neurons.
	/// </remarks>
	public override void _Process(double delta)
	{
		_sinceLastUpdate += delta;
		if (_sinceLastUpdate > _eyeData.UpdateIntervalSeconds)
		{
			_sinceLastUpdate = 0;
			UpdateVisibleEntities();
			var activators = ConvertVisibleEntitiesToNeuronActivators();
			ActivateNeurons(activators);
			SpendEnergy();
		}
	}

	public override void SpendEnergy()
	{
		_energyManager.SpendEnergy(
			_energyManager.CalculateEnergyConsumptionEyeProcessing(
				_eyeData.EyeComplexity,
				_eyeData.ActivatorPerEntity,
				_eyeData.ViewDistance,
				_FOVVertical,
				_FOVHorizontal
			)
		);
	}

	private float[] ConvertVisibleEntitiesToNeuronActivators()
	{
		// Assuming VisibleEntities is sorted in descending order to get the biggest entities first.
		// If you want the biggest entities, you should use OrderByDescending instead of OrderBy.
		var procesingEntities = VisibleEntities.OrderByDescending(entity => entity.size).Take(_eyeData.EyeComplexity);

		// I expect up to maxActivators values per entity
		int maxActivators = typeof(VisibleEntityData).GetFields().Length;

		// If _eyeData.ActivatorPerEntity is less than maxActivators, this will not fill all types of data for each entity.
		int activatorsCount = Math.Min(maxActivators, _eyeData.ActivatorPerEntity);
		float[] activators = new float[procesingEntities.Count() * activatorsCount];
		int idx = 0;

		foreach (var entity in procesingEntities)
		{
			// Ensure we don't exceed the limit of _eyeData.ActivatorPerEntity
			if (activatorsCount > 0)
				activators[idx * activatorsCount + 0] = Utils.ScaleValue((int)entity.entityType, 0, Utils.GetCountOfEnumValues<EntityType>());
			if (activatorsCount > 1)
				activators[idx * activatorsCount + 1] = Utils.ScaleValue(entity.angle.Y, -_eyeData.FOVHorizontal / 2, _eyeData.FOVHorizontal / 2);
			if (activatorsCount > 2)
				activators[idx * activatorsCount + 2] = Utils.ScaleValue(entity.size, 0, 100);
			if (activatorsCount > 3)
				activators[idx * activatorsCount + 3] = Utils.ScaleValue(entity.distance, 0, _eyeData.ViewDistance);

			idx += 1;
		}
		return activators;
	}


	private void ActivateNeurons(float[] activators)
	{
		if (activators.Length > 0)
		{
			// Find the neurons ID's that are linked to this Eye
			// var neurons = _brainRef.NeuronsMap.FindAll(x => x.BodyPartLink == this).OrderBy(x => x.ID).Select(x => x.ID).ToArray();
			for (int i = 0; i < activators.Length; i++)
			{
				_brainRef.SetNeuronValue(_neuronInputIndexes[i], activators[i]);
			}
		}
	}

	/// <summary>
	/// Updates list of all entities visible to the creature within its vision area, stored in VisibleEntities.
	/// </summary>
	/// <remarks>
	/// This method first clears the list of visible entities. It then retrieves all overlapping areas within the creature's vision area.
	/// If an overlapping area is an entity that implements the IVisible interface, it is added to the list of visible entities.
	/// </remarks>
	private void UpdateVisibleEntities()
	{
		var overlaping = _visionArea.GetOverlappingAreas();
		VisibleEntities.Clear();
		foreach (var area in overlaping)
		{
			if (area is IVisible visible)
			{
				var data = visible.GetEntityData();
				data.angle = CalculateAngleBetweenCreatureAndEntity(visible);
				data.distance = (visible as Node3D).GlobalTransform.Origin.DistanceTo(this.GlobalTransform.Origin);
				//GD.Print($"Eye {Name} sees {(visible as Node3D).Name} at {data.angle} degrees");
				VisibleEntities.Add(data);
			}
		}
	}

	/// <summary>
	/// Calculates the angle between the creature and a visible entity.
	/// </summary>
	/// <param name="visible">The visible entity to calculate the angle to.</param>
	/// <returns>A Vector3 where the Y component represents the angle in degrees between the creature's forward direction and the direction to the visible entity. Positive values indicate the entity is to the right of the creature, negative values indicate it is to the left.</returns>
	private Vector3 CalculateAngleBetweenCreatureAndEntity(IVisible visible)
	{
		Vector3 creaturePosition = Utils.GetFirstParentOfType<Creature>(this).GlobalTransform.Origin;
		Vector3 objectPosition = (visible as Node3D).GlobalTransform.Origin;

		// Calculate the direction vector from the creature to the object
		Vector3 directionToObject = (objectPosition - creaturePosition).Normalized();

		// Assuming the creature's forward vector is along its local negative Z-axis
		Vector3 creatureForward = Utils.GetFirstParentOfType<Creature>(this).GlobalTransform.Basis.Z.Normalized() * -1;

		// Calculate the angle between the vectors
		float dotProduct = creatureForward.Dot(directionToObject);
		float angle = Mathf.Acos(dotProduct);

		// Use the cross product to determine if the object is to the left or right
		Vector3 crossProduct = creatureForward.Cross(directionToObject);

		// If crossProduct.y is positive, the object is to the left, otherwise it's to the right
		if (crossProduct.Y > 0)
		{
			angle = -angle;
		}

		// Convert the angle to degrees if necessary
		float angleDegrees = Mathf.RadToDeg(angle);

		// angleDegrees now holds the signed angle in degrees
		return new(0, angleDegrees, 0);
	}

	#endregion Logic

	#region Signals

	private void OnVisionAreaAreaEntered(Area3D area)
	{
		// if (area is IConsumable consumable)
		// {
		// 	var MyBrain = NodeUtilities.GetFirstParentOfType<Creature>(this).GetBrain().ToString();
		// 	GD.Print($"{VisionArea.Name} spotted a {area.Name}\nMyBrain: {MyBrain}");
		// }
	}


	private void OnVisionAreaAreaExited(Area3D area)
	{
		// if (area is IConsumable consumable)
		// {
		// 	GD.Print($"{VisionArea.Name} no longer sees {area.Name}");
		// }
	}

	#endregion Signals

}
