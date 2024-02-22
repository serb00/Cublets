using System.Collections.Generic;
using Godot;
using System;
using System.Linq;

/// <summary>
/// Represents a creature in the game.
/// </summary>
/// <remarks>
/// This class extends the Node3D class and implements the IVisible interface.
/// It represents a creature entity in the game world, with a brain and a body.
/// The creature can have multiple body parts, such as eyes, which are stored in the _bodyParts list.
/// </remarks>
public partial class Creature : CharacterBody3D, IVisible
{

    #region Parameters

    double updateBrainIntervalSeconds = 0.5;
    double secondsSinceLastBrainUpdate = 1; // so the brain updates on the first frame
    [Export] int minHiddenLayers = 0;
    [Export] int maxHiddenLayers = 2;

    [Export] float MaxSpeedPerSecond = 1000; // m/s
    [Export] float MaxRotationPerSecond = 90; // degrees


    #endregion Parameters

    #region Attributes

    Brain _brain = new();
    Body _body;
    public EnergyManager _energyManager;
    public DNA _dna;
    // TODO: movethis to reference and the building logic to other class Builder from DNA
    private BodyPartsCollection _bodyPartsCollection;

    /// <summary>
    /// List of body parts that the creature has.
    /// </summary>
    public List<BodyPart> _bodyParts = new();

    [Export] Node3D ParentForBodyParts;

    [Export] public PackedScene _bodyScene;

    readonly List<BrainZone> InputNeuronsList = new();
    readonly List<BrainZone> OutputNeuronsList = new();

    private Vector3 _lastPosition;
    private float _lastRotationY;
    public float Fitness { get; set; }

    #endregion Attributes

    #region Constructors

    public void Initialize(Vector3 position, DNA dna)
    {
        _dna = dna;
        Position = position;
        _bodyPartsCollection = (BodyPartsCollection)Engine.GetSingleton("BodyPartsCollection");

        //prefill body parts list
        CreateBodyPartsListFromDNA();
        _body = _bodyScene.Instantiate() as Body;
        _energyManager = new EnergyManager(0, _body.GetSize());
        var bodyData = (BodyData)ResourceLoader.Load(_bodyPartsCollection.GetBodyPartResourseOfTypeByIndex(_dna.BodyGene.Type, _dna.BodyGene.ID));
        _body.Initialize(this, bodyData);
        AddChild(_body);
        InitializeBodyParts();
        InitializeBrain();
        ConnectBrainToBodyParts();
        _energyManager.MaximizeEnergy();
    }

    private void ConnectBrainToBodyParts()
    {
        foreach (BodyPart bodyPart in _bodyParts)
        {
            bodyPart._brainRef = _brain;
        }
    }

    private void CreateBodyPartsListFromDNA()
    {
        int currentNeuronIndex = 0;
        int brainInputNeurons = 0;
        int brainOutputNeurons = 0;

        // Eyes
        foreach (var eyeGene in _dna.EyesGene)
        {
            string path = _bodyPartsCollection.GetBodyPartResourseOfTypeByIndex(BodyPartType.Eye, 0);
            PackedScene eyeScene = (PackedScene)ResourceLoader.Load(path);
            path = _bodyPartsCollection.GetBodyPartResourseOfTypeByIndex(BodyPartType.Eye, eyeGene.ID);
            EyeData eyeData = (EyeData)ResourceLoader.Load(path);
            var newEye = CreateEye(
                eyeScene,
                eyeData,
                eyeGene.Angle,
                currentNeuronIndex,
                out currentNeuronIndex
            );
            _bodyParts.Add(newEye);

            InputNeuronsList.Add(new BrainZone(BrainZoneType.Visual, currentNeuronIndex - brainInputNeurons, newEye));
            brainInputNeurons = currentNeuronIndex;

        }

        // Movement
        OutputNeuronsList.Add(new BrainZone(BrainZoneType.Movement, 2));
        brainOutputNeurons += 2;

        // Mouths
        foreach (var mouthGene in _dna.MouthsGene)
        {
            var path = _bodyPartsCollection.GetBodyPartResourseOfTypeByIndex(BodyPartType.Mouth, 0);
            PackedScene mouthScene = (PackedScene)ResourceLoader.Load(path);
            path = _bodyPartsCollection.GetBodyPartResourseOfTypeByIndex(BodyPartType.Mouth, mouthGene.ID);
            MouthData mouthData = (MouthData)ResourceLoader.Load(path);
            var newMouth = CreateMouth(
                mouthScene,
                mouthData,
                mouthGene.Angle,
                currentNeuronIndex,
                out currentNeuronIndex
            );
            _bodyParts.Add(newMouth);

            InputNeuronsList.Add(new BrainZone(BrainZoneType.Consumption, currentNeuronIndex - brainInputNeurons, newMouth));
            brainInputNeurons = currentNeuronIndex;
            OutputNeuronsList.Add(new BrainZone(BrainZoneType.Consumption, mouthData.OutputNeurons, newMouth));
            brainOutputNeurons += mouthData.OutputNeurons;
        }

    }

    private Eye CreateEye(PackedScene eyeScene, EyeData eyeData, Vector3 angle, int currentNeuronIndex, out int neuronLastIndex)
    {
        Eye eye = eyeScene.Instantiate() as Eye;
        eye.Name = $"Eye_{eye.GetInstanceId()}";
        eye.Angle = angle;
        eye._eyeData = eyeData;
        var neuronsNum = eye._eyeData.EyeComplexity * eye._eyeData.ActivatorPerEntity;
        eye._neuronInputIndexes = new int[neuronsNum];
        for (int i = 0; i < neuronsNum; i++)
        {
            eye._neuronInputIndexes[i] = currentNeuronIndex++;
        }
        neuronLastIndex = currentNeuronIndex;
        return eye;
    }

    private Mouth CreateMouth(PackedScene mouthScene, MouthData mouthData, Vector3 angle, int currentNeuronIndex, out int neuronLastIndex)
    {
        Mouth mouth = mouthScene.Instantiate() as Mouth;
        mouth.Name = $"Mouth_{mouth.GetInstanceId()}";
        mouth.Angle = angle;
        mouth._mouthData = mouthData;
        mouth._neuronInputIndexes = new int[1];
        mouth._neuronInputIndexes[0] = currentNeuronIndex++;

        neuronLastIndex = currentNeuronIndex;
        return mouth;
    }

    private void InitializeBrain()
    {

        _brain.Initialize(InputNeuronsList, OutputNeuronsList, GD.RandRange(minHiddenLayers, maxHiddenLayers), 1, this);
    }

    private void InitializeBodyParts()
    {

        string tempString = (string)_body.GetShape().ToString();
        string shapeName = tempString.Substring(1, tempString.Find("Shape3D") - 1);

        switch (shapeName)
        {

            case "Box":
                //_body.GetSize() / 2
                ParentForBodyParts.Position = new Vector3(0, _body.GetSize() / 2, 0);
                foreach (var part in _bodyParts)
                {
                    ParentForBodyParts.AddChild(part);
                    part.Initialize(
                        GetBoxShapeBodyPartPosition(part.Angle, _body.GetScale()),
                        GetBoxShapeBodyPartRotation(part.Angle, _body.GetScale()));
                }
                break;
            default:
                break;
        }
    }

    private Vector3 GetBoxShapeBodyPartPosition(Vector3 angle, Vector3 cubeScale)
    {
        // Step 1: Normalize the angle vector
        Vector3 direction = angle.Normalized();

        // Step 2: Adjust direction by inverse cube scale to identify face correctly
        Vector3 adjustedDirection = new Vector3(
            direction.X / cubeScale.X,
            direction.Y / cubeScale.Y,
            direction.Z / cubeScale.Z).Normalized();



        // Step 3: Find the predominant axis (largest absolute value in adjustedDirection)
        if (Math.Abs(adjustedDirection.X) > Math.Abs(adjustedDirection.Y) && Math.Abs(adjustedDirection.X) > Math.Abs(adjustedDirection.Z))
        {
            // Determine base position on Y-axis face
            float baseX = Math.Sign(adjustedDirection.X) * cubeScale.X / 2;
            // Apply offset based on the X and Y components of the angle
            float offsetY = adjustedDirection.Y * cubeScale.Y / 2;
            float offsetZ = adjustedDirection.Z * cubeScale.Z / 2;
            return new Vector3(baseX, offsetY, offsetZ);

        }
        else if (Math.Abs(adjustedDirection.Y) > Math.Abs(adjustedDirection.Z))
        {
            // Determine base position on Y-axis face
            float baseY = Math.Sign(adjustedDirection.Y) * cubeScale.Y / 2;
            // Apply offset based on the X and Y components of the angle
            float offsetX = adjustedDirection.X * cubeScale.X / 2;
            float offsetZ = adjustedDirection.Z * cubeScale.Z / 2;
            return new Vector3(offsetX, baseY, offsetZ);
        }
        else
        {
            // Determine base position on Z-axis face
            float baseZ = Math.Sign(adjustedDirection.Z) * cubeScale.Z / 2;
            // Apply offset based on the X and Y components of the angle
            float offsetX = adjustedDirection.X * cubeScale.X / 2;
            float offsetY = adjustedDirection.Y * cubeScale.Y / 2;
            return new Vector3(offsetX, offsetY, baseZ);
        }
    }

    private Vector3 GetBoxShapeBodyPartRotation(Vector3 angle, Vector3 cubeScale)
    {
        // Step 1: Normalize the angle vector
        Vector3 direction = angle.Normalized();

        // Step 2: Adjust direction by inverse cube scale to identify face correctly
        Vector3 adjustedDirection = new Vector3(
            direction.X / cubeScale.X,
            direction.Y / cubeScale.Y,
            direction.Z / cubeScale.Z).Normalized();

        // Step 3: Find the predominant axis (largest absolute value in adjustedDirection)
        if (Math.Abs(adjustedDirection.X) > Math.Abs(adjustedDirection.Y) && Math.Abs(adjustedDirection.X) > Math.Abs(adjustedDirection.Z))
        {
            // On X-axis face
            // return new Vector3(Math.Sign(adjustedDirection.X), 0, 0);
            return new Vector3(0, Math.Sign(adjustedDirection.X) > 0 ? -90 : 90, 0);
        }
        else if (Math.Abs(adjustedDirection.Y) > Math.Abs(adjustedDirection.Z))
        {
            // On Y-axis face
            // return new Vector3(0, Math.Sign(adjustedDirection.Y), 0);
            return new Vector3(Math.Sign(adjustedDirection.Y) > 0 ? 90 : -90, 0, 0);
        }
        else
        {
            // On Z-axis face
            // return new Vector3(0, 0, Math.Sign(adjustedDirection.Z));
            return new Vector3(0, Math.Sign(adjustedDirection.Z) > 0 ? 180 : 0, 0);
        }
    }

    #endregion Constructors

    #region Getters

    public Brain GetBrain()
    {
        return _brain;
    }

    public VisibleEntityData GetEntityData()
    {
        return new VisibleEntityData
        {
            entityType = EntityType.Creature,
            size = _body.GetSize(),
        };
    }

    #endregion Getters

    #region Setters

    public void SetBrain(Brain brain)
    {
        _brain = brain;
    }

    #endregion Setters


    #region Logic

    public override void _PhysicsProcess(double delta)
    {
        if (_energyManager.CurrentEnergy <= 0)
        {
            DisableCharacter();
            return;
        }

        var movementNeurons = _brain.NeuronsMap.FindAll(x => x.Type == BrainZoneType.Movement).OrderBy(x => x.ID).Select(x => x.ID).ToArray();
        var velocityNeuron = _brain.GetNeuron(movementNeurons[0]);
        var rotationNeuron = _brain.GetNeuron(movementNeurons[1]);

        // Update last position and rotation for difference calculations
        _lastPosition = Position;
        _lastRotationY = Rotation.Y;

        // Calculate the desired movement speed based on neuron output and delta
        var MovementSpeed = velocityNeuron * MaxSpeedPerSecond * (float)delta;

        // Calculate rotation adjustment factor based on Engine.TimeScale
        // This tries to adjust the rotation to be proportional to the change in speed
        var rotationAdjustmentFactor = Engine.TimeScale;

        // Calculate the adjusted rotational radians, compensating for changes in speed
        var RotateRadians = Mathf.DegToRad(rotationNeuron * MaxRotationPerSecond * (float)delta * rotationAdjustmentFactor);

        // Apply the adjusted rotation before calculating the forward vector
        RotateY((float)RotateRadians);

        // Now calculate the forward direction based on the current orientation
        // This needs to take into account the creature's global orientation, which includes the applied rotation
        var globalOrientation = GlobalTransform.Basis.GetEuler();
        var forwardDir = new Vector3(Mathf.Sin(globalOrientation.Y), 0, Mathf.Cos(globalOrientation.Y)).Normalized();

        // Apply the calculated velocity based on the forward direction
        Velocity = forwardDir * MovementSpeed;

        // Finally, apply the movement
        MoveAndSlide();

        // Calculate energy consumption
        float distanceTraveled = Position.DistanceTo(_lastPosition);
        float angleRotated = Mathf.RadToDeg(Mathf.Abs(Rotation.Y - _lastRotationY));

        // Reduce energy based on distance and rotation
        _energyManager.SpendEnergy(
            _energyManager.CalculateEnergyConsumptionMovement(distanceTraveled) +
            _energyManager.CalculateEnergyConsumptionRotation(angleRotated)
        );
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        secondsSinceLastBrainUpdate += delta;
        if (secondsSinceLastBrainUpdate > updateBrainIntervalSeconds)
        {
            _brain.UpdateBrain();
            secondsSinceLastBrainUpdate = 0f;
            _energyManager.SpendEnergy(
                _energyManager.CalculateEnergyConsumptionBrainProcessing(
                    _brain.GetSignalPasses(),
                    _brain.GetNeuronConnectionsCount(),
                    _brain.GetNeuronsCount()
                )
            );
        }
    }

    public void DisableCharacter()
    {
        SetProcess(false);
        SetPhysicsProcess(false);
        Hide();
        CollisionLayer = 0; // Disables collision layer 2 for the object itself
        CollisionMask = 0; // Disables collision layers 1 and 2 for detection
    }

    public void EnableCharacter(Vector3 position)
    {
        SetProcess(true);
        SetPhysicsProcess(true);
        Show();
        CollisionLayer = 1 << 1; // Enables layer 2 for the object itself
        CollisionMask = 1 | (1 << 1); // Enables layers 1 and 2 for detection
        Position = position;
        _energyManager.Reset();
    }

    #endregion Logic

}
