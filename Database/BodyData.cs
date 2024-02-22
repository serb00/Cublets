using Godot;

[GlobalClass]
public partial class BodyData : Resource
{
    [Export] public string Name { get; set; }
    [Export] public PackedScene Scene { get; set; }
    [Export] public float Size { get; set; }

    // public BodyData(string name, PackedScene scene, float size)
    // {
    //     Name = name;
    //     Scene = scene;
    //     Size = size;
    // }

}
