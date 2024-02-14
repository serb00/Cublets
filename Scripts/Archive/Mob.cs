using Godot;

public partial class Mob : CharacterBody3D
{
    [Export] float _velocity = 1;
    float _minVelocity = 0;
    float _maxVelocity = 20;
    [Export] float _rotation = 0; //from -0.1 to 0.1 with step 0.01
    float _minRotation = -0.1f;
    float _maxRotation = 0.1f;
    bool _visible = true;
    [Export] Area3D Mouth;
    int CurrentEnergy = 100;
    int MaxEnergy;

    Brain _myBrain;

    // Emitted when the player jumped on the mob.
    [Signal] public delegate void SquashedEventHandler();

    public Brain GetCreatureData()
    {
        // Return the relevant creature data
        return _myBrain;
    }

    public override void _PhysicsProcess(double delta)
    {
        // adjust velocity and angle to match parameters
        Velocity = Vector3.Forward * _velocity;
        RotateY(_rotation);
        Velocity = Velocity.Rotated(Vector3.Up, Rotation.Y);

        MoveAndSlide();
    }



    // This function will be called from the Main scene.
    public void Initialize(Vector3 startPosition)
    {
        // We position the mob by placing it at startPosition
        // and rotate it towards center of the world.
        LookAtFromPosition(startPosition, Vector3.Zero, Vector3.Up);
        // Rotate this mob randomly within range of -45 and +45 degrees,
        // so that it doesn't move directly towards the player.
        RotateY((float)GD.RandRange(-Mathf.Pi / 4.0, Mathf.Pi / 4.0));

        // We calculate a forward velocity that represents the speed.
        Velocity = Vector3.Forward * _velocity;
        // We then rotate the velocity vector based on the mob's Y rotation
        // in order to move in the direction the mob is looking.
        Velocity = Velocity.Rotated(Vector3.Up, Rotation.Y);
        _myBrain = new Brain(40, 8, 8, 4, 8, 1);

        // TODO: Make MaxEnergy dependent on size or something else.
        MaxEnergy = 2500;
    }

    public override void _Ready()
    {
        // Mouth.Connect("body_entered", nameof(OnBodyEnteredMouthArea));
    }

    private void OnAreaEnteredMouthArea(Node body)
    {
        if (body is IConsumable consumable)
        {
            CurrentEnergy += consumable.Consume();
            // Apply the effect, like increasing health
            GD.Print($"{this.Name}: Energy: {CurrentEnergy}.");
        }
    }

    private void _on_visible_on_screen_notifier_3d_screen_exited()
    {
        _visible = false;
    }

    private void _on_visible_on_screen_notifier_3d_screen_entered()
    {
        _visible = true;
    }

    public void Squash()
    {
        EmitSignal(SignalName.Squashed);
        QueueFree();
    }
}