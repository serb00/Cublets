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
    Brain _brain;
    Body _body;
    // [Export] public PackedScene[] _bodyParts;

    /// <summary>
    /// List of body parts that the creature has.
    /// </summary>
    public List<BodyPart> _bodyParts = new();

    [Export] Node3D ParentForBodyParts;

    [Export] public PackedScene _bodyScene;

    private int brainInputNeurons;
    private int brainOutputNeurons;
    private int brainTotalNeurons;

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
        brainInputNeurons = 0;
        brainOutputNeurons = 0;
        // Eyes
        PackedScene eyeScene = (PackedScene)ResourceLoader.Load("res://Scenes/Entities/Creatures/Body_parts/Eye/Eye.tscn"); ;
        EyeData eyeData = (EyeData)ResourceLoader.Load("res://Scenes/Entities/Creatures/Body_parts/Eye/Eyes/Eye_01.tres");
        CreateEye(
            eyeScene,
            eyeData,
            new(0, 0.5f, -1),
            currentNeuronIndex,
            out currentNeuronIndex
        );

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
        brainInputNeurons += currentNeuronIndex;
        brainOutputNeurons += 2;

        // Mouths
        PackedScene mouthScene = (PackedScene)ResourceLoader.Load("res://Scenes/Entities/Creatures/Body_parts/Mouth/Mouth.tscn");
        MouthData mouthData = (MouthData)ResourceLoader.Load("res://Scenes/Entities/Creatures/Body_parts/Mouth/Mouths/Mouth_001.tres");
        CreateMouth(
            mouthScene,
            mouthData,
            new(0, -0.5f, -1),
            currentNeuronIndex,
            out currentNeuronIndex
        );

        brainInputNeurons += currentNeuronIndex;
        brainOutputNeurons += 1;
    }

    private void CreateEye(PackedScene eyeScene, EyeData eyeData, Vector3 angle, int currentNeuronIndex, out int neuronLastIndex)
    {
        BodyPart eye = eyeScene.Instantiate() as BodyPart;
        eye.Name = $"Eye_{eye.GetInstanceId()}";
        eye.Angle = angle;
        Eye tmp = eye as Eye;
        tmp._eyeData = eyeData;
        var neuronsNum = tmp._eyeData.EyeComplexity * tmp._eyeData.ActivatorPerEntity;
        tmp._neuronInputIndexes = new int[neuronsNum];
        for (int i = 0; i < neuronsNum; i++)
        {
            tmp._neuronInputIndexes[i] = currentNeuronIndex++;
        }
        neuronLastIndex = currentNeuronIndex;
        _bodyParts.Add(eye);
    }

    private void CreateMouth(PackedScene mouthScene, MouthData mouthData, Vector3 angle, int currentNeuronIndex, out int neuronLastIndex)
    {
        BodyPart mouth = mouthScene.Instantiate() as BodyPart;
        mouth.Name = $"Mouth_{mouth.GetInstanceId()}";
        mouth.Angle = angle;
        Mouth tmp = mouth as Mouth;
        tmp._mouthData = mouthData;
        tmp._neuronInputIndexes = new int[1];
        tmp._neuronInputIndexes[0] = currentNeuronIndex++;

        neuronLastIndex = currentNeuronIndex;
        _bodyParts.Add(mouth);
    }

    private void InitializeBrain()
    {
        int inputNeurons = brainInputNeurons;
        int outputNeurons = brainOutputNeurons;
        int totalNeurons = brainInputNeurons + brainOutputNeurons + GD.RandRange(10, 20);
        int minConnections = GD.RandRange(3, 5);
        int maxConnections = GD.RandRange(minConnections, 10);

        _brain = new Brain(totalNeurons, inputNeurons, outputNeurons, minConnections, maxConnections, 1);
    }

    public Brain GetBrain()
    {
        return _brain;
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

    public EntityData GetEntityData()
    {
        return new EntityData
        {
            entityType = EntityType.Creature,
            size = _body.GetSize(),
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocityNeuron = _brain.GetNeuron(_brain.numTotalNeurons - brainInputNeurons);
        var rotationNeuron = _brain.GetNeuron(_brain.numTotalNeurons - brainInputNeurons + 1);

        Velocity = Vector3.Forward * velocityNeuron * 20;
        RotateY(rotationNeuron);
        Velocity = Velocity.Rotated(Vector3.Up, Rotation.Y);

        MoveAndSlide();
    }
}
