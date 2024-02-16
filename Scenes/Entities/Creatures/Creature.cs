using System.Collections.Generic;
using Godot;
using System;

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
    double secondsSinceLastBrainUpdate = 0;

    #endregion Parameters

    #region Attributes

    readonly Brain _brain = new();
    Body _body;
    // [Export] public PackedScene[] _bodyParts;

    /// <summary>
    /// List of body parts that the creature has.
    /// </summary>
    public List<BodyPart> _bodyParts = new();

    [Export] Node3D ParentForBodyParts;

    [Export] public PackedScene _bodyScene;

    readonly List<BrainZone> InputNeuronsList = new();
    readonly List<BrainZone> OutputNeuronsList = new();

    #endregion Attributes

    #region Constructors

    public void Initialize(Vector3 position)
    {
        Position = position;

        //prefill body parts list
        CreateBodyPartsList();

        Body bodyScene = _bodyScene.Instantiate() as Body;
        _body = bodyScene;
        bodyScene.Initialize(ParentForBodyParts);
        AddChild(bodyScene);
        InitializeBodyParts();
        InitializeBrain();
        ConnectBrainToBodyParts();
    }

    private void ConnectBrainToBodyParts()
    {
        foreach (BodyPart bodyPart in _bodyParts)
        {
            bodyPart._brainRef = _brain;
        }
    }

    private void CreateBodyPartsList()
    {
        int currentNeuronIndex = 0;
        int brainInputNeurons = 0;
        int brainOutputNeurons = 0;
        // Eyes
        PackedScene eyeScene = (PackedScene)ResourceLoader.Load("res://Scenes/Entities/Creatures/Body_parts/Eye/Eye.tscn"); ;
        EyeData eyeData = (EyeData)ResourceLoader.Load("res://Scenes/Entities/Creatures/Body_parts/Eye/Eyes/Eye_01.tres");
        var eye = CreateEye(
            eyeScene,
            eyeData,
            new(0, 0.5f, -1),
            currentNeuronIndex,
            out currentNeuronIndex
        );
        _bodyParts.Add(eye);

        InputNeuronsList.Add(new BrainZone(BrainZoneType.Visual, currentNeuronIndex - brainInputNeurons, eye));
        brainInputNeurons = currentNeuronIndex;

        // CreateEye(
        //     eyeScene,
        //     eyeData,
        //     new(1, 0, 0),
        //     currentNeuronIndex,
        //     out currentNeuronIndex
        // );

        // eyeData = (EyeData)ResourceLoader.Load("res://Scenes/Entities/Creatures/Body_parts/Eye/Eyes/Eye_02.tres");
        // currentNeuronIndex = CreateEye(
        //     eyeScene,
        //     eyeData,
        //     new(-1, 0.25f, 0),
        //     currentNeuronIndex
        // );


        // Movement
        OutputNeuronsList.Add(new BrainZone(BrainZoneType.Movement, 2));
        brainOutputNeurons += 2;

        // Mouths
        PackedScene mouthScene = (PackedScene)ResourceLoader.Load("res://Scenes/Entities/Creatures/Body_parts/Mouth/Mouth.tscn");
        MouthData mouthData = (MouthData)ResourceLoader.Load("res://Scenes/Entities/Creatures/Body_parts/Mouth/Mouths/Mouth_001.tres");
        var mouth = CreateMouth(
            mouthScene,
            mouthData,
            new(0, -0.5f, -1),
            currentNeuronIndex,
            out currentNeuronIndex
        );
        _bodyParts.Add(mouth);

        InputNeuronsList.Add(new BrainZone(BrainZoneType.Consumption, currentNeuronIndex - brainInputNeurons, mouth));
        brainInputNeurons = currentNeuronIndex;
        OutputNeuronsList.Add(new BrainZone(BrainZoneType.Consumption, mouthData.OutputNeurons, mouth));
        brainOutputNeurons += mouthData.OutputNeurons;
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

        _brain.Initialize(InputNeuronsList, OutputNeuronsList, GD.RandRange(0, 5), 1);
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

    public EntityData GetEntityData()
    {
        return new EntityData
        {
            entityType = EntityType.Creature,
            size = _body.GetSize(),
        };
    }

    #endregion Getters

    public override void _PhysicsProcess(double delta)
    {
        var velocityNeuron = _brain.GetNeuron(_brain.NumTotalNeurons - _brain.NumOutputNeurons);
        var rotationNeuron = _brain.GetNeuron(_brain.NumTotalNeurons - _brain.NumOutputNeurons + 1);

        Velocity = Vector3.Forward * velocityNeuron * 20;
        RotateY(rotationNeuron / 20);
        Velocity = Velocity.Rotated(Vector3.Up, Rotation.Y);

        MoveAndSlide();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        secondsSinceLastBrainUpdate += delta;
        if (secondsSinceLastBrainUpdate > updateBrainIntervalSeconds)
        {
            _brain.UpdateBrain();
            secondsSinceLastBrainUpdate = 0f;
        }
    }
}
